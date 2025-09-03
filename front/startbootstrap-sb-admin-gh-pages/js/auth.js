const API_BASE = "https://localhost:7258/api"; // Ajusta si cambia el puerto

// ----------- REGISTRO ADMIN -----------
document.getElementById("registerAdminForm").addEventListener("submit", async (e) => {
    e.preventDefault();

    const formData = new URLSearchParams();
    formData.append("NombreUsuario", document.getElementById("admin_nombreUsuario").value);
    formData.append("Correo", document.getElementById("admin_correo").value);
    formData.append("Contraseña", document.getElementById("admin_contraseña").value);
    formData.append("ConfirmarContraseña", document.getElementById("admin_confirmarContraseña").value);

    try {
        const response = await fetch(`${API_BASE}/Admin`, {
            method: "POST",
            headers: { "Content-Type": "application/x-www-form-urlencoded" },
            body: formData.toString()
        });

        if (!response.ok) throw await response.json();
        const data = await response.json();
        alert("✅ Admin registrado con éxito: " + data.nombreUsuario);
    } catch (err) {
        console.error(err);
        alert("❌ No se pudo registrar Admin: " + JSON.stringify(err));
    }
});

// ----------- LOGIN ADMIN -----------
document.getElementById("loginAdminForm").addEventListener("submit", async (e) => {
    e.preventDefault();

    const formData = new URLSearchParams();
    formData.append("Correo", document.getElementById("adminLogin_correo").value);
    formData.append("Contraseña", document.getElementById("adminLogin_contraseña").value);

    try {
        const response = await fetch(`${API_BASE}/Admin/login`, {
            method: "POST",
            headers: { "Content-Type": "application/x-www-form-urlencoded" },
            body: formData.toString()
        });

        if (!response.ok) throw await response.json();
        const data = await response.json();
        localStorage.setItem("tokenAdmin", data.token);
        alert("✅ Admin logueado con éxito");
    } catch (err) {
        console.error(err);
        alert("❌ No se pudo iniciar sesión Admin: " + JSON.stringify(err));
    }
});

// ----------- REGISTRO USUARIO -----------
document.getElementById("registerUserForm").addEventListener("submit", async (e) => {
    e.preventDefault();

    const formData = new URLSearchParams();
    formData.append("NombreUsuario", document.getElementById("user_nombreUsuario").value);
    formData.append("Correo", document.getElementById("user_correo").value);
    formData.append("Contraseña", document.getElementById("user_contraseña").value);
    formData.append("ConfirmarContraseña", document.getElementById("user_confirmarContraseña").value);

    try {
        const response = await fetch(`${API_BASE}/User`, {
            method: "POST",
            headers: { "Content-Type": "application/x-www-form-urlencoded" },
            body: formData.toString()
        });

        if (!response.ok) throw await response.json();
        const data = await response.json();
        alert("✅ Usuario registrado con éxito: " + data.nombreUsuario);
    } catch (err) {
        console.error(err);
        alert("❌ No se pudo registrar Usuario: " + JSON.stringify(err));
    }
});

// ----------- LOGIN USUARIO -----------
document.getElementById("loginUserForm").addEventListener("submit", async (e) => {
    e.preventDefault();

    const formData = new URLSearchParams();
    formData.append("Correo", document.getElementById("userLogin_correo").value);
    formData.append("Contraseña", document.getElementById("userLogin_contraseña").value);

    try {
        const response = await fetch(`${API_BASE}/User/login`, {
            method: "POST",
            headers: { "Content-Type": "application/x-www-form-urlencoded" },
            body: formData.toString()
        });

        if (!response.ok) throw await response.json();
        const data = await response.json();
        localStorage.setItem("tokenUser", data.token);
        alert("✅ Usuario logueado con éxito");
    } catch (err) {
        console.error(err);
        alert("❌ No se pudo iniciar sesión Usuario: " + JSON.stringify(err));
    }
});


