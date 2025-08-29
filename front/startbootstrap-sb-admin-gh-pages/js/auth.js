const API_BASE = "https://localhost:7258/api"; // Ajusta si cambia el puerto

// ----------- REGISTRO ADMIN -----------
document.getElementById("registerAdminForm").addEventListener("submit", async (e) => {
  e.preventDefault();

  const payload = {
    nombreUsuario: document.getElementById("admin_nombreUsuario").value,
    correo: document.getElementById("admin_correo").value,
    contraseña: document.getElementById("admin_contraseña").value,
    confirmarContraseña: document.getElementById("admin_confirmarContraseña").value
  };

  try {
    const response = await fetch(`${API_BASE}/Admin`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(payload)
    });

    if (!response.ok) throw new Error("Error en el registro Admin");
    const data = await response.json();
    alert("✅ Admin registrado con éxito: " + data.nombreUsuario);
  } catch (err) {
    console.error(err);
    alert("❌ No se pudo registrar Admin");
  }
});

// ----------- LOGIN ADMIN -----------
document.getElementById("loginAdminForm").addEventListener("submit", async (e) => {
  e.preventDefault();

  const payload = {
    correo: document.getElementById("adminLogin_correo").value,
    contraseña: document.getElementById("adminLogin_contraseña").value
  };

  try {
    const response = await fetch(`${API_BASE}/Admin/login`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(payload)
    });

    if (!response.ok) throw new Error("Error en login Admin");
    const data = await response.json();
    localStorage.setItem("tokenAdmin", data.token); // Guarda token si backend devuelve JWT
    alert("✅ Admin logueado con éxito");
  } catch (err) {
    console.error(err);
    alert("❌ No se pudo iniciar sesión Admin");
  }
});

// ----------- REGISTRO USUARIO -----------
document.getElementById("registerUserForm").addEventListener("submit", async (e) => {
  e.preventDefault();

  const payload = {
    nombreUsuario: document.getElementById("user_nombreUsuario").value,
    correo: document.getElementById("user_correo").value,
    contraseña: document.getElementById("user_contraseña").value,
    confirmarContraseña: document.getElementById("user_confirmarContraseña").value
  };

  try {
    const response = await fetch(`${API_BASE}/Usuario`, { // ⚡ Ajusta si tu endpoint es diferente
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(payload)
    });

    if (!response.ok) throw new Error("Error en el registro Usuario");
    const data = await response.json();
    alert("✅ Usuario registrado con éxito: " + data.nombreUsuario);
  } catch (err) {
    console.error(err);
    alert("❌ No se pudo registrar Usuario");
  }
});

// ----------- LOGIN USUARIO -----------
document.getElementById("loginUserForm").addEventListener("submit", async (e) => {
  e.preventDefault();

  const payload = {
    correo: document.getElementById("userLogin_correo").value,
    contraseña: document.getElementById("userLogin_contraseña").value
  };

  try {
    const response = await fetch(`${API_BASE}/Usuario/login`, { // ⚡ Ajusta si tu endpoint es diferente
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(payload)
    });

    if (!response.ok) throw new Error("Error en login Usuario");
    const data = await response.json();
    localStorage.setItem("tokenUser", data.token); // Guarda token si backend devuelve JWT
    alert("✅ Usuario logueado con éxito");
  } catch (err) {
    console.error(err);
    alert("❌ No se pudo iniciar sesión Usuario");
  }
});
