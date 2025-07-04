<template>
  <!-- 📦 Diálogo principal -->
  <Dialog v-model:visible="visible" modal class="w-11" :closable="false" :draggable="false"
    header="Mantenimiento de Tribunales">
    <div class="d-flex justify-content-between align-items-center mb-3">
      <Button class="text-white" style="background-color: #003870;" @click="abrirFormularioNuevo">
        <i class="fas fa-plus me-2"></i> Nuevo Tribunal
      </Button>
      <input type="text" class="form-control-sm bg-white text-dark" placeholder="Buscar..." v-model="search" />
    </div>

    <!-- Tabla -->
    <table class="table table-bordered table-hover table-sm">
      <thead class="table-light">
        <tr>
          <th @click="sort('nombre_Tribunal')">Nombre</th>
          <th @click="sort('descripcion')">Descripción</th>
          <th @click="sort('telefono')">Teléfono</th>
          <th @click="sort('direccion')">Dirección</th>
          <th @click="sort('distrito')">Provincia</th>
          <th @click="sort('estatus')">Estado</th>
          <th>Acciones</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="tribunal in paginatedTribunales" :key="tribunal.id_Tribunal">
          <td>{{ tribunal.nombre_Tribunal }}</td>
          <td>{{ tribunal.descripcion }}</td>
          <td>{{ tribunal.telefono }}</td>
          <td>{{ tribunal.direccion }}</td>
          <td>{{ tribunal.distrito }}</td>
          <td>
            <span :class="['badge', tribunal.estatus ? 'bg-success' : 'bg-danger']">
              {{ tribunal.estatus ? 'Activo' : 'Inactivo' }}
            </span>
          </td>
          <td>
            <Button size="small" icon="pi pi-pencil" class="me-2" style="background-color: #003870; color: white"
              @click="abrirFormularioEditar(tribunal)" />
            <Button size="small" icon="pi pi-trash" style="background-color: #003870; color: white"
              @click="confirmarEliminacion(tribunal.id_Tribunal)" />
          </td>
        </tr>
        <tr v-if="filteredTribunales.length === 0">
          <td colspan="7" class="text-center">No se han encontrado tribunales.</td>
        </tr>
      </tbody>
    </table>

    <!-- Paginación -->
    <nav>
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

    <!-- Cerrar -->
    <div class="text-end mt-4">
      <Button label="Cerrar" class="p-button-text" @click="emit('close')" />
    </div>
  </Dialog>

  <!-- ✏️ Diálogo crear/editar -->
  <Dialog v-model:visible="mostrarDialogoFormulario" modal :closable="false" header="Formulario Tribunal"
    style="width: 40vw;">
    <div class="p-fluid">
      <div class="field" v-for="(label, key) in camposTexto" :key="key">
        <label :for="key">{{ label }}</label>
        <InputText v-model="tribunalEditando[key]" />
      </div>

      <div class="field">
        <label>Estado</label>
        <Dropdown v-model="tribunalEditando.estatus" :options="estadoOptions" optionLabel="label"
          optionValue="value" />
      </div>

      <div class="text-end mt-4">
        <Button label="Cancelar" class="p-button-text me-2" @click="cerrarDialogo" />
        <Button label="Guardar" class="p-button" style="background-color: #003870;" @click="guardarTribunal" />
      </div>
    </div>
  </Dialog>

  <ConfirmDialog />
</template>

<script setup>
import { ref, computed, onMounted, watch } from 'vue'
import Dialog from 'primevue/dialog'
import ConfirmDialog from 'primevue/confirmdialog'
import InputText from 'primevue/inputtext'
import Dropdown from 'primevue/dropdown'
import Button from 'primevue/button'
import { useConfirm } from 'primevue/useconfirm'
import { push } from 'notivue'
import api from '@/utilities/api.js'

const emit = defineEmits(['close'])
const props = defineProps({ visible: Boolean })

const mostrarDialogoFormulario = ref(false)
const tribunalEditando = ref({})
const confirm = useConfirm()

const tribunales = ref([])
const search = ref('')
const sortBy = ref('nombre_Tribunal')
const sortDesc = ref(false)
const page = ref(1)
const perPage = 10

const camposTexto = {
  nombre_Tribunal: 'Nombre',
  descripcion: 'Descripción',
  telefono: 'Teléfono',
  direccion: 'Dirección',
  distrito: 'Provincia'
}

const estadoOptions = [
  { label: 'Activo', value: true },
  { label: 'Inactivo', value: false }
]

onMounted(() => {
  loadTribunales()
})

async function loadTribunales() {
  try {
    const response = await api.get('/api/Tribunales/Tribunales')
    tribunales.value = response.data
  } catch {
    push.error('No se pudieron cargar los tribunales')
  }
}

function abrirFormularioNuevo() {
  tribunalEditando.value = {
    nombre_Tribunal: '',
    descripcion: '',
    telefono: '',
    direccion: '',
    distrito: '',
    estatus: true
  }
  mostrarDialogoFormulario.value = true
}

function abrirFormularioEditar(tribunal) {
  tribunalEditando.value = { ...tribunal }
  mostrarDialogoFormulario.value = true
}

function cerrarDialogo() {
  mostrarDialogoFormulario.value = false
}

async function guardarTribunal() {
  try {
    const body = tribunalEditando.value
    if (!body.nombre_Tribunal?.trim()) {
      return push.warning('El nombre es obligatorio')
    }

    if (body.id_Tribunal) {
      await api.put('/api/Tribunales/Editar', body)
      push.success('Tribunal actualizado correctamente')
    } else {
      await api.post('/api/Tribunales/Crear', body)
      push.success('Tribunal creado correctamente')
    }

    cerrarDialogo()
    await loadTribunales()
  } catch {
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
        await loadTribunales()
        if (page.value > totalPages.value) page.value = totalPages.value
        push.success('Tribunal eliminado correctamente')
      } catch {
        push.error('No se pudo eliminar el tribunal')
      }
    }
  })
}

function sort(field) {
  if (sortBy.value === field) {
    sortDesc.value = !sortDesc.value
  } else {
    sortBy.value = field
    sortDesc.value = false
  }
}

const sortedTribunales = computed(() => {
  return [...tribunales.value].sort((a, b) => {
    const aVal = a[sortBy.value]
    const bVal = b[sortBy.value]
    if (aVal < bVal) return sortDesc.value ? 1 : -1
    if (aVal > bVal) return sortDesc.value ? -1 : 1
    return 0
  })
})

const filteredTribunales = computed(() => {
  if (!search.value) return sortedTribunales.value
  const term = search.value.toLowerCase()
  return sortedTribunales.value.filter(t =>
    Object.values(t).some(val => String(val ?? '').toLowerCase().includes(term))
  )
})

const paginatedTribunales = computed(() => {
  const start = (page.value - 1) * perPage
  return filteredTribunales.value.slice(start, start + perPage)
})

const totalPages = computed(() => {
  return Math.max(1, Math.ceil(filteredTribunales.value.length / perPage))
})
</script>
