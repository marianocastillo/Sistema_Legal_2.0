<template>
  <Dialog v-model:visible="dialogVisibleSala" modal :header="`Salas del Tribunal: ${props.nombreTribunal}`"
    class="dialog-salas" :style="{ width: '50%', height: '85vh' }">

    <div class="card p-4">
      <div class="flex justify-between items-center mb-4 flex-wrap gap-2">
        <h2 class="text-xl font-bold m-0">Salas registradas</h2>

        <div class="flex items-center gap-2 filtro-busqueda-bar">
          <Button label="Cerrar" icon="pi pi-times" class="p-button-sm btn-litigio" @click="cerrarSala" />
          <Button label="Nueva Sala" icon="pi pi-plus" class="p-button-sm btn-litigio"
            @click="abrirFormularioNuevaSala" />
          <div class="search-container">
            <span class="p-input-icon-left search-input-wrapper">
              <i class="pi pi-search search-icon" />
              <input type="text" class="search-input" placeholder="Buscar..." v-model="search" />
            </span>
          </div>
        </div>
      </div>

      <table class="table table-sm table-bordered table-hover">
        <thead class="table-light">
          <tr>
            <th>Sala</th>
            <th class="text-center" :style="{ width: '80px' }">Acciones</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="sala in paginatedSalas" :key="sala.idSala">
            <td>{{ sala.nombre }}</td>
            <td class="text-end pe-2 ps-2">
              <div class="btn-group">
                <button class="btn btn-sm btn-hover me-1" style="background-color: #003870;" @click="editarSala(sala)">
                  <i class="pi pi-pencil white-icon"></i>
                </button>
                <button class="btn btn-sm btn-hover" style="background-color: #003870;"
                  @click="eliminarSala(sala.idSala)">
                  <i class="pi pi-trash white-icon"></i>
                </button>
              </div>
            </td>
          </tr>
          <tr v-if="filteredSalas.length === 0">
            <td colspan="2" class="text-center text-muted">No hay salas registradas.</td>
          </tr>
        </tbody>
      </table>
      <!-- Paginación -->
      <nav class="mt-4">
        <ul class="pagination justify-content-end">
          <li class="page-item" :class="{ disabled: page === 1 }">
            <button class="page-link" @click="page--">Anterior</button>
          </li>
          <li class="page-item" v-for="p in totalPages" :key="p" :class="{ active: page === p }">
            <button class="page-link" @click="page = p">{{ p }}</button>
          </li>
          <li class="page-item" :class="{ disabled: page === totalPages }">
            <button class="page-link" @click="page++">Siguiente</button>
          </li>
        </ul>
      </nav>

    </div>

    <!-- Formulario de Sala -->
    <Dialog v-model:visible="formVisible" modal header="Formulario de Sala" class="dialog-form w-96">
      <div class="p-fluid p-4">
        <div class="field">
          <label>Nombre de la Sala</label>
          <InputText v-model="form.nombre" placeholder="Ingrese nombre" class="w-full" />
        </div>
        <div class="text-end mt-4">
          <Button label="Cancelar" class="btn-litigio me-2" @click="formVisible = false" />
          <Button label="Guardar" class="btn-litigio" @click="guardarSala" />
        </div>
      </div>
    </Dialog>
  </Dialog>


  <ListadoTribunales v-model:visible="mostrarDialogoTribunales" />
</template>


<script setup>
import { ref, watch, computed, onMounted } from 'vue'
import Dialog from 'primevue/dialog'
import Button from 'primevue/button'
import InputText from 'primevue/inputtext'
import api from '@/utilities/api'
import { push } from 'notivue'
import ListadoTribunales from '../Tribunales/ListadoTribunales.vue'

const mostrarDialogoTribunales = ref(false)
const page = ref(1)
const perPage = 10

const totalPages = computed(() =>
  Math.ceil(filteredSalas.value.length / perPage)
)

//Props
const props = defineProps({
  visible: Boolean,
  tribunalId: Number,
  nombreTribunal: String // 👈 agrega esta línea
})

onMounted(async () => {
  if (dialogVisibleSala.value && props.tribunalId) {
    await cargarSalas()
  }
})
const emit = defineEmits(['update:visible'])

//Computed para manejar el v-model correctamente
const dialogVisibleSala = computed({
  get: () => props.visible,
  set: val => emit('update:visible', val)
})

const formVisible = ref(false)
const salas = ref([])
const search = ref('')
const form = ref({ idSala: 0, nombre: '' })
const editing = ref(false)

const filteredSalas = computed(() =>
  salas.value.filter(s =>
    s.nombre.toLowerCase().includes(search.value.toLowerCase())
  )
)


const paginatedSalas = computed(() => {
  const start = (page.value - 1) * perPage
  return filteredSalas.value.slice(start, start + perPage)
})

watch(() => props.visible, async (val) => {
  if (val && props.tribunalId) {
    await cargarSalas()
  }
})

async function cargarSalas() {
  try {
    const res = await api.get(`/api/Tribunales/${props.tribunalId}/Salas`)
    console.log('Respuesta de la API:', res.data)

    // Verifica si res.data tiene una propiedad "data"
    if (Array.isArray(res.data)) {
      salas.value = res.data
    } else if (Array.isArray(res.data.data)) {
      salas.value = res.data.data
    } else {
      salas.value = []
    }

  } catch (err) {
    push.error('Error al cargar salas')
    salas.value = []
  }
}

function cerrarSala(){
  dialogVisibleSala.value = false
  mostrarDialogoTribunales.value = true
}
function abrirFormularioNuevaSala() {
  form.value = { idSala: 0, nombre: '' }
  editing.value = false
  formVisible.value = true
}

function editarSala(sala) {
  form.value = { ...sala }
  editing.value = true
  formVisible.value = true
}

async function guardarSala() {
  try {
    const payload = {
      idSala: form.value.idSala,
      nombre: form.value.nombre,
      idTribunal: props.tribunalId
    }
    const url = editing.value
      ? `/api/Tribunales/Actualizarsalas/${form.value.idSala}`
      : '/api/Tribunales/CrearSalas'
    const metodo = editing.value ? 'put' : 'post'

    await api[metodo](url, payload)
    push.success('Sala guardada correctamente')
    formVisible.value = false
    await cargarSalas()
  } catch (err) {
    push.error('Error al guardar sala')
  }
}

async function eliminarSala(id) {
  if (!confirm('¿Deseas eliminar esta sala?')) return
  try {
    await api.delete(`/api/Tribunales/EliminarSalas/${id}`)
    push.success('Sala eliminada')
    await cargarSalas()
  } catch (err) {
    push.error('Error al eliminar sala')
  }
}
</script>

<style scoped>
.white-icon {
  color: white !important;
}

.btn-group .btn {
  margin: 0 2px;
}

thead th {
  background-color: rgb(241, 242, 250);
  border: none !important;
  font-weight: 600;
  color: #2e3842;
  font-size: 0.95rem;
}

tbody td {
  border-right: 1px solid #ebebeb;
  border-top: none !important;
  border-bottom: none !important;
  border-left: none !important;
}

/* === Filtro de búsqueda + botones === */
.filtro-busqueda-bar {
  display: flex;
  align-items: center;
  gap: 1rem;
  flex-wrap: nowrap;
  margin-left: auto;
}

.search-container {
  max-width: 250px;
  width: 100%;
  position: relative;
}

.search-input-wrapper {
  position: relative;
  display: block;
}

.search-input {
  width: 100%;
  padding: 0.5rem 2.5rem 0.5rem 2rem;
  font-size: 14px;
  border: 1px solid #ccc;
  border-radius: 6px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
  transition: border-color 0.3s, box-shadow 0.3s;
  background-color: #fff;
  color: #000;
}

.search-input:focus {
  outline: none;
  border-color: #003870;
  box-shadow: 0 0 5px rgba(0, 56, 112, 0.3);
}

.search-icon {
  position: absolute;
  left: 10px;
  top: 50%;
  transform: translateY(-50%);
  color: #888;
  pointer-events: none;
  font-size: 1rem;
}

.pagination .page-link {
  color: #003870;
  border: 1px solid #ccc;
  padding: 0.3rem 0.75rem;
  font-size: 0.875rem;
}

.pagination .page-item.active .page-link {
  background-color: #003870;
  border-color: #003870;
  color: white;
}

.pagination .page-item.disabled .page-link {
  color: #aaa;
  pointer-events: none;
}


@media (max-width: 768px) {
  .filtro-busqueda-bar {
    flex-direction: column;
    align-items: flex-end;
    width: 100%;
    margin-left: 0;
    gap: 0.5rem;
  }

  .search-container {
    width: 100%;
  }
}
</style>
