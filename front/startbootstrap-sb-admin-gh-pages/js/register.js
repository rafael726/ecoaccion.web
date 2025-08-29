const API_BASE = "https://localhost:7258/api";
document.getElementById("registerForm")?.addEventListener("submit", async (e) => {
    e.preventDefault();
    const name = document.getElementById("regName").value.trim();
    const email = document.getElementById("regEmail").value.trim();
    const password = document.getElementById("regPassword").value.trim();
    const confirm = document.getElementById("regConfirm").value.trim();
    if (password !== confirm) {
        mostrarAlerta("registerAlert", "Las contraseñas no coinciden", "danger");
        return;
    }
    try {
        const res = await fetch(`${API_BASE}/auth/register`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ name, email, password })
        });
        if (!res.ok) throw new Error("Error al registrarse");
        mostrarAlerta("registerAlert", "Usuario registrado correctamente", "success");
        document.getElementById("registerForm").reset();
    } catch (err) {
        mostrarAlerta("registerAlert", err.message, "danger");
    }
});
function mostrarAlerta(id, mensaje, tipo) {
    const alert = document.getElementById(id);
    alert.textContent = mensaje;
    alert.className = `alert alert-${tipo}`;
    alert.classList.remove("d-none");
}