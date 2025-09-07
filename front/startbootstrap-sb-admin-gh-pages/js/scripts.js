// scripts.js

// ===== Config API (auto-detección local/prod) =====
const isLocal =
  location.hostname === "localhost" ||
  location.hostname === "127.0.0.1";

const API_HTTP_PORT = 5258; // ajusta si tu perfil HTTP usa otro puerto
const API_HTTPS_PORT = 7258; // tu puerto HTTPS actual

// Opción A (recomendada en desarrollo): usar HTTP en local para evitar certs
const USE_HTTP_IN_LOCAL = true;

const API_BASE = `https://localhost:7258/api`; // en prod cámbialo por tu dominio

// ===== Helpers de storage/auth =====
function getUserFromStorage() {
  // Unificar llaves "user" y "usuario"
  const u = JSON.parse(localStorage.getItem("user") || "null")
        || JSON.parse(localStorage.getItem("usuario") || "null");
  return u || null;
}

function getToken() {
  const t = localStorage.getItem("token") || localStorage.getItem("tokenAdmin") || "";
  // si el storage ya trae "Bearer ...", quítalo
  return t.replace(/^Bearer\s+/i, "");
}

function requireAuthOrRedirect() {
  const token = getToken();
  if (!token) {
    alert("No estás autenticado. Por favor inicia sesión.");
    window.location.href = "login.html";
    return null;
  }
  return token;
}

// ===== Mostrar nombre de usuario =====
(function showUserName() {
  const u = getUserFromStorage();
  const nombreUsuarioElem = document.getElementById("nombreUsuario");
  if (nombreUsuarioElem) {
    nombreUsuarioElem.innerText = (u && (u.nombre || u.name)) || "Invitado";
  }
})();

// ===== Visibilidad por rol (si el HTML define esos IDs) =====
(function setRoleViews() {
  const u = getUserFromStorage();
  if (!u) return;

  if (u.rol === "admin") {
    const adminView = document.getElementById("admin-view");
    const adminMenu = document.getElementById("admin-menu-item");
    if (adminView) adminView.style.display = "block";
    if (adminMenu) adminMenu.style.display = "block";
  } else if (u.rol === "user") {
    const userView = document.getElementById("user-view");
    if (userView) userView.style.display = "block";
  }
})();

// ===== Fetch seguro con mejor manejo de errores =====
async function safeFetchJson(url, options = {}) {
  try {
    const res = await fetch(url, {
      // 'cors' por defecto en navegadores; mantener explícito no hace daño
      mode: "cors",
      headers: {
        Accept: "application/json",
        ...(options.headers || {})
      },
      ...options
    });

    if (!res.ok) {
      // Si el backend no devuelve CORS correcto, el navegador puede bloquear y ni siquiera verás el status aquí.
      // Pero cuando hay status, mostramos detalle:
      const text = await res.text().catch(() => "");
      throw new Error(`HTTP ${res.status} - ${text || res.statusText || "Error de servidor"}`);
    }

    // Algunos endpoints devuelven 204 sin cuerpo
    if (res.status === 204) return null;

    return await res.json();
  } catch (err) {
    // Diferenciar causas típicas
    // - TypeError: Failed to fetch -> CORS/certificado/mixed content/servidor caído
    console.error("Fetch error:", err);
    let msg = err.message || "Error de red";
    if (msg.includes("Failed to fetch")) {
      msg = isLocal && USE_HTTP_IN_LOCAL
        ? "No se pudo conectar al API. Verifica que el puerto HTTP esté activo y CORS configurado."
        : "Fallo de conexión (posible certificado no confiable o CORS). Revisa swagger en el API y el candado HTTPS.";
    }
    throw new Error(msg);
  }
}

// ===== Cargar Desafíos =====
function fmtFecha(iso) {
  if (!iso) return "-";
  const d = new Date(iso);
  if (isNaN(d)) return iso;
  return d.toLocaleDateString("es-CO", { year: "numeric", month: "2-digit", day: "2-digit" });
}

async function cargarDesafios() {
  const token = requireAuthOrRedirect();
  if (!token) return;

  const tbody = document.getElementById("desafios-tbody");
  const desafiosActivosElem = document.getElementById("desafiosActivos");
  const desafiosCompletadosElem = document.getElementById("desafiosCompletados");

  if (tbody) {
    tbody.innerHTML = `
      <tr>
        <td colspan="5" class="text-center">
          <span class="spinner-border spinner-border-sm"></span> Cargando...
        </td>
      </tr>`;
  }

  try {
    const data = await safeFetchJson(`https://localhost:7258/api/Desafio`, {
      headers: {
         Authorization: `Bearer ${getToken()}`
      }
    });

    let activos = 0;
    let finalizados = 0; 

    if (tbody) tbody.innerHTML = "";

    const hoy = new Date();

    (data || []).forEach(d => {
      const ini = d.fechaInicio ? new Date(d.fechaInicio) : null;
      const fin = d.fechaFin ? new Date(d.fechaFin) : null;

      const estaActivo = ini && fin && (hoy >= ini && hoy <= fin);
      const estaFinalizado = fin && (hoy > fin);
      if (estaActivo) activos++;
      if (estaFinalizado) finalizados++;

      const tr = document.createElement("tr");
      tr.innerHTML = `
        <td>${d.titulo ?? "-"}</td>
        <td>${d.descripcion ?? "-"}</td>
        <td>${d.meta ?? "-"}</td>
        <td>${fmtFecha(d.fechaInicio)} — ${fmtFecha(d.fechaFin)}</td>
        <td>${estaActivo ? '<span class="badge badge-success">Activo</span>' :
                           estaFinalizado ? '<span class="badge badge-secondary">Finalizado</span>'
                                          : '<span class="badge badge-info">Próximo</span>'}
        </td>
      `;
      tbody && tbody.appendChild(tr);
    });

    if (desafiosActivosElem) desafiosActivosElem.innerText = activos;
    if (desafiosCompletadosElem) desafiosCompletadosElem.innerText = finalizados;

  } catch (err) {
    if (tbody) {
      tbody.innerHTML = `<tr><td colspan="5" class="text-center text-danger">
        ${err.message || "No se pudieron cargar los desafíos."}
      </td></tr>`;
    } else {
      alert(err.message || "No se pudieron cargar los desafíos.");
    }
  }
}


// Ejecutar solo en la página de desafíos
if (window.location.pathname.includes("desafios.html")) {
  cargarDesafios();
}
