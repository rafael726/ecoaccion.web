document.addEventListener("DOMContentLoaded", () => {
  const token = localStorage.getItem("token") || localStorage.getItem("tokenAdmin");
  
  if (!token) {
    window.location.href = "login.html";
    return;
  }

  const tbody = document.getElementById("usuarios-tbody");
  const formEditar = document.getElementById("formEditarUsuario");

  function cargarUsuarios() {
    fetch("https://localhost:7258/api/usuarios", {
      headers: { Authorization: `Bearer ${token}` }
    })
      .then(res => {
        if (!res.ok) throw new Error("Error al cargar usuarios");
        return res.json();
      })
      .then(usuarios => {
        tbody.innerHTML = "";
        usuarios.forEach(usuario => {
          const tr = document.createElement("tr");
          tr.innerHTML = `
            <td>${usuario.id}</td>
            <td>${usuario.nombre}</td>
            <td>${usuario.correo}</td>
            <td>${usuario.estado}</td>
            <td>${usuario.rol}</td>
            <td>
              <button class="btn btn-sm btn-primary editar-btn" data-id="${usuario.id}"><i class="fas fa-edit"></i></button>
              <button class="btn btn-sm btn-danger eliminar-btn" data-id="${usuario.id}"><i class="fas fa-trash"></i></button>
            </td>
          `;
          tbody.appendChild(tr);
        });

        // Agregar eventos después de cargar
        agregarEventosBotones();
      })
      .catch(err => {
        console.error(err);
        tbody.innerHTML = `<tr><td colspan="6" class="text-center text-danger">Error al cargar usuarios</td></tr>`;
      });
  }

  function agregarEventosBotones() {
    // Botones de editar
    document.querySelectorAll(".editar-btn").forEach(btn => {
      btn.addEventListener("click", e => {
        const id = e.currentTarget.dataset.id;
        abrirModalEditar(id);
      });
    });

    // Botones de eliminar
    document.querySelectorAll(".eliminar-btn").forEach(btn => {
      btn.addEventListener("click", e => {
        const id = e.currentTarget.dataset.id;
        eliminarUsuario(id);
      });
    });
  }

  function abrirModalEditar(id) {
    fetch(`https://localhost:7258/api/usuarios/${id}`, {
      headers: { Authorization: `Bearer ${token}` }
    })
      .then(res => res.json())
      .then(usuario => {
        document.getElementById("editar-id").value = usuario.id;
        document.getElementById("editar-nombre").value = usuario.nombre;
        document.getElementById("editar-correo").value = usuario.correo;
        document.getElementById("editar-estado").value = usuario.estado;
        document.getElementById("editar-rol").value = usuario.rol;

        $("#editarUsuarioModal").modal("show");
      })
      .catch(err => console.error("Error al cargar usuario:", err));
  }

  formEditar.addEventListener("submit", e => {
    e.preventDefault();

    const id = document.getElementById("editar-id").value;
    const usuarioActualizado = {
      nombre: document.getElementById("editar-nombre").value,
      correo: document.getElementById("editar-correo").value,
      estado: document.getElementById("editar-estado").value,
      rol: document.getElementById("editar-rol").value
    };

    fetch(`https://localhost:7258/api/usuarios/${id}`, {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`
      },
      body: JSON.stringify(usuarioActualizado)
    })
      .then(res => {
        if (!res.ok) throw new Error("Error al actualizar usuario");
        $("#editarUsuarioModal").modal("hide");
        cargarUsuarios();
      })
      .catch(err => console.error("Error al guardar cambios:", err));
  });

  function eliminarUsuario(id) {
  Swal.fire({
    title: '¿Seguro que deseas eliminar este usuario?',
    icon: 'warning',
    showCancelButton: true,
    confirmButtonText: 'Sí, eliminar',
    cancelButtonText: 'Cancelar'
  }).then((result) => {
    if (result.isConfirmed) {
      fetch(`https://localhost:7258/api/usuarios/${id}`, {
        method: "DELETE",
        headers: { Authorization: `Bearer ${token}` }
      })
      .then(res => {
        if (!res.ok) throw new Error("Error al eliminar usuario");
        Swal.fire('Eliminado', 'El usuario ha sido eliminado.', 'success');
        cargarUsuarios();
      })
      .catch(err => Swal.fire("Error", "No se pudo eliminar el usuario", "error"));
    }
  })

    fetch(`https://localhost:7258/api/usuarios/${id}`, {
      method: "DELETE",
      headers: { Authorization: `Bearer ${token}` }
    })
      .then(res => {
        if (!res.ok) throw new Error("Error al eliminar usuario");
        cargarUsuarios();
      })
      .catch(err => console.error("Error al eliminar:", err));
  }

  // Cargar usuarios al inicio
  cargarUsuarios();
});

