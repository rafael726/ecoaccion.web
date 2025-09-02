const API_BASE = "https://localhost:7258/api"; // Ajusta el puerto si cambia

// ----------- LOGIN -----------
document.getElementById("loginForm").addEventListener("submit", async (e) => {
  e.preventDefault();

  const email = document.getElementById("email").value;
  const password = document.getElementById("password").value;
  const alertBox = document.getElementById("loginAlert");

  try {
    const response = await fetch(`${API_BASE}/auth/login`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ correo: email, contraseña: password })
    });

    const data = await response.json();

    if (!response.ok) {
      throw new Error(data.message || "Credenciales inválidas");
    }

    // Guardar token y rol
    localStorage.setItem("token", data.token);
    localStorage.setItem("role", data.role);

    // Redirigir según rol
    if (data.role === "Admin") {
      window.location.href = "admin-dashboard.html";
    } else {
      window.location.href = "dashboard.html";
    }
  } catch (err) {
    alertBox.textContent = err.message;
    alertBox.classList.remove("d-none");
  }
});

// ----------- REGISTRO -----------
document.getElementById("registerForm").addEventListener("submit", async (e) => {
  e.preventDefault();

  const name = document.getElementById("regName").value;
  const email = document.getElementById("regEmail").value;
  const password = document.getElementById("regPassword").value;
  const confirm = document.getElementById("regConfirm").value;
  const alertBox = document.getElementById("registerAlert");

  if (password !== confirm) {
    alertBox.textContent = "Las contraseñas no coinciden.";
    alertBox.classList.remove("d-none", "alert-success");
    alertBox.classList.add("alert-danger");
    return;
  }

  try {
    const response = await fetch(`${API_BASE}/auth/register`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ nombre: name, correo: email, contraseña: password })
    });

    const data = await response.json();

    if (!response.ok) {
      throw new Error(data.message || "Error al registrar usuario");
    }

    alertBox.textContent = "✅ Registro exitoso, ahora puedes iniciar sesión.";
    alertBox.classList.remove("d-none", "alert-danger");
    alertBox.classList.add("alert-success");
  } catch (err) {
    alertBox.textContent = err.message;
    alertBox.classList.remove("d-none", "alert-success");
    alertBox.classList.add("alert-danger");
  }
});

// ----------- OLVIDÉ CONTRASEÑA -----------
document.getElementById("forgotForm").addEventListener("submit", async (e) => {
  e.preventDefault();

  const email = document.getElementById("forgotEmail").value;
  const alertBox = document.getElementById("forgotAlert");

  try {
    const response = await fetch(`${API_BASE}/auth/forgot-password`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ correo: email })
    });

    const data = await response.json();

    if (!response.ok) {
      throw new Error(data.message || "Error al enviar correo de recuperación");
    }

    alertBox.textContent = "📩 Se envió un enlace a tu correo.";
    alertBox.classList.remove("d-none", "alert-danger");
    alertBox.classList.add("alert-success");
  } catch (err) {
    alertBox.textContent = err.message;
    alertBox.classList.remove("d-none", "alert-success");
    alertBox.classList.add("alert-danger");
  }
});
