<script setup>
import { ref, computed, onMounted } from 'vue'
import Dialog from 'primevue/dialog'
import ConfirmDialog from 'primevue/confirmdialog'
import InputText from 'primevue/inputtext'
import Dropdown from 'primevue/dropdown'
import Button from 'primevue/button'
import { useConfirm } from 'primevue/useconfirm'
import { push } from 'notivue'
import api from '@/utilities/api.js'

const props = defineProps(['visible'])
const emit = defineEmits(['update:visible', 'close'])

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
  try {
    const body = tribunalEditando.value
    if (!body.nombre_Tribunal?.trim()) return push.warning('El nombre es obligatorio')

    if (body.id_Tribunal) {
      // ✅ Corrección aquí: ID se pasa por la URL
      await api.put(`/api/Tribunales/ActualizarTribunal/${body.id_Tribunal}`, body)
      push.success('Tribunal actualizado correctamente')
    } else {
      await api.post('/api/Tribunales/CrearTribunal', body)
      push.success('Tribunal creado correctamente')
    }

    cerrarFormulario()
    await cargarTribunales()
  } catch (err) {
    push.error('Error al guardar el tribunal')
  }
}

function confirmarEliminacion(id) {
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
        push.error('No se pudo eliminar el tribunal')
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
</script>

<template>
  <Dialog v-model:visible="dialogVisible" modal class="dialog-tribunales" :closable="false" :draggable="false" header="Mantenimiento de Tribunales">
    <div class="d-flex justify-content-between align-items-center mb-3">
      <Button class="text-white" style="background-color: #003870;" @click="abrirFormularioNuevo">
        <i class="fas fa-plus me-2"></i> Nuevo Tribunal
      </Button>
      <input type="text" class="form-control-sm bg-white text-dark" placeholder="Buscar..." v-model="search" />
    </div>

    <div v-for="tribunal in paginatedTribunales" :key="tribunal.id_Tribunal" class="tribunal-card">
      <div class="tribunal-info">
        <div><strong>{{ tribunal.nombre_Tribunal }}</strong></div>
        <div class="small text-muted">{{ tribunal.descripcion }} - {{ tribunal.telefono }} - {{ tribunal.direccion }}</div>
        <div class="small">Provincia: {{ tribunal.distrito }}</div>
      </div>
      <div class="d-flex align-items-center gap-2">
        <span :class="['badge', tribunal.estatus ? 'bg-success' : 'bg-danger']">
          {{ tribunal.estatus ? 'Activo' : 'Inactivo' }}
        </span>
        <Button icon="pi pi-pencil" class="p-button-text p-button-sm text-dark" @click="abrirFormularioEditar(tribunal)" />
        <Button icon="pi pi-trash" class="p-button-text p-button-sm text-danger" @click="confirmarEliminacion(tribunal.id_Tribunal)" />
      </div>
    </div>

    <div v-if="filteredTribunales.length === 0" class="text-center text-muted mt-2">No hay tribunales registrados.</div>

    <div class="text-end mt-3">
      <Button label="Cerrar" icon="pi pi-times" class="p-button-sm" @click="dialogVisible = false" />
    </div>
  </Dialog>

  <Dialog v-model:visible="mostrarFormulario" modal class="dialog-formulario" :closable="false" header="Formulario Tribunal">
    <div class="p-fluid">
      <div class="field" v-for="campo in camposFormulario" :key="campo.label">
        <label>{{ campo.label }}</label>
        <InputText v-model="tribunalEditando[campo.model]" />
      </div>

      <div class="field">
        <label>Estado</label>
        <Dropdown v-model="tribunalEditando.estatus" :options="estadoOptions" optionLabel="label" optionValue="value" />
      </div>

      <div class="text-end mt-4">
        <Button label="Cancelar" class="p-button-text me-2" @click="cerrarFormulario" />
        <Button label="Guardar" class="p-button" style="background-color: #003870; color: white" @click="guardarTribunal" />
      </div>
    </div>
  </Dialog>

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

.dialog-formulario {
  width: 35vw;
}

.p-dropdown.p-focus {
  border-color: #003870 !important;
  box-shadow: 0 0 0 0.2rem rgba(0, 56, 112, 0.25);
}
</style>
