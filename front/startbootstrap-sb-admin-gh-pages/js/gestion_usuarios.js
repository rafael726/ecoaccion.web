// js/gestion_usuarios.js
(() => {
  // Solo en esta página
  const isThisPage =
    location.pathname.endsWith("gestion_usuarios.html") ||
    document.getElementById("usuarios-tbody");
  if (!isThisPage) return;

  // ===== API =====
  const API_BASE = window.API_BASE || "https://localhost:7258/api";
  const USERS_ENDPOINT = `${API_BASE}/User`; // AJUSTA si tu backend usa otra ruta

  // ===== Auth / rol admin via JWT =====
  const AUTH = {
    storage: "local",
    tokenKeys: ["token", "tokenAdmin"],
    roleClaims: [
      "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
      "role",
      "roles",
    ],
  };
  const store = AUTH.storage === "session" ? sessionStorage : localStorage;

  function getTokenRaw() {
    for (const k of AUTH.tokenKeys) {
      const v = store.getItem(k);
      if (v) return v.replace(/^Bearer\s+/i, "").trim();
    }
    return "";
  }
  function getAuthHeader() {
    const t = getTokenRaw();
    return t ? `Bearer ${t}` : "";
  }
  function decodePayload(jwt) {
    try {
      const [, p] = jwt.split(".");
      const pad = (s) => s.replace(/-/g, "+").replace(/_/g, "/").padEnd(Math.ceil(s.length / 4) * 4, "=");
      return JSON.parse(atob(pad(p || "")));
    } catch { return {}; }
  }
  function getRole() {
    const token = getTokenRaw();
    if (!token) return "";
    const payload = decodePayload(token);
    for (const claim of AUTH.roleClaims) {
      const val = payload?.[claim];
      if (!val) continue;
      return Array.isArray(val) ? String(val[0]).toLowerCase() : String(val).toLowerCase();
    }
    return "";
  }
  function requireAdminOrRedirect() {
    const token = getTokenRaw();
    if (!token) {
      alert("Sesión expirada. Inicia sesión.");
      location.href = "login.html";
      return false;
    }
    if (getRole() !== "admin") {
      alert("No tienes permiso para acceder a esta página.");
      location.href = "index.html";
      return false;
    }
    return true;
  }

  // ===== Fetch seguro =====
  async function safeFetchJson(url, options = {}) {
    const res = await fetch(url, {
      headers: { Accept: "application/json", ...(options.headers || {}) },
      ...options,
    });
    if (!res.ok) {
      if (res.status === 401) {
        store.removeItem("token");
        store.removeItem("tokenAdmin");
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

  // ===== Normalizadores (por si el back usa otros nombres) =====
  const getId = (u) => u.id ?? u.idUsuario ?? u.userId ?? u.usuarioId ?? null;
  const getNombre = (u) => u.nombreUsuario ?? u.nombre ?? u.name ?? "-";
  const getCorreo = (u) => u.correo ?? u.email ?? "-";
  const getPuntos = (u) => u.puntos ?? u.puntosTotales ?? 0;

  // ===== Render tabla =====
  function renderTable(data = []) {
    const tbody = document.getElementById("usuarios-tbody");
    const rows = data.map((u) => {
      const id = getId(u);
      const nombre = getNombre(u);
      const correo = getCorreo(u);
      const puntos = getPuntos(u);
      return `
        <tr>
          <td>${id ?? "-"}</td>
          <td>${nombre}</td>
          <td>${correo}</td>
          <td>${puntos}</td>
          <td class="text-nowrap" style="width:120px;">
            <button class="btn btn-sm btn-primary btn-edit" data-id="${id}" title="Editar"><i class="fas fa-edit"></i></button>
            <button class="btn btn-sm btn-danger btn-del" data-id="${id}" title="Eliminar"><i class="fas fa-trash"></i></button>
          </td>
        </tr>`;
    });
    tbody.innerHTML = rows.join("");
  }

  // ===== Cargar usuarios =====
 async function cargarUsuarios() {
  const tbody = document.getElementById("usuarios-tbody");
  tbody.innerHTML = `<tr><td colspan="5" class="text-center"><span class="spinner-border spinner-border-sm"></span> Cargando...</td></tr>`;
  try {
    const data = await safeFetchJson(USERS_ENDPOINT, {
      headers: { Authorization: getAuthHeader() },
    });
    window.__USERS_CACHE__ = Array.isArray(data) ? data : [];
    renderTable(window.__USERS_CACHE__);

    // >>> actualiza Total Usuarios
    const cardTot = document.getElementById("cardTotal");
    if (cardTot) cardTot.textContent = window.__USERS_CACHE__.length;

  } catch (e) {
    console.error(e);
    tbody.innerHTML = `<tr><td colspan="5" class="text-center text-danger">${e.message || "No se pudieron cargar los usuarios."}</td></tr>`;
    const cardTot = document.getElementById("cardTotal");
    if (cardTot) cardTot.textContent = "0";
  }
}


  // ===== Crear =====
  async function crearUsuario(e) {
    e.preventDefault();
    const nombre = document.getElementById("crear-nombre").value.trim();
    const correo = document.getElementById("crear-correo").value.trim();
    const pass = document.getElementById("crear-pass").value;
    const pass2 = document.getElementById("crear-pass2").value;

    if (pass !== pass2) {
      Swal.fire({ icon: "warning", text: "Las contraseñas no coinciden" });
      return;
    }
    if (pass.length < 6) {
      Swal.fire({ icon: "warning", text: "La contraseña debe tener al menos 6 caracteres" });
      return;
    }

    const body = {
      nombreUsuario: nombre,
      correo,
      contraseña: pass,
      confirmarContraseña: pass2
    };

    try {
      const res = await fetch(USERS_ENDPOINT, {
        method: "POST",
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
      $("#crearUsuarioModal").modal("hide");
      document.getElementById("formCrearUsuario").reset();
      await cargarUsuarios();
      Swal.fire({ icon: "success", text: "Usuario creado correctamente" });
    } catch (err) {
      console.error(err);
      Swal.fire({ icon: "error", text: "No se pudo crear el usuario" });
    }
  }

  // ===== Abrir modal editar =====
  function abrirEditar(id) {
  const u = (window.__USERS_CACHE__ || []).find(x => getId(x) === id);
  if (!u) return;
  document.getElementById("editar-id").value = id;
  document.getElementById("editar-nombre").value = getNombre(u);
  document.getElementById("editar-correo").value = getCorreo(u);
  document.getElementById("editar-puntos").value = getPuntos(u); // <--- NUEVO
  document.getElementById("editar-pass").value = "";
  document.getElementById("editar-pass2").value = "";
  $("#editarUsuarioModal").modal("show");
}


  // ===== Guardar edición =====
 async function guardarEdicion(e) {
  e.preventDefault();
  const id = document.getElementById("editar-id").value;
  const nombre = document.getElementById("editar-nombre").value.trim();
  const correo = document.getElementById("editar-correo").value.trim();
  const puntosInput = document.getElementById("editar-puntos").value;
  const pass = document.getElementById("editar-pass").value;
  const pass2 = document.getElementById("editar-pass2").value;

  const puntos = parseInt(puntosInput, 10);
  if (isNaN(puntos) || puntos < 0) {
    Swal.fire({ icon: "warning", text: "Los puntos deben ser un número entero mayor o igual a 0." });
    return;
  }

  const body = {
    nombreUsuario: nombre,
    correo,
    puntos // <--- ENVÍA LOS PUNTOS
  };

  // Contraseña opcional
  if (pass || pass2) {
    if (pass !== pass2) {
      Swal.fire({ icon: "warning", text: "Las contraseñas no coinciden" });
      return;
    }
    if (pass.length < 6) {
      Swal.fire({ icon: "warning", text: "La contraseña debe tener al menos 6 caracteres" });
      return;
    }
    body.contraseña = pass;
    body.confirmarContraseña = pass2;
  }

  try {
    const res = await fetch(`${USERS_ENDPOINT}/${id}`, {
      method: "PUT",
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
    $("#editarUsuarioModal").modal("hide");
    await cargarUsuarios();
    Swal.fire({ icon: "success", text: "Usuario actualizado" });
  } catch (err) {
    console.error(err);
    Swal.fire({ icon: "error", text: "No se pudo actualizar el usuario" });
  }
}


  // ===== Eliminar =====
  async function eliminarUsuario(id) {
    const ok = await Swal.fire({
      icon: "warning",
      title: "¿Eliminar usuario?",
      text: "Esta acción no se puede deshacer.",
      showCancelButton: true,
      confirmButtonText: "Sí, eliminar",
      cancelButtonText: "Cancelar"
    }).then(r => r.isConfirmed);
    if (!ok) return;

    try {
      const res = await fetch(`${USERS_ENDPOINT}/${id}`, {
        method: "DELETE",
        headers: { Authorization: getAuthHeader() },
      });
      if (!res.ok) {
        const t = await res.text().catch(() => "");
        throw new Error(`HTTP ${res.status} - ${t || res.statusText}`);
      }
      await cargarUsuarios();
      Swal.fire({ icon: "success", text: "Usuario eliminado" });
    } catch (err) {
      console.error(err);
      Swal.fire({ icon: "error", text: "No se pudo eliminar el usuario" });
    }
  }

  // ===== Init =====
  document.addEventListener("DOMContentLoaded", () => {
    if (!requireAdminOrRedirect()) return;

    // Encabezado Acciones visible solo para admin (doble seguro)
    const thAcc = document.getElementById("thAcciones");
    if (getRole() === "admin" && thAcc) thAcc.style.display = "";

    // Cargar datos
    cargarUsuarios();

    // Crear
    document.getElementById("formCrearUsuario")?.addEventListener("submit", crearUsuario);

    // Editar
    document.getElementById("formEditarUsuario")?.addEventListener("submit", guardarEdicion);

    // Delegación eventos en tabla
    document.getElementById("usuarios-tbody")?.addEventListener("click", (e) => {
      const edit = e.target.closest(".btn-edit");
      const del = e.target.closest(".btn-del");
      if (edit) abrirEditar(parseInt(edit.dataset.id, 10));
      if (del) eliminarUsuario(parseInt(del.dataset.id, 10));
    });
  });
})();
