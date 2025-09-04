document.addEventListener("DOMContentLoaded", () => {
  const token = localStorage.getItem("token") || localStorage.getItem("tokenAdmin");
    const usuario = JSON.parse(localStorage.getItem("usuario") || localStorage.getItem("admin") || "{}");
  document.getElementById("nombreUsuario").innerText = usuario.nombre;

  if (!token) {
    window.location.href = "login.html";
    return;
  }

  const tbody = document.querySelector("tbody");

  fetch("https://localhost:7258/api/participaciones/ranking", {
    headers: {
      Authorization: `Bearer ${token}`
    }
  })
    .then(res => {
      if (!res.ok) throw new Error("Error al obtener el ranking");
      return res.json();
    })
    .then(usuarios => {
      tbody.innerHTML = "";

      usuarios.forEach((user, index) => {
        const tr = document.createElement("tr");
        tr.innerHTML = `
          <td>${index + 1}</td>
          <td>${user.nombre}</td>
          <td>${user.puntos}</td>
        `;
        tbody.appendChild(tr);
      });
    })
    .catch(error => {
      console.error("Error al cargar el ranking:", error);
      tbody.innerHTML = `<tr><td colspan="3" class="text-center text-danger">No se pudo cargar el ranking</td></tr>`;
    });
});
