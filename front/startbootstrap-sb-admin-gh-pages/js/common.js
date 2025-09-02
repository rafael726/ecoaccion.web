export function fetchWithToken(url, options = {}) {
    const token = localStorage.getItem("token");
    if (!options.headers) options.headers = {};
    options.headers.Authorization = `Bearer ${token}`;
    return fetch(url, options);
}

// Función para mostrar alertas
export function showAlert(id, message, type = "success") {
    const alert = document.getElementById(id);
    alert.className = `alert alert-${type}`;
    alert.innerText = message;
    alert.classList.remove("d-none");
}
