<script setup>
import { ref, computed, onMounted } from 'vue'
import Dialog from 'primevue/dialog'
import ConfirmDialog from 'primevue/confirmdialog'
import InputText from 'primevue/inputtext'
import Dropdown from 'primevue/dropdown'
import Button from 'primevue/button'
import { useConfirm } from 'primevue/useconfirm'
import api from '@/utilities/api.js'
import { push } from 'notivue'
import DialogSalas from '../Salas/DialogSalas.vue'


const props = defineProps(['visible'])
const emit = defineEmits(['update:visible', 'close'])
const mostrarDialogoSalas = ref(false)
const tribunalSeleccionado = ref(null)
const errores = ref({})

const dialogVisible = computed({
  get: () => props.visible,
  set: (val) => emit('update:visible', val)
})

const mostrarFormulario = ref(false)
const tribunalEditando = ref({})
const confirm = useConfirm()

const tribunales = ref([])
const search = ref('')
const page = ref(1)
const perPage = 10

const estadoOptions = [
  { label: 'Activo', value: true },
  { label: 'Inactivo', value: false }
]

const camposFormulario = [
  { label: 'Nombre', model: 'nombre_Tribunal' },
  { label: 'Descripción', model: 'descripcion' },
  { label: 'Teléfono', model: 'telefono' },
  { label: 'Dirección', model: 'direccion' },
  { label: 'Provincia', model: 'distrito' }
]

function abrirDialogoSalas(tribunal) {

  console.log(tribunal.nombre_Tribunal)
  tribunalSeleccionado.value = tribunal
  mostrarDialogoSalas.value = true
}

onMounted(() => cargarTribunales())

async function cargarTribunales() {
  try {
    const { data } = await api.get('/api/Tribunales/Tribunales')
    tribunales.value = data
  } catch (err) {
    push.error('No se pudieron cargar los tribunales')
  }
}

function abrirFormularioNuevo() {
  errores.value = {}
  tribunalEditando.value = {
    nombre_Tribunal: '', descripcion: '', telefono: '', direccion: '', distrito: '', estatus: true
  }
  mostrarFormulario.value = true
}

function abrirFormularioEditar(tribunal) {
  tribunalEditando.value = { ...tribunal }
  mostrarFormulario.value = true
}

function cerrarFormulario() {
  mostrarFormulario.value = false
}

async function guardarTribunal() {
  errores.value = {} // limpiar antes de validar
  const body = tribunalEditando.value

  // Validaciones manuales
  if (!body.nombre_Tribunal?.trim()) errores.value.nombre_Tribunal = 'Este campo es obligatorio'
  if (!body.descripcion?.trim()) errores.value.descripcion = 'Este campo es obligatorio'
  if (!body.telefono?.trim()) errores.value.telefono = 'Este campo es obligatorio'
  if (!body.direccion?.trim()) errores.value.direccion = 'Este campo es obligatorio'
  if (!body.distrito?.trim()) errores.value.distrito = 'Este campo es obligatorio'

  // Si hay errores, detener el guardado
  if (Object.keys(errores.value).length) return

  try {
    if (body.id_Tribunal) {
      await api.put(`/api/Tribunales/ActualizarTribunal/${body.id_Tribunal}`, body)
    } else {
      await api.post('/api/Tribunales/CrearTribunal', body)
    }

    cerrarFormulario()
    await cargarTribunales()
  } catch (err) {
    // Puedes mostrar un error general aquí si quieres
  }
}


function confirmarEliminacion(id) {
  console.log(id);
  confirm.require({
    message: '¿Seguro que deseas eliminar este tribunal?',
    header: 'Eliminar Tribunal',
    icon: 'pi pi-exclamation-triangle',
    acceptLabel: 'Sí',
    rejectLabel: 'Cancelar',
    acceptClass: 'p-button-danger',
    accept: async () => {
      try {
        await api.delete(`/api/Tribunales/EliminarTribunal/${id}`)
        await cargarTribunales()
        push.success('Tribunal eliminado correctamente')
      } catch (err) {
        push.error('No se pudo eliminar el tribunal, tiene casos agregados')
      }
    }
  })
}

const filteredTribunales = computed(() => {
  if (!search.value) return tribunales.value
  const term = search.value.toLowerCase()
  return tribunales.value.filter(t =>
    Object.values(t).some(val => String(val).toLowerCase().includes(term))
  )
})

const paginatedTribunales = computed(() => {
  const start = (page.value - 1) * perPage
  return filteredTribunales.value.slice(start, start + perPage)
})

function validarCampo(model) {
  const valor = tribunalEditando.value[model]
  if (!valor || !valor.trim()) {
    errores.value[model] = 'Este campo es obligatorio'
  } else {
    delete errores.value[model]
  }
}


</script>

<template>
  <Dialog v-model:visible="dialogVisible" modal class="dialog-tribunales" :closable="false" :draggable="false">
    <template #header>
      <div class="flex justify-content-between align-items-center m-2 flex-wrap gap-2 pt-3 w-100">
        <!-- Columna izquierda: solo el título -->
        <div class="flex-grow">
          <h2 class="text-2xl font-bold m-0">Mantenimiento de Tribunales</h2>
        </div>

        <!-- Columna derecha: botones + buscador -->
        <div class="flex items-center gap-2 filtro-busqueda-bar">
          <button class="btn-filtro" @click="abrirFormularioNuevo">
            <i class="fas fa-plus me-2"></i> Nuevo Tribunal
          </button>

          <button class="btn-filtro" @click="dialogVisible = false">
            <i class="pi pi-times me-2"></i> Cerrar
          </button>

          <div class="search-container">
            <span class="p-input-icon-left search-input-wrapper">
              <i class="pi pi-search search-icon" />
              <input type="text" class="search-input" placeholder="Buscar..." v-model="search" />
            </span>
          </div>
        </div>
      </div>
    </template>




    <div v-for="tribunal in paginatedTribunales" :key="tribunal.id_Tribunal" class="tribunal-card">
      <div class="tribunal-info">
        <div><strong>{{ tribunal.nombre_Tribunal }}</strong></div>
        <div class="small text-muted">{{ tribunal.descripcion }} - {{ tribunal.telefono }} - {{ tribunal.direccion }}
        </div>
        <div class="small">Provincia: {{ tribunal.distrito }}</div>
      </div>
      <div class="d-flex align-items-center gap-2">
        <span :class="['badge', tribunal.estatus ? 'bg-success' : 'bg-danger']">
          {{ tribunal.estatus ? 'Activo' : 'Inactivo' }}
        </span>
        <Button icon="pi pi-pencil" class="p-button-text p-button-sm text-dark" @click="abrirFormularioEditar(tribunal)"
          v-tooltip="'Modificar tribunal'" />
        <Button icon="pi pi-trash" class="p-button-text p-button-sm text-danger"
          @click="confirmarEliminacion(tribunal.id_Tribunal)" v-tooltip="'Eliminar tribunal'" />
        <Button icon="pi pi-eye white-icon" class="p-button-text p-button-sm text-danger"
          @click="abrirDialogoSalas(tribunal)" v-tooltip="'Ver salas tribunal'" />
      </div>
    </div>

    <div v-if="filteredTribunales.length === 0" class="text-center text-muted mt-2">No hay tribunales registrados.</div>
  </Dialog>

  <Dialog v-model:visible="mostrarFormulario" modal class="dialog-formulario-form" :closable="false">

    <!-- Header personalizado -->
    <template #header>
      <div class="flex justify-content-center align-items-center w-full">
        <h2 class="text-2xl font-bold m-0">Formulario Tribunal</h2>
      </div>
    </template>


    <!-- Contenido del formulario -->


    <div class="p-fluid formgrid grid mt-2">
      <!-- <div class="field col-12 md:col-6" v-for="campo in camposFormulario" :key="campo.label">
        <label>{{ campo.label }} <span class="text-red-500">*</span></label>
        <InputText v-model="tribunalEditando[campo.model]"/>
      </div> -->

      <div class="field col-12 md:col-6" v-for="campo in camposFormulario" :key="campo.model">
        <label>{{ campo.label }} <span class="text-red-500">*</span></label>

        <InputText v-model="tribunalEditando[campo.model]" @input="validarCampo(campo.model)"
          :class="{ 'input-error': errores[campo.model] }" />

        <small class="text-red-500" v-if="errores[campo.model]">
          {{ errores[campo.model] }}
        </small>
      </div>


      <div class="field col-12 md:col-6">
        <label>Estado</label>
        <Dropdown v-model="tribunalEditando.estatus" :options="estadoOptions" optionLabel="label" optionValue="value"
          class="w-full" />
      </div>
    </div>

    <template #footer>
      <div class="flex gap-2 w-full justify-content-center mt-4">
        <Button label="Cancelar" class="btn-filtro" @click="cerrarFormulario" />
        <Button label="Guardar" class="btn-filtro" style="background-color: #003870; color: white"
          @click="guardarTribunal" />
      </div>
    </template>

  </Dialog>





  <DialogSalas v-if="tribunalSeleccionado" v-model:visible="mostrarDialogoSalas"
    :tribunal-id="tribunalSeleccionado.id_Tribunal" :nombre-tribunal="tribunalSeleccionado.nombre_Tribunal" />

  <ConfirmDialog />
</template>

<style scoped>
.dialog-tribunales {
  width: 38vw;
}

.tribunal-card {
  border: 1px solid #ddd;
  border-radius: 8px;
  padding: 0.75rem 1rem;
  margin-bottom: 0.5rem;
  background-color: #fff;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.05);
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.tribunal-info {
  display: flex;
  flex-direction: column;
}

.p-dropdown.p-focus {
  border-color: #003870 !important;
  box-shadow: 0 0 0 0.2rem rgba(0, 56, 112, 0.25);
}

.btn-filtro {
  padding: 0.35rem 1.2rem;
  height: 38px;
  border-radius: 6px;
  font-size: 0.88rem;
  font-weight: 500;
  background-color: #003870;
  border-color: #003870;
  color: #f0f0f0;
  transition: all 0.2s ease;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  line-height: 1;
  white-space: nowrap;
  text-decoration: none;
  border: 1px solid transparent;
}

.btn-filtro:hover,
.btn-filtro.active {
  background-color: #c00606 !important;
  border-color: #c00606 !important;
  box-shadow: 0 6px 16px rgba(121, 1, 51, 0.3) !important;
  transform: translateY(-1px) !important;
  transition: background-color 0.2s !important;
}

.search-container {
  display: flex;
  align-items: center;
  height: 38px;
}

.search-input-wrapper {
  position: relative;
}

.search-input-wrapper .search-icon {
  position: absolute;
  left: 10px;
  top: 50%;
  transform: translateY(-50%);
  color: #6c757d;
}

.search-input {
  padding-left: 2rem;
  padding-right: 0.75rem;
  height: 38px;
  border-radius: 6px;
  border: 1px solid #ced4da;
  font-size: 0.88rem;
  background-color: #fff;
  color: #212529;
}

.input-error {
  border: 1px solid #dc2626 !important; /* rojo */
  box-shadow: 0 0 0 0.15rem rgba(220, 38, 38, 0.25); /* opcional */
}
</style>
