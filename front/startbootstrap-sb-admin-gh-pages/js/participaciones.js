// js/participaciones.js
(() => {
  // Ejecuta solo en participaciones.html
  const isPage = location.pathname.endsWith("participaciones.html") || document.getElementById("part-tbody");
  if (!isPage) return;

  const API_BASE = window.API_BASE || "https://localhost:7258/api";
  const ENDPOINT = `${API_BASE}/Participacion`;

  // ===== Auth & rol helpers =====
  const tokenKeys = ["token", "tokenAdmin"];
  const getRawToken = () => {
    for (const k of tokenKeys) {
      const v = localStorage.getItem(k);
      if (v) return v.replace(/^Bearer\s+/i, "").trim();
    }
    return "";
  };
  const getAuthHeader = () => {
    const t = getRawToken();
    return t ? `Bearer ${t}` : "";
  };
  const padB64 = (s) => s.replace(/-/g, "+").replace(/_/g, "/").padEnd(Math.ceil(s.length/4)*4, "=");
  const decodeJWT = () => {
    const t = getRawToken();
    if (!t) return {};
    const parts = t.split(".");
    if (parts.length < 2) return {};
    try { return JSON.parse(atob(padB64(parts[1]))); } catch { return {}; }
  };
  const CLAIM_ROLE = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
  const getRole = () => {
    const p = decodeJWT();
    const r = p[CLAIM_ROLE] || p.role || p.roles || "";
    return Array.isArray(r) ? String(r[0]).toLowerCase() : String(r).toLowerCase();
  };
  const getUserId = () => {
    const p = decodeJWT();
    const sub = p.sub || p.nameid || p.nameId;
    const n = parseInt(sub, 10);
    return isNaN(n) ? 0 : n;
  };
  const requireAuthOrRedirect = () => {
    if (!getRawToken()) {
      alert("Sesión expirada. Inicia sesión.");
      location.href = "login.html";
      return false;
    }
    return true;
  };

  // ===== Fetch seguro =====
  async function safeFetchJson(url, options = {}) {
    const res = await fetch(url, {
      headers: { Accept: "application/json", ...(options.headers || {}) },
      ...options,
    });
    if (!res.ok) {
      if (res.status === 401) {
        localStorage.removeItem("token");
        localStorage.removeItem("tokenAdmin");
        alert("Sesión expirada. Inicia sesión.");
        location.href = "login.html";
        throw new Error("401");
      }
      const t = await res.text().catch(() => "");
      throw new Error(`HTTP ${res.status} - ${t || res.statusText}`);
    }
    if (res.status === 204) return null;
    return res.json();
  }

  // ===== Utilidades de fecha =====
  const toInputLocal = (iso) => {
    if (!iso) return "";
    const d = new Date(iso);
    if (isNaN(d)) return "";
    const p = (x) => String(x).padStart(2, "0");
    return `${d.getFullYear()}-${p(d.getMonth()+1)}-${p(d.getDate())}T${p(d.getHours())}:${p(d.getMinutes())}`;
  };
  const toHuman = (iso) => {
    if (!iso) return "-";
    const d = new Date(iso);
    if (isNaN(d)) return "-";
    return d.toLocaleString();
  };

  // ===== Estado global =====
  const state = {
    cache: [],
    role: "user",
    userId: 0,
  };

  // ===== Render tabla =====
  function renderTable() {
    const tbody = document.getElementById("part-tbody");
    const isAdmin = state.role === "admin";

    const rows = state.cache.map((p, idx) => {
      const id = p.idPart ?? p.id ?? 0;
      const canEdit = isAdmin || (p.idUsuario === state.userId);
      const evidenciaUrl = `${ENDPOINT}/${id}/image`;
      const evCell = `
        <a class="btn btn-sm btn-outline-secondary" href="${evidenciaUrl}" target="_blank">
          Ver
        </a>
      `;
      

      return `
        <tr>
          <td>${idx + 1}</td>
          <td>${p.tituloDesafio ?? "-"}</td>
          <td>${p.progreso ?? "-"}</td>
          <td>${toHuman(p.fechaRegistro)}</td>
          <td class="text-nowrap">${evCell}</td>
          
        </tr>
      `;
    });

    tbody.innerHTML = rows.join("") || `
      <tr><td colspan="6" class="text-center text-muted">Sin participaciones aún.</td></tr>
    `;
  }

  // ===== Cargar participaciones =====
  async function cargarParticipaciones() {
    const tbody = document.getElementById("part-tbody");
    tbody.innerHTML = `
      <tr><td colspan="6" class="text-center">
        <span class="spinner-border spinner-border-sm"></span> Cargando...
      </td></tr>`;
    try {
      const data = await safeFetchJson(ENDPOINT, {
        headers: { Authorization: getAuthHeader() },
      });
      let list = Array.isArray(data) ? data : [];
      // Filtra por usuario si no es admin
      if (state.role !== "admin") {
        list = list.filter(x => (x.idUsuario ?? 0) === state.userId);
      }
      state.cache = list;
      renderTable();
    } catch (err) {
      console.error(err);
      tbody.innerHTML = `<tr><td colspan="6" class="text-center text-danger">${err.message || "No se pudieron cargar las participaciones."}</td></tr>`;
    }
  }

  // ===== Abrir crear / editar =====
  function openCreate() {
    document.getElementById("partModalTitle").textContent = "Nueva participación";
    document.getElementById("pIdPart").value = "";
    document.getElementById("pIdUsuario").value = state.userId;
    document.getElementById("pIdDesafio").value = "";
    document.getElementById("pTitulo").value = "";
    document.getElementById("pProgreso").value = "";
    document.getElementById("pFecha").value = toInputLocal(new Date().toISOString());
    document.getElementById("pEvidencia").value = "";
    $("#partModal").modal("show");
  }

  function openEdit(id) {
    const item = state.cache.find(x => (x.idPart ?? x.id) === id);
    if (!item) return;
    document.getElementById("partModalTitle").textContent = "Editar participación";
    document.getElementById("pIdPart").value = item.idPart ?? item.id ?? "";
    document.getElementById("pIdUsuario").value = item.idUsuario ?? state.userId;
    document.getElementById("pIdDesafio").value = item.idDesafio ?? "";
    document.getElementById("pTitulo").value = item.tituloDesafio ?? "";
    document.getElementById("pProgreso").value = item.progreso ?? "";
    document.getElementById("pFecha").value = toInputLocal(item.fechaRegistro);
    document.getElementById("pEvidencia").value = "";
    $("#partModal").modal("show");
  }

  // ===== Guardar (crear/editar) =====
  async function guardarPart(e) {
    e.preventDefault();
    const id = (document.getElementById("pIdPart").value || "").trim();
    const body = {
      idUsuario: parseInt(document.getElementById("pIdUsuario").value, 10) || state.userId,
      idDesafio: parseInt(document.getElementById("pIdDesafio").value, 10),
      tituloDesafio: document.getElementById("pTitulo").value.trim(),
      progreso: document.getElementById("pProgreso").value.trim(),
      fechaRegistro: new Date(document.getElementById("pFecha").value).toISOString(),
      // evidencia se maneja por endpoint separado
    };

    // Si no es admin y pretende editar/crear para otro usuario, bloquear
    if (state.role !== "admin" && body.idUsuario !== state.userId) {
      alert("No puedes crear/editar participaciones para otro usuario.");
      return;
    }

    try {
      const method = id ? "PUT" : "POST";
      const url = id ? `${ENDPOINT}/${id}` : ENDPOINT;

      const res = await fetch(url, {
        method,
        headers: {
          "Content-Type": "application/json",
          Authorization: getAuthHeader(),
        },
        body: JSON.stringify(body),
      });
      if (!res.ok) {
        const t = await res.text().catch(() => "");
        throw new Error(`HTTP ${res.status} - ${t || res.statusText}`);
      }

      // Obten el id (en creación lo devuelve el API, si no, usa el existente)
      const saved = await res.json().catch(() => null);
      const newId = (saved && (saved.idPart ?? saved.id)) || parseInt(id, 10);

      // Subir evidencia si se seleccionó un archivo
      const fileInput = document.getElementById("pEvidencia");
      const file = fileInput?.files?.[0];
      if (newId && file) {
        const fd = new FormData();
        // AJUSTA el nombre del campo si tu API espera otro (por ej. "image" o "file")
        fd.append("file", file);

        const up = await fetch(`${ENDPOINT}/${newId}/image`, {
          method: "POST", // si tu API usa PUT, cámbialo aquí
          headers: { Authorization: getAuthHeader() },
          body: fd,
        });
        if (!up.ok) {
          const t = await up.text().catch(() => "");
          console.warn("Evidencia no subida:", t || up.statusText);
          // No interrumpimos el guardado por la evidencia
        }
      }

      $("#partModal").modal("hide");
      await cargarParticipaciones();
      alert("Guardado correctamente");
    } catch (err) {
      console.error(err);
      alert("No se pudo guardar la participación.");
    }
  }

  // ===== Eliminar =====
  async function eliminarPart(id) {
    if (!confirm("¿Eliminar esta participación?")) return;
    try {
      const res = await fetch(`${ENDPOINT}/${id}`, {
        method: "DELETE",
        headers: { Authorization: getAuthHeader() },
      });
      if (!res.ok) {
        const t = await res.text().catch(() => "");
        throw new Error(`HTTP ${res.status} - ${t || res.statusText}`);
      }
      await cargarParticipaciones();
      alert("Participación eliminada");
    } catch (err) {
      console.error(err);
      alert("No se pudo eliminar la participación");
    }
  }

  // ===== Init =====
  document.addEventListener("DOMContentLoaded", () => {
    if (!requireAuthOrRedirect()) return;
    state.role = getRole() || "user";
    state.userId = getUserId() || 0;

    // Oculta botón "Nueva participación" si no hay sesión
    const btnNew = document.getElementById("btnNuevaPart");
    if (btnNew) {
      // Tanto admin como user pueden crear; si quisieras restringir a admin:
      // if (state.role !== "admin") btnNew.style.display = "none";
      btnNew.addEventListener("click", openCreate);
    }

    // Cargar lista
    cargarParticipaciones();

    // Submit guardar
    document.getElementById("formPart")?.addEventListener("submit", guardarPart);

    // Delegación acciones tabla
    document.getElementById("part-tbody")?.addEventListener("click", (e) => {
      const edit = e.target.closest(".btn-edit");
      const del = e.target.closest(".btn-del");
      if (edit) {
        const id = parseInt(edit.dataset.id, 10);
        openEdit(id);
      }
      if (del) {
        const id = parseInt(del.dataset.id, 10);
        // Solo admin o propietario
        const it = state.cache.find(x => (x.idPart ?? x.id) === id);
        if (!it) return;
        if (state.role !== "admin" && it.idUsuario !== state.userId) {
          alert("No puedes eliminar participaciones de otros usuarios.");
          return;
        }
        eliminarPart(id);
      }
    });
  });
})();
