const API_BASE = "https://localhost:7258/api";
const token = localStorage.getItem("token");

// Verificar sesión
if (!token) {
    window.location.href = "login.html";
}

// Ejemplo: traer datos protegidos
async function cargarDatos() {
    const response = await fetch(`${API_BASE}/usuarios/perfil`, {
        headers: { "Authorization": `Bearer ${token}` }
    });

    const data = await response.json();
    document.getElementById("nombreUsuario").innerText = data.nombre;
}

cargarDatos();
