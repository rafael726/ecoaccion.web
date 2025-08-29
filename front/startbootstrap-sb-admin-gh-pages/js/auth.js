const API_BASE = "https://localhost:7258/api"; // ⚡ cámbialo por tu backend

function showAlert(el, type, text) {
    el.className = `alert alert-${type}`;
    el.textContent = text;
    el.classList.remove("d-none");
}
function hideAlert(el) {
    el.classList.add("d-none");
    el.textContent = "";
}
function setLoading(btn, loading) {
    if (!btn) return;
    btn.disabled = !!loading;
    btn.dataset.originalText = btn.dataset.originalText || btn.innerHTML;
    btn.innerHTML = loading ? "Procesando..." : btn.dataset.originalText;
}

// LOGIN
document.getElementById("loginForm")?.addEventListener("submit", async e => {
    e.preventDefault();
    const email = document.getElementById("email").value.trim();
    const password = document.getElementById("password").value;
    const alertBox = document.getElementById("loginAlert");
    const btn = document.getElementById("btnLogin");

    hideAlert(alertBox);
    setLoading(btn, true);

    try {
        const res = await fetch(`${API_BASE}/auth/login`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ email, password })
        });

        if (!res.ok) {
            showAlert(alertBox, "danger", "Credenciales inválidas.");
            return;
        }

        const data = await res.json();
        localStorage.setItem("token", data.token);
        localStorage.setItem("usuario", JSON.stringify(data.user || { email }));

        window.location.href = "index.html";
    } catch {
        showAlert(alertBox, "danger", "Error de conexión con el servidor.");
    } finally {
        setLoading(btn, false);
    }
});

// REGISTRO
document.getElementById("registerForm")?.addEventListener("submit", async e => {
    e.preventDefault();
    const nombre = document.getElementById("regName").value.trim();
    const email = document.getElementById("regEmail").value.trim();
    const password = document.getElementById("regPassword").value;
    const confirm = document.getElementById("regConfirm").value;
    const alertBox = document.getElementById("registerAlert");
    const btn = document.getElementById("btnRegister");

    hideAlert(alertBox);
    if (password !== confirm) {
        showAlert(alertBox, "danger", "Las contraseñas no coinciden.");
        return;
    }
    setLoading(btn, true);

    try {
        const res = await fetch(`${API_BASE}/auth/register`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ nombre, email, password })
        });

        if (!res.ok) {
            showAlert(alertBox, "danger", "No se pudo registrar.");
            return;
        }

        showAlert(alertBox, "success", "Usuario registrado. Ya puedes iniciar sesión.");
    } catch {
        showAlert(alertBox, "danger", "Error de conexión con el servidor.");
    } finally {
        setLoading(btn, false);
    }
});

// OLVIDÉ CONTRASEÑA
document.getElementById("forgotForm")?.addEventListener("submit", async e => {
    e.preventDefault();
    const email = document.getElementById("forgotEmail").value.trim();
    const alertBox = document.getElementById("forgotAlert");
    const btn = document.getElementById("btnForgot");

    hideAlert(alertBox);
    setLoading(btn, true);

    try {
        const res = await fetch(`${API_BASE}/auth/forgot-password`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ email })
        });

        if (!res.ok) {
            showAlert(alertBox, "danger", "No se pudo procesar la solicitud.");
            return;
        }
        showAlert(alertBox, "success", "Si el correo existe, te llegará un enlace.");
    } catch {
        showAlert(alertBox, "danger", "Error de conexión con el servidor.");
    } finally {
        setLoading(btn, false);
    }
});
