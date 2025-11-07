// ** CONFIGURACIÓN **
// Cambia a HTTPS si tu API backend usa HTTPS
const URL_BASE = "https://localhost:7135";
// Si ves errores de certificado SSL, primero abre https://localhost:7135 en el navegador y acepta el certificado

// ** VARIABLES GLOBALES **
let carreras = [];
let tiposEstudiante = [];
let paginaActual = 1;
const pageSize = 10;

// ** UTILIDADES **
function mostrarMensaje(element, mensaje, tipo = 'error') {
    element.textContent = mensaje;
    element.style.display = 'block';
    
    if (tipo === 'success') {
        element.style.backgroundColor = '#d4edda';
        element.style.borderColor = '#c3e6cb';
        element.style.color = '#155724';
    } else {
        element.style.backgroundColor = '#f8d7da';
        element.style.borderColor = '#f5c6cb';
        element.style.color = '#721c24';
    }
}

function ocultarMensaje(element) {
    element.style.display = 'none';
}

// ** API SERVICES **

// Estudiantes API
const EstudiantesAPI = {
    async getAll(filtros = {}) {
        const params = new URLSearchParams();
        
        Object.entries(filtros).forEach(([key, value]) => {
            if (value !== null && value !== undefined && value !== '') {
                params.append(key, value);
            }
        });

        const response = await fetch(`${URL_BASE}/api/estudiantes/get-all?${params}`);
        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
        return await response.json();
    },

    async getById(id) {
        const response = await fetch(`${URL_BASE}/api/estudiantes/${id}`);
        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
        return await response.json();
    },

    async create(estudiante) {
        console.log('📤 Enviando estudiante:', JSON.stringify(estudiante, null, 2));
        
        const response = await fetch(`${URL_BASE}/api/estudiantes`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(estudiante)
        });
        
        console.log('📥 Response status:', response.status);
        
        if (!response.ok) {
            const errorText = await response.text();
            console.error('❌ Error response:', errorText);
            throw new Error(`HTTP error! status: ${response.status} - ${errorText}`);
        }
        
        const result = await response.json();
        console.log('✅ Response data:', result);
        return result;
    },

    async update(id, estudiante) {
        const response = await fetch(`${URL_BASE}/api/estudiantes/${id}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(estudiante)
        });
        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
        return await response.json();
    },

    async delete(id) {
        const response = await fetch(`${URL_BASE}/api/estudiantes/${id}`, {
            method: 'DELETE'
        });
        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
        return await response.json();
    },

    async validarMatricula(matricula, idEstudianteExcluir = null) {
        const params = new URLSearchParams();
        if (idEstudianteExcluir) params.append('idEstudianteExcluir', idEstudianteExcluir);
        
        const response = await fetch(`${URL_BASE}/api/estudiantes/validar-matricula/${matricula}?${params}`);
        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
        return await response.json();
    },

    async getTipoInfo(idTipoEstudiante) {
        const response = await fetch(`${URL_BASE}/api/estudiantes/tipo-info/${idTipoEstudiante}`);
        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
        return await response.json();
    }
};

// Carreras API
const CarrerasAPI = {
    async getAll() {
        const response = await fetch(`${URL_BASE}/api/carreras/get-all`);
        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
        return await response.json();
    },

    async getById(id) {
        const response = await fetch(`${URL_BASE}/api/carreras/${id}`);
        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
        return await response.json();
    },

    async create(carrera) {
        const response = await fetch(`${URL_BASE}/api/carreras`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(carrera)
        });
        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
        return await response.json();
    },

    async update(id, carrera) {
        const response = await fetch(`${URL_BASE}/api/carreras/${id}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(carrera)
        });
        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
        return await response.json();
    },

    async delete(id) {
        const response = await fetch(`${URL_BASE}/api/carreras/${id}`, {
            method: 'DELETE'
        });
        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
        return await response.json();
    }
};

// Tipos de Estudiante API
const TiposEstudianteAPI = {
    async getAll() {
        const response = await fetch(`${URL_BASE}/api/tipos-estudiante/get-all`);
        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
        return await response.json();
    },

    async getById(id) {
        const response = await fetch(`${URL_BASE}/api/tipos-estudiante/${id}`);
        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
        return await response.json();
    },

    async create(tipo) {
        const response = await fetch(`${URL_BASE}/api/tipos-estudiante`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(tipo)
        });
        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
        return await response.json();
    },

    async update(id, tipo) {
        const response = await fetch(`${URL_BASE}/api/tipos-estudiante/${id}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(tipo)
        });
        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
        return await response.json();
    },

    async delete(id) {
        const response = await fetch(`${URL_BASE}/api/tipos-estudiante/${id}`, {
            method: 'DELETE'
        });
        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
        return await response.json();
    }
};

// ** INICIALIZACIÓN **
async function inicializar() {
    console.log('🚀 Inicializando aplicación...');
    console.log('🔗 URL Base API:', URL_BASE);
    
    // Intentar cargar carreras y tipos, pero no bloquear si fallan
    await Promise.allSettled([
        cargarCarreras(),
        cargarTiposEstudiante()
    ]);
    
    // Siempre intentar cargar estudiantes
    console.log('📋 Cargando estudiantes...');
    await cargarEstudiantes();
    console.log('✅ Inicialización completada');
}

async function cargarCarreras() {
    try {
        console.log('📚 Cargando carreras...');
        const response = await CarrerasAPI.getAll();
        
        let carrerasData = [];
        
        // Manejar diferentes formatos de respuesta
        if (response && response.success && response.data) {
            carrerasData = Array.isArray(response.data) ? response.data : [];
        } else if (Array.isArray(response)) {
            carrerasData = response;
        }
        
        carreras = carrerasData;
        
        // Llenar el select de carreras del formulario
        const selectCarrera = document.getElementById('carrera');
        if (selectCarrera) {
            selectCarrera.innerHTML = '<option value="">Seleccione una carrera...</option>';
            
            carreras.forEach(carrera => {
                const option = document.createElement('option');
                option.value = carrera.idCarrera;
                option.textContent = carrera.nombreCarrera;
                selectCarrera.appendChild(option);
            });
        }
        
        // Llenar el select de filtro de carreras
        const selectFiltroCarrera = document.getElementById('filtro-carrera-select');
        if (selectFiltroCarrera) {
            selectFiltroCarrera.innerHTML = '<option value="">Todas las carreras</option>';
            
            carreras.forEach(carrera => {
                const option = document.createElement('option');
                option.value = carrera.idCarrera;
                option.textContent = carrera.nombreCarrera;
                selectFiltroCarrera.appendChild(option);
            });
        }
        
        console.log('✅ Carreras cargadas:', carreras.length);
    } catch (error) {
        console.warn('⚠️ No se pudieron cargar las carreras (la funcionalidad de carreras estará limitada):', error.message);
        carreras = [];
    }
}

async function cargarTiposEstudiante() {
    try {
        console.log('👥 Cargando tipos de estudiante...');
        const response = await TiposEstudianteAPI.getAll();
        
        let tiposData = [];
        
        // Manejar diferentes formatos de respuesta
        if (response && response.success && response.data) {
            tiposData = Array.isArray(response.data) ? response.data : [];
        } else if (Array.isArray(response)) {
            tiposData = response;
        }
        
        tiposEstudiante = tiposData;
        
        // Llenar el select de tipos de estudiante del formulario
        const selectTipo = document.getElementById('tipoEstudiante');
        if (selectTipo) {
            selectTipo.innerHTML = '<option value="">Seleccione tipo de estudiante...</option>';
            
            tiposEstudiante.forEach(tipo => {
                const option = document.createElement('option');
                option.value = tipo.idTipoEstudiante;
                option.textContent = tipo.nombreTipo;
                selectTipo.appendChild(option);
            });
        }
        
        // Llenar el select de filtro de tipos
        const selectFiltroTipo = document.getElementById('filtro-tipo');
        if (selectFiltroTipo) {
            selectFiltroTipo.innerHTML = '<option value="">Todos los tipos</option>';
            
            tiposEstudiante.forEach(tipo => {
                const option = document.createElement('option');
                option.value = tipo.idTipoEstudiante;
                option.textContent = tipo.nombreTipo;
                selectFiltroTipo.appendChild(option);
            });
        }
        
        console.log('✅ Tipos de estudiante cargados:', tiposEstudiante.length);
    } catch (error) {
        console.warn('⚠️ No se pudieron cargar los tipos de estudiante (la funcionalidad estará limitada):', error.message);
        tiposEstudiante = [];
    }
}

// ** REGISTRO DE ESTUDIANTE **
document.getElementById('student-form').addEventListener('submit', async function(event) {
    event.preventDefault();
    
    const matricula = document.getElementById('matricula')?.value || generateMatricula();
    const nombreCompleto = document.getElementById('nombre').value;
    const idCarrera = parseInt(document.getElementById('carrera').value);
    const semestre = parseInt(document.getElementById('semestre').value);
    const promedio = parseFloat(document.getElementById('promedio').value);
    const idTipoEstudiante = parseInt(document.getElementById('tipoEstudiante').value);

    const estudianteData = {
        matricula: matricula,
        nombreCompleto: nombreCompleto,
        semestre: semestre,
        promedio: promedio,
        idCarrera: idCarrera,
        idTipoEstudiante: idTipoEstudiante
    };

    const msgElement = document.getElementById('mensaje-servidor');
    ocultarMensaje(msgElement);

    try {
        // Validación de campos
        if (!estudianteData.nombreCompleto || !estudianteData.matricula) {
            throw new Error("El nombre y la matrícula son obligatorios.");
        }
        
        if (isNaN(estudianteData.idCarrera) || estudianteData.idCarrera === 0) {
            throw new Error("Debe seleccionar una carrera.");
        }
        
        if (isNaN(estudianteData.idTipoEstudiante) || estudianteData.idTipoEstudiante === 0) {
            throw new Error("Debe seleccionar un tipo de estudiante.");
        }
        
        if (isNaN(estudianteData.semestre) || isNaN(estudianteData.promedio)) {
            throw new Error("Todos los campos son obligatorios.");
        }

        console.log('📋 Datos del estudiante a enviar:', estudianteData);

        // Validar matrícula única (comentado temporalmente para debugging)
        // const validacionMatricula = await EstudiantesAPI.validarMatricula(estudianteData.matricula);
        // if (validacionMatricula.success && validacionMatricula.data === false) {
        //     throw new Error("La matrícula ya existe. Por favor, use otra matrícula.");
        // }

        const response = await EstudiantesAPI.create(estudianteData);

        if (response.success) {
            mostrarMensaje(msgElement, 
                `Registro exitoso. Matrícula: ${response.data.matricula}`, 'success');
            document.getElementById('student-form').reset();
            await cargarEstudiantes();
        } else {
            mostrarMensaje(msgElement, 
                `Error al registrar: ${response.message || 'Error desconocido'}`);
        }
    } catch (error) {
        mostrarMensaje(msgElement, `Error: ${error.message}`);
    }
});

// ** LISTADO Y FILTRADO DE ESTUDIANTES **
async function cargarEstudiantes(filtros = {}) {
    const tbody = document.querySelector('#estudiantes-table tbody');
    tbody.innerHTML = '<tr><td colspan="7">Cargando...</td></tr>';
    
    try {
        const filtrosCompletos = {
            ...filtros,
            page: paginaActual,
            pageSize: pageSize
        };

        const response = await EstudiantesAPI.getAll(filtrosCompletos);
        
        console.log('📦 Respuesta completa de la API:', response);

        tbody.innerHTML = ''; // Limpiar la tabla

        // Manejar diferentes formatos de respuesta
        let estudiantes = [];
        
        if (response.success && response.data) {
            // Formato: ApiResponse<PaginatedResponse<EstudianteDto>>
            if (response.data.data && Array.isArray(response.data.data)) {
                estudiantes = response.data.data;
                actualizarPaginacion(response.data);
            }
            // Formato: ApiResponse<List<EstudianteDto>>
            else if (Array.isArray(response.data)) {
                estudiantes = response.data;
            }
        }
        // Formato directo: Array de estudiantes (sin ApiResponse wrapper)
        else if (Array.isArray(response)) {
            estudiantes = response;
        }

        console.log('👥 Estudiantes a mostrar:', estudiantes.length);

        if (estudiantes.length === 0) {
            tbody.innerHTML = '<tr><td colspan="7">No se encontraron estudiantes.</td></tr>';
            return;
        }
        
        estudiantes.forEach(estudiante => {
            const row = tbody.insertRow();
            row.insertCell().textContent = estudiante.idEstudiante;
            row.insertCell().textContent = estudiante.matricula;
            row.insertCell().textContent = estudiante.nombreCompleto || estudiante.nombre;
            row.insertCell().textContent = estudiante.nombreCarrera || estudiante.carrera || 'N/A';
            row.insertCell().textContent = estudiante.semestre;
            row.insertCell().textContent = estudiante.promedio;
            
            // Botones de acción
            const actionsCell = row.insertCell();
            
            const editBtn = document.createElement('button');
            editBtn.textContent = 'Editar';
            editBtn.classList.add('btn-editar');
            editBtn.onclick = () => editarEstudiante(estudiante.idEstudiante);
            
            const deleteBtn = document.createElement('button');
            deleteBtn.textContent = 'Eliminar';
            deleteBtn.classList.add('btn-eliminar');
            deleteBtn.onclick = () => eliminarEstudiante(estudiante.idEstudiante);
            
            actionsCell.appendChild(editBtn);
            actionsCell.appendChild(deleteBtn);
        });

    } catch (error) {
        tbody.innerHTML = `<tr><td colspan="7">Error al cargar datos: ${error.message}</td></tr>`;
        console.error("❌ Error al cargar estudiantes:", error);
    }
}

function actualizarPaginacion(paginatedData) {
    // Esta función se puede implementar para mostrar controles de paginación
    console.log(`Página ${paginatedData.page} de ${paginatedData.totalPages}`);
    console.log(`Total de registros: ${paginatedData.totalRecords}`);
}

// ** FILTRADO **
function filtrarEstudiantes() {
    const filtroNombre = document.getElementById('filtro-nombre').value.trim();
    const filtroMatricula = document.getElementById('filtro-matricula').value.trim();
    const filtroCarrera = document.getElementById('filtro-carrera-select').value;
    const filtroSemestre = document.getElementById('filtro-semestre').value;
    const filtroTipo = document.getElementById('filtro-tipo').value;
    
    const filtros = {};
    
    if (filtroNombre) {
        filtros.nombreCompleto = filtroNombre;
    }
    
    if (filtroMatricula) {
        filtros.matricula = filtroMatricula;
    }
    
    if (filtroCarrera) {
        filtros.idCarrera = parseInt(filtroCarrera);
    }
    
    if (filtroSemestre) {
        filtros.semestre = parseInt(filtroSemestre);
    }
    
    if (filtroTipo) {
        filtros.idTipoEstudiante = parseInt(filtroTipo);
    }
    
    paginaActual = 1; // Resetear a primera página
    cargarEstudiantes(filtros);
}

function limpiarFiltros() {
    document.getElementById('filtro-nombre').value = '';
    document.getElementById('filtro-matricula').value = '';
    document.getElementById('filtro-carrera-select').value = '';
    document.getElementById('filtro-semestre').value = '';
    document.getElementById('filtro-tipo').value = '';
    
    paginaActual = 1;
    cargarEstudiantes();
}

// ** EDICIÓN DE ESTUDIANTE **
async function editarEstudiante(idEstudiante) {
    try {
        const response = await EstudiantesAPI.getById(idEstudiante);
        
        if (!response.success) {
            alert('Error al obtener datos del estudiante');
            return;
        }

        const estudiante = response.data;
        
        // Pedir nuevos datos (aquí puedes implementar un modal más sofisticado)
        const nuevoNombre = prompt('Nuevo nombre completo:', estudiante.nombreCompleto);
        if (nuevoNombre === null) return; // Cancelado
        
        const nuevaMatricula = prompt('Nueva matrícula:', estudiante.matricula);
        if (nuevaMatricula === null) return;
        
        const nuevoSemestre = prompt('Nuevo semestre (1-10):', estudiante.semestre);
        if (nuevoSemestre === null) return;
        
        const nuevoPromedio = prompt('Nuevo promedio (0-10):', estudiante.promedio);
        if (nuevoPromedio === null) return;
        
        // Mostrar opciones de carrera
        let opcionesCarrera = 'Carreras disponibles:\n';
        carreras.forEach(c => {
            opcionesCarrera += `${c.idCarrera}: ${c.nombreCarrera}\n`;
        });
        const nuevaCarrera = prompt(opcionesCarrera + '\nIngrese el ID de la carrera:', estudiante.idCarrera);
        if (nuevaCarrera === null) return;
        
        // Mostrar opciones de tipo
        let opcionesTipo = 'Tipos de estudiante disponibles:\n';
        tiposEstudiante.forEach(t => {
            opcionesTipo += `${t.idTipoEstudiante}: ${t.nombreTipo}\n`;
        });
        const nuevoTipo = prompt(opcionesTipo + '\nIngrese el ID del tipo:', estudiante.idTipoEstudiante);
        if (nuevoTipo === null) return;

        const estudianteActualizado = {
            matricula: nuevaMatricula,
            nombreCompleto: nuevoNombre,
            semestre: parseInt(nuevoSemestre),
            promedio: parseFloat(nuevoPromedio),
            idCarrera: parseInt(nuevaCarrera),
            idTipoEstudiante: parseInt(nuevoTipo)
        };

        console.log('📝 Actualizando estudiante:', estudianteActualizado);

        const updateResponse = await EstudiantesAPI.update(idEstudiante, estudianteActualizado);
        
        if (updateResponse.success) {
            alert('✅ Estudiante actualizado con éxito');
            await cargarEstudiantes();
        } else {
            alert('❌ Error al actualizar: ' + (updateResponse.message || 'Error desconocido'));
        }

    } catch (error) {
        alert('❌ Error al editar estudiante: ' + error.message);
        console.error('Error al editar estudiante:', error);
    }
}

// ** ELIMINACIÓN DE ESTUDIANTE **
async function eliminarEstudiante(idEstudiante) {
    if (!confirm(`¿Está seguro de que desea eliminar el estudiante con ID ${idEstudiante}?`)) {
        return;
    }

    try {
        const response = await EstudiantesAPI.delete(idEstudiante);

        if (response.success) {
            alert(`Estudiante eliminado con éxito.`);
            await cargarEstudiantes();
        } else {
            alert(`Error al eliminar: ${response.message || 'Error desconocido'}`);
        }
    } catch (error) {
        alert(`Error de conexión al intentar eliminar: ${error.message}`);
        console.error("Error al eliminar estudiante:", error);
    }
}

// ** UTILIDAD PARA GENERAR MATRÍCULA **
function generateMatricula() {
    const prefix = 'A';
    const randomNumber = Math.floor(Math.random() * 100000).toString().padStart(5, '0');
    return prefix + randomNumber;
}

// ** INICIALIZACIÓN AL CARGAR LA PÁGINA **
window.onload = inicializar;