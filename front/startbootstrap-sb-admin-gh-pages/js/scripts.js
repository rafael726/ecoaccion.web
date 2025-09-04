// scripts.js

// Validar usuario y rol para mostrar vistas o redirigir
const user = JSON.parse(localStorage.getItem("user"));
if (!user) {
  window.location.href = "login.html";
} else {
  if (user.rol === "admin") {
    const adminView = document.getElementById("admin-view");
    const adminMenu = document.getElementById("admin-menu-item");
    if (adminView) adminView.style.display = "block";
    if (adminMenu) adminMenu.style.display = "block";
  } else if (user.rol === "user") {
    const userView = document.getElementById("user-view");
    if (userView) userView.style.display = "block";
  }
}

// Mostrar nombre del usuario en la interfaz
const usuario = JSON.parse(localStorage.getItem("usuario")) || { nombre: "Invitado" };
const nombreUsuarioElem = document.getElementById("nombreUsuario");
if (nombreUsuarioElem) {
  nombreUsuarioElem.innerText = usuario.nombre;
}

// Función para cargar desafíos
function cargarDesafios() {
 const token = localStorage.getItem("token") || localStorage.getItem("tokenAdmin");
  if (!token) {
    alert("No estás autenticado. Por favor inicia sesión.");
    window.location.href = "login.html";
    return;
  }

  const tbody = document.getElementById("desafios-tbody");
  const desafiosActivosElem = document.getElementById("desafiosActivos");
  const desafiosCompletadosElem = document.getElementById("desafiosCompletados");

  fetch("https://localhost:7258/api/desafios", {
    headers: {
      "Authorization": `Bearer ${token}`
    }
  })
  .then(res => {
    if (!res.ok) throw new Error("Error al cargar desafíos");
    return res.json();
  })
  .then(data => {
    let activos = 0;
    let completados = 0;
    tbody.innerHTML = "";

    data.forEach(d => {
      const tr = document.createElement("tr");
      tr.innerHTML = `
        <td>${d.nombre}</td>
        <td>${d.descripcion}</td>
        <td>${d.puntos}</td>
        <td>${d.estado}</td>
      `;
      tbody.appendChild(tr);

      if (d.estado.toLowerCase() === "activo") activos++;
      if (d.estado.toLowerCase() === "completado") completados++;
    });

    desafiosActivosElem.innerText = activos;
    desafiosCompletadosElem.innerText = completados;
  })
  .catch(err => {
    alert(err.message);
    tbody.innerHTML = `<tr><td colspan="4" class="text-center text-danger">No se pudieron cargar los desafíos.</td></tr>`;
  });
}

// Ejecutar cargarDesafios solo si estamos en la página desafios.html
if (window.location.pathname.includes("desafios.html")) {
  cargarDesafios();
}
