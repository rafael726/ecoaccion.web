// js/desafios.js
(() => {
  // Ejecuta solo en la página de desafíos
  const isDesafiosPage =
    location.pathname.endsWith("desafios.html") ||
    document.getElementById("desafios-tbody");
  if (!isDesafiosPage) return;

  // Usa window.API_BASE si ya lo definiste; si no, pon aquí tu base
  const API_BASE = window.API_BASE || "https://localhost:7258/api";

  // ======= AUTH CONFIG (AJUSTADO A TU BACK) =======
  const AUTH = {
    storage: "local", // 'local' o 'session'
    tokenKeys: ["token", "tokenAdmin"], // llaves donde guardas el JWT
    userKeys: ["user", "usuario"], // llaves donde guardas el objeto user (opcional)
    roleClaims: [
      // ClaimTypes.Role en tu back:
      "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
      // fallbacks por si cambia
      "role",
      "roles",
    ],
    idClaim: "sub", // JwtRegisteredClaimNames.Sub
  };

  // ===== Helpers de storage / auth =====
  function getStore() {
    return AUTH.storage === "session" ? sessionStorage : localStorage;
  }
  function readFirstKey(keys = []) {
    const s = getStore();
    for (const k of keys) {
      const v = s.getItem(k);
      if (v) return v;
    }
    return "";
  }
  function getUser() {
    const s = getStore();
    for (const k of AUTH.userKeys) {
      const raw = s.getItem(k);
      if (raw) {
        try {
          return JSON.parse(raw);
        } catch {}
      }
    }
    return null;
  }
  function getToken() {
    const raw = readFirstKey(AUTH.tokenKeys).trim();
    // limpia posible "Bearer "
    return raw.replace(/^Bearer\s+/i, "");
  }
  function getAuthHeader() {
    const jwt = getToken();
    return jwt ? `Bearer ${jwt}` : "";
  }
  function decodePayload(jwt) {
    try {
      const [, p] = jwt.split(".");
      const pad = (s) =>
        s.replace(/-/g, "+").replace(/_/g, "/").padEnd(Math.ceil(s.length / 4) * 4, "=");
      return JSON.parse(atob(pad(p || "")));
    } catch {
      return {};
    }
  }
  function getRole() {
    // 1) desde el objeto guardado
    const u = getUser();
    if (u?.rol) return String(u.rol).toLowerCase();

    // 2) desde el JWT
    const jwt = getToken();
    if (!jwt) return "";
    const payload = decodePayload(jwt);

    for (const claim of AUTH.roleClaims) {
      const val = payload?.[claim];
      if (!val) continue;
      if (Array.isArray(val)) return String(val[0]).toLowerCase();
      return String(val).toLowerCase();
    }
    return "";
  }
  function getCurrentAdminId() {
    const u = getUser();
    if (u?.id) return parseInt(u.id, 10);

    const jwt = getToken();
    const payload = decodePayload(jwt);
    const idVal = payload?.[AUTH.idClaim];
    return idVal != null ? parseInt(idVal, 10) : 0;
  }
  function requireAuthOrRedirect() {
    const t = getToken();
    if (!t) {
      alert("Sesión expirada. Inicia sesión.");
      location.href = "login.html";
      return null;
    }
    return t;
  }

  // ===== Fetch JSON con manejo de 401 =====
  async function safeFetchJson(url, options = {}) {
    const res = await fetch(url, {
      headers: { Accept: "application/json", ...(options.headers || {}) },
      ...options,
    });
    if (!res.ok) {
      if (res.status === 401) {
        const s = getStore();
        s.removeItem("token");
        s.removeItem("tokenAdmin");
        alert("Sesión expirada. Inicia sesión.");
        location.href = "login.html";
        throw new Error("401");
      }
      const text = await res.text().catch(() => "");
      throw new Error(`HTTP ${res.status} - ${text || res.statusText}`);
    }
    if (res.status === 204) return null;
    return res.json();
  }

  // ===== Utilidades de fecha =====
  function toInputLocalValue(iso) {
    if (!iso) return "";
    const d = new Date(iso);
    if (isNaN(d)) return "";
    const p = (n) => String(n).padStart(2, "0");
    return `${d.getFullYear()}-${p(d.getMonth() + 1)}-${p(d.getDate())}T${p(
      d.getHours()
    )}:${p(d.getMinutes())}`;
  }

  // ===== Detección de rol admin (para acciones) =====
  const state = { cache: [], isAdmin: false };
 function initRole() {
  const role = getRole ? getRole() : ( // si pegaste la versión con getRole()
    ((JSON.parse(localStorage.getItem("user") || localStorage.getItem("usuario") || "{}")?.rol) || "").toLowerCase()
  );
  state.isAdmin = role === "admin";

  // Botón "Crear desafío"
  const btn = document.getElementById("btnNuevoDesafio");
  if (btn) btn.style.display = state.isAdmin ? "inline-block" : "none";

  // Encabezado de la columna Acciones
  const thAcc = document.getElementById("thAcciones");
  if (thAcc) thAcc.style.display = state.isAdmin ? "" : "none";
}

  // ===== Render de tabla y cards =====
 function renderTable() {
  const tbody = document.getElementById("desafios-tbody");
  const hoy = new Date();

  const rows = state.cache.map((d) => {
    const ini = d.fechaInicio ? new Date(d.fechaInicio) : null;
    const fin = d.fechaFin ? new Date(d.fechaFin) : null;
    const activo = ini && fin && hoy >= ini && hoy <= fin;
    const finalizado = fin && hoy > fin;
    const badge = activo
      ? 'success">Activo'
      : finalizado
      ? 'secondary">Finalizado'
      : 'info">Próximo';

    // celdas comunes (5)
    let tds = `
      <td>${d.titulo ?? "-"}</td>
      <td>${d.descripcion ?? "-"}</td>
      <td>${d.meta ?? "-"}</td>
      <td>${toInputLocalValue(d.fechaInicio).replace("T", " ")} — ${toInputLocalValue(d.fechaFin).replace("T", " ")}</td>
      <td><span class="badge badge-${badge}</span></td>
    `;

    // agrega "Acciones" solo si es admin (6ta celda)
    if (state.isAdmin) {
      tds += `
        <td class="text-nowrap" style="width:120px;">
          <button class="btn btn-sm btn-primary btn-edit" data-id="${d.idDesafio}" title="Editar"><i class="fas fa-edit"></i></button>
          <button class="btn btn-sm btn-danger btn-delete" data-id="${d.idDesafio}" title="Eliminar"><i class="fas fa-trash"></i></button>
        </td>
      `;
    }

    return `<tr>${tds}</tr>`;
  });

  tbody.innerHTML = rows.join("");

  // Actualiza cards
  const activos = state.cache.filter((d) => {
    const ini = d.fechaInicio ? new Date(d.fechaInicio) : null;
    const fin = d.fechaFin ? new Date(d.fechaFin) : null;
    return ini && fin && hoy >= ini && hoy <= fin;
  }).length;

  const finalizados = state.cache.filter((d) => {
    const fin = d.fechaFin ? new Date(d.fechaFin) : null;
    return fin && hoy > fin;
  }).length;

  const aEl = document.getElementById("desafiosActivos");
  const cEl = document.getElementById("desafiosCompletados");
  if (aEl) aEl.textContent = activos;
  if (cEl) cEl.textContent = finalizados;
}

  // ===== CRUD =====
  async function cargarDesafios() {
    const token = requireAuthOrRedirect();
    if (!token) return;

    const tbody = document.getElementById("desafios-tbody");
    tbody.innerHTML = `<tr><td colspan="6" class="text-center"><span class="spinner-border spinner-border-sm"></span> Cargando...</td></tr>`;
    try {
      const data = await safeFetchJson(`${API_BASE}/Desafio`, {
        headers: { Authorization: getAuthHeader() },
      });
      state.cache = Array.isArray(data) ? data : [];
      renderTable();
    } catch (e) {
      console.error(e);
      tbody.innerHTML = `<tr><td colspan="6" class="text-center text-danger">${
        e.message || "No se pudieron cargar los desafíos."
      }</td></tr>`;
    }
  }

  function openCreate() {
    document.getElementById("desafioModalTitle").textContent = "Crear desafío";
    document.getElementById("dId").value = "";
    document.getElementById("dTitulo").value = "";
    document.getElementById("dDescripcion").value = "";
    document.getElementById("dMeta").value = "";
    document.getElementById("dFechaInicio").value = "";
    document.getElementById("dFechaFin").value = "";
    $("#desafioModal").modal("show");
  }

  function openEdit(id) {
    const d = state.cache.find((x) => x.idDesafio === id);
    if (!d) return;
    document.getElementById("desafioModalTitle").textContent = "Editar desafío";
    document.getElementById("dId").value = d.idDesafio;
    document.getElementById("dTitulo").value = d.titulo || "";
    document.getElementById("dDescripcion").value = d.descripcion || "";
    document.getElementById("dMeta").value = d.meta || "";
    document.getElementById("dFechaInicio").value = toInputLocalValue(d.fechaInicio);
    document.getElementById("dFechaFin").value = toInputLocalValue(d.fechaFin);
    $("#desafioModal").modal("show");
  }

  async function guardarDesafio(e) {
    e.preventDefault();
    const id = document.getElementById("dId").value.trim();
    const body = {
      titulo: document.getElementById("dTitulo").value.trim(),
      descripcion: document.getElementById("dDescripcion").value.trim(),
      meta: document.getElementById("dMeta").value.trim(),
      idAdmin: getCurrentAdminId(),
      fechaInicio: new Date(document.getElementById("dFechaInicio").value).toISOString(),
      fechaFin: new Date(document.getElementById("dFechaFin").value).toISOString(),
    };

    try {
      const method = id ? "PUT" : "POST";
      const url = id ? `${API_BASE}/Desafio/${id}` : `${API_BASE}/Desafio`;
      const res = await fetch(url, {
        method,
        headers: {
          "Content-Type": "application/json",
          Authorization: getAuthHeader(),
        },
        body: JSON.stringify(body),
      });
      if (!res.ok) {
        const txt = await res.text().catch(() => "");
        throw new Error(`HTTP ${res.status} - ${txt || res.statusText}`);
      }

      $("#desafioModal").modal("hide");
      await cargarDesafios();
      alert("Guardado correctamente");
    } catch (err) {
      console.error(err);
      alert("No se pudo guardar el desafío");
    }
  }

  async function eliminarDesafio(id) {
    if (!confirm("¿Eliminar este desafío?")) return;
    try {
      const res = await fetch(`${API_BASE}/Desafio/${id}`, {
        method: "DELETE",
        headers: { Authorization: getAuthHeader() },
      });
      if (!res.ok) {
        const t = await res.text().catch(() => "");
        throw new Error(`HTTP ${res.status} - ${t || res.statusText}`);
      }
      await cargarDesafios();
      alert("Desafío eliminado");
    } catch (err) {
      console.error(err);
      alert("No se pudo eliminar el desafío");
    }
  }

  // ===== Eventos =====
 document.addEventListener("DOMContentLoaded", () => {
  initRole();
  cargarDesafios();

  document.getElementById("btnNuevoDesafio")?.addEventListener("click", openCreate);
  document.getElementById("formDesafio")?.addEventListener("submit", guardarDesafio);

  // Solo escuchar acciones si es admin
  if (state.isAdmin) {
    document.getElementById("desafios-tbody")?.addEventListener("click", (e) => {
      const edit = e.target.closest(".btn-edit");
      const del = e.target.closest(".btn-delete");
      if (edit) openEdit(parseInt(edit.dataset.id, 10));
      if (del) eliminarDesafio(parseInt(del.dataset.id, 10));
    });
  }
});
})();
