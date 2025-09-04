document.addEventListener("DOMContentLoaded", () => {
  const token = localStorage.getItem("token") || localStorage.getItem("tokenAdmin");
    const usuario = JSON.parse(localStorage.getItem("usuario") || localStorage.getItem("admin") || "{}");

  if (!token || !usuario) {
    window.location.href = "login.html";
    return;
  }

  const tbody = document.querySelector("tbody");
  const totalPuntos = document.querySelector(".card.border-left-info .h5");
  const totalActivas = document.querySelector(".card.border-left-primary .h5");
  const totalCompletadas = document.querySelector(".card.border-left-success .h5");

  function cargarParticipaciones() {
    fetch(`https://localhost:7258/api/participaciones/usuario/${usuario.id}`, {
      headers: {
        Authorization: `Bearer ${token}`
      }
    })
      .then(res => {
        if (!res.ok) throw new Error("Error al obtener participaciones");
        return res.json();
      })
      .then(participaciones => {
        tbody.innerHTML = "";

        let puntos = 0;
        let activas = 0;
        let completadas = 0;

        participaciones.forEach(p => {
          const tr = document.createElement("tr");
          tr.innerHTML = `
            <td>${p.nombreDesafio || "Desafío sin nombre"}</td>
            <td>${new Date(p.fechaParticipacion).toISOString().split("T")[0]}</td>
            <td>${p.puntos}</td>
            <td>${p.estado}</td>
          `;
          tbody.appendChild(tr);

          puntos += p.puntos || 0;
          if (p.estado === "Activo") activas++;
          if (p.estado === "Completado") completadas++;
        });

        totalPuntos.textContent = puntos;
        totalActivas.textContent = activas;
        totalCompletadas.textContent = completadas;
      })
      .catch(err => {
        console.error(err);
        tbody.innerHTML = `<tr><td colspan="4" class="text-center text-danger">No se pudieron cargar las participaciones</td></tr>`;
        totalPuntos.textContent = "0";
        totalActivas.textContent = "0";
        totalCompletadas.textContent = "0";
      });
  }

  cargarParticipaciones();
});
