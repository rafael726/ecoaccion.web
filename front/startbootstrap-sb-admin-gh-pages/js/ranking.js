// js/ranking.js
(() => {
  // Ejecuta sólo en ranking.html
  const isRankingPage =
    location.pathname.endsWith("ranking.html") ||
    document.getElementById("ranking-tbody");
  if (!isRankingPage) return;

  const API_BASE = window.API_BASE || "https://localhost:7258/api";
  const USERS_ENDPOINT = `${API_BASE}/User`; // ajusta si tu backend usa otra ruta

  // ====== Helpers Auth / JWT ======
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
  const padB64 = (s) => s.replace(/-/g, "+").replace(/_/g, "/").padEnd(Math.ceil(s.length / 4) * 4, "=");
  const getEmailFromJWT = () => {
    const jwt = getRawToken();
    if (!jwt) return localStorage.getItem("userEmail") || "";
    try {
      const [, p] = jwt.split(".");
      const payload = JSON.parse(atob(padB64(p || "")) || "{}");
      const email = payload?.email || "";
      if (email) localStorage.setItem("userEmail", email);
      return email;
    } catch {
      return localStorage.getItem("userEmail") || "";
    }
  };

  // ====== Fetch seguro ======
  async function safeFetchJson(url, options = {}) {
    const res = await fetch(url, {
      headers: { Accept: "application/json", ...(options.headers || {}) },
      ...options,
    });
    if (!res.ok) {
      if (res.status === 401) {
        alert("Sesión expirada. Inicia sesión.");
        localStorage.removeItem("token");
        localStorage.removeItem("tokenAdmin");
        location.href = "login.html";
        throw new Error("401");
      }
      const t = await res.text().catch(() => "");
      throw new Error(`HTTP ${res.status} - ${t || res.statusText}`);
    }
    if (res.status === 204) return null;
    return res.json();
  }

  // ====== Normalizadores de usuario ======
  const getId = (u) => u.id ?? u.idUsuario ?? u.userId ?? u.usuarioId ?? null;
  const getNombre = (u) => u.nombreUsuario ?? u.nombre ?? u.name ?? "-";
  const getCorreo = (u) => u.correo ?? u.email ?? "-";
  const getPuntos = (u) => Number(u?.puntos ?? u?.puntosTotales ?? 0) || 0;

  // ====== Estilos para top-3 (inyectamos CSS) ======
  (function injectStyles() {
    const css = `
      .rank-1 { background-color: rgba(255,215,0,.12) !important; }    /* oro */
      .rank-2 { background-color: rgba(192,192,192,.12) !important; }  /* plata */
      .rank-3 { background-color: rgba(205,127,50,.12) !important; }   /* bronce */
    `;
    const style = document.createElement("style");
    style.textContent = css;
    document.head.appendChild(style);
  })();

  // ====== Render ======
  function renderRanking(users = []) {
    const tbody = document.getElementById("ranking-tbody");
    const me = (getEmailFromJWT() || "").toLowerCase();

    // Ordenar por puntos DESC
    const sorted = [...users].sort((a, b) => getPuntos(b) - getPuntos(a));

    // Construir filas
    const rows = sorted.map((u, idx) => {
      const pos = idx + 1;
      const nombre = getNombre(u);
      const correo = getCorreo(u);
      const puntos = getPuntos(u);
      const isMe = correo.toLowerCase() === me;
      const rankClass = pos === 1 ? "rank-1" : pos === 2 ? "rank-2" : pos === 3 ? "rank-3" : "";
      const meClass = isMe ? "table-info" : ""; // resalta tu fila
      const trClass = `${rankClass} ${meClass}`.trim();

      return `
        <tr class="${trClass}">
          <td>${pos}</td>
          <td>${nombre}${isMe ? ' <span class="text-muted">(tú)</span>' : ""}</td>
          <td>${puntos}</td>
        </tr>
      `;
    });

    tbody.innerHTML = rows.join("") || `
      <tr><td colspan="3" class="text-center text-muted">No hay usuarios para mostrar.</td></tr>
    `;
  }

  // ====== Cargar y pintar ======
  async function cargarRanking() {
    const tbody = document.getElementById("ranking-tbody");
    tbody.innerHTML = `
      <tr>
        <td colspan="3" class="text-center">
          <span class="spinner-border spinner-border-sm"></span> Cargando...
        </td>
      </tr>
    `;
    try {
      const data = await safeFetchJson(USERS_ENDPOINT, {
        headers: { Authorization: getAuthHeader() },
      });
      const list = Array.isArray(data) ? data : [];
      renderRanking(list);
    } catch (err) {
      console.error(err);
      tbody.innerHTML = `
        <tr><td colspan="3" class="text-center text-danger">
          ${err.message || "No se pudo cargar el ranking."}
        </td></tr>
      `;
    }
  }

  document.addEventListener("DOMContentLoaded", cargarRanking);
})();
