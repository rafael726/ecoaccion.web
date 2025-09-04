const API_BASE = "https://localhost:7258/api"; // Ajusta si cambia el puerto


function hideAlert(el) {
    el.classList.add("d-none");
    el.textContent = "";
}

function showAlert(el, type, text) {
    el.className = `alert alert-${type}`;
    el.textContent = text;
    el.classList.remove("d-none");
}


function setLoading(btn, loading) {
    if (!btn) return;
    btn.disabled = !!loading;
    btn.dataset.originalText = btn.dataset.originalText || btn.innerHTML;
    btn.innerHTML = loading ? "Procesando..." : btn.dataset.originalText;
}

// ----------- REGISTRO ADMIN -----------
document.getElementById("registerAdminForm")?.addEventListener("submit", async (e) => {
    e.preventDefault();

    // Construir el objeto con los valores del formulario
    const adminData = {
        nombreUsuario: document.getElementById("admin_nombreUsuario").value,
        correo: document.getElementById("admin_correo").value,
        contraseña: document.getElementById("admin_contraseña").value,
        confirmarContraseña: document.getElementById("admin_confirmarContraseña").value
    };

    const rolConfig = document.getElementById("regRole").value;

    try {
        const response = await fetch(`${API_BASE}/` + rolConfig, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(adminData)
        });

        if (!response.ok) throw await response.json();
        const data = await response.json();
        alert("✅ Usuario registrado con éxito: " + data.nombreUsuario);
    } catch (err) {
        console.error(err);
        alert("❌ No se pudo registrar Usuario: " + JSON.stringify(err));
    }
});

// ----------- LOGIN ADMIN -----------
document.getElementById("loginAdminForm")?.addEventListener("submit", async (e) => {
    e.preventDefault();

    const correo = document.getElementById("adminLogin_correo").value.trim();
    const contraseña = document.getElementById("adminLogin_contraseña").value;
    const alertBox = document.getElementById("loginAlert"); // div para mensajes
    const btn = document.getElementById("btnLoginAdmin"); // botón login
    const rolConfig = document.getElementById("loginRol").value;
   
    setLoading(btn, true);

    try {
        const response = await fetch(`${API_BASE}/` + rolConfig + `/login`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ Correo: correo, Contraseña: contraseña })
        });

        if (!response.ok) {
            showAlert(alertBox, "danger", "❌ Credenciales inválidas.");
            return;
        }

        const data = await response.json();

        // Guardar token en localStorage
        localStorage.setItem("tokenAdmin", data.token);
        localStorage.setItem("admin", JSON.stringify(data.admin || { correo }));

        // Redirigir al index
        window.location.href = "index.html";
    } catch (err) {
        console.error(err);
        showAlert(alertBox, "danger", "⚠️ Error de conexión con el servidor.");
    } finally {
        setLoading(btn, false);
    }
});