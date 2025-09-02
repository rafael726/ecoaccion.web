document.addEventListener('DOMContentLoaded', () => {
    const userEmailSpan = document.getElementById('user-email');
    const cerrarSesionLink = document.getElementById('cerrar-sesion-btn'); // El enlace que activa el modal de logout
    const confirmCerrarSesionBtn = document.getElementById('confirm-cerrar-sesion-btn'); // El botón de confirmación en el modal

    const usuarioLogueado = JSON.parse(localStorage.getItem('usuarioLogueado'));

    if (usuarioLogueado && userEmailSpan) {
        userEmailSpan.textContent = usuarioLogueado.correo;
    } else {
        // Si no hay sesión, puedes redirigir al login
        // window.location.href = 'login.html';
    }

    if (confirmCerrarSesionBtn) {
        confirmCerrarSesionBtn.addEventListener('click', (e) => {
            e.preventDefault(); // Evita que el enlace redirija inmediatamente
            localStorage.removeItem('usuarioLogueado');
            window.location.href = 'login.html'; // Redirige después de limpiar la sesión
        });
    }
});

function guardarSesion(usuario) {
    localStorage.setItem('usuarioLogueado', JSON.stringify(usuario));
}