const API_BASE_URL = 'https://localhost:7258/api/';

const desafioForm = document.getElementById('desafioForm');
const modalTitle = document.getElementById('modalTitle');
const desafioIdInput = document.getElementById('desafioId');
const modal = new bootstrap.Modal(document.getElementById('desafioModal'));

async function fetchDesafios() {
    try {
        const response = await fetch(`${API_BASE_URL}Desafio`);
        if (!response.ok) throw new Error('Error al cargar desafíos');
        const desafios = await response.json();
        const tbody = document.getElementById('desafios-table-body');
        tbody.innerHTML = '';
        desafios.forEach(desafio => {
            const row = tbody.insertRow();
            row.innerHTML = `
                <td>${desafio.idDesafio}</td>
                <td>${desafio.titulo}</td>
                <td>${desafio.descripcion}</td>
                <td>${desafio.meta}</td>
                <td>
                    <button class="btn btn-warning btn-sm me-2" onclick="editDesafio(${desafio.idDesafio})">Editar</button>
                    <button class="btn btn-danger btn-sm" onclick="deleteDesafio(${desafio.idDesafio})">Eliminar</button>
                </td>
            `;
        });
    } catch (error) {
        console.error(error);
        alert('Error: No se pudieron cargar los desafíos. Revise la consola para más detalles.');
    }
}

async function saveDesafio(desafioData) {
    try {
        const method = desafioData.idDesafio ? 'PUT' : 'POST';
        const url = desafioData.idDesafio ? `${API_BASE_URL}Desafio/${desafioData.idDesafio}` : `${API_BASE_URL}Desafio`;
        
        const response = await fetch(url, {
            method,
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(desafioData)
        });
        if (!response.ok) throw new Error('Error al guardar el desafío');
        alert('Desafío guardado con éxito!');
        modal.hide();
        fetchDesafios();
    } catch (error) {
        console.error(error);
        alert('Error: No se pudo guardar el desafío.');
    }
}

async function deleteDesafio(id) {
    if (!confirm('¿Seguro que desea eliminar este desafío?')) return;
    try {
        const response = await fetch(`${API_BASE_URL}Desafio/${id}`, { method: 'DELETE' });
        if (!response.ok) throw new Error('Error al eliminar el desafío');
        alert('Desafío eliminado.');
        fetchDesafios();
    } catch (error) {
        console.error(error);
        alert('Error: No se pudo eliminar el desafío.');
    }
}

async function editDesafio(id) {
    try {
        const response = await fetch(`${API_BASE_URL}Desafio/${id}`);
        if (!response.ok) throw new Error('Desafío no encontrado');
        const desafio = await response.json();
        
        modalTitle.textContent = 'Editar Desafío';
        desafioIdInput.value = desafio.idDesafio;
        document.getElementById('titulo').value = desafio.titulo;
        document.getElementById('descripcion').value = desafio.descripcion;
        document.getElementById('meta').value = desafio.meta;
        
        modal.show();
    } catch (error) {
        console.error(error);
        alert('Error: No se pudo cargar el desafío para edición.');
    }
}

desafioForm.addEventListener('submit', (e) => {
    e.preventDefault();
    const desafioData = {
        idDesafio: desafioIdInput.value ? parseInt(desafioIdInput.value) : 0,
        titulo: document.getElementById('titulo').value,
        descripcion: document.getElementById('descripcion').value,
        meta: document.getElementById('meta').value,
        // Agrega otros campos si son necesarios para POST/PUT
    };
    saveDesafio(desafioData);
});

fetchDesafios();