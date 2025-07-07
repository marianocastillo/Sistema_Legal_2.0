<template>
  <Dialog v-model:visible="dialogVisible" modal header="Salas del Tribunal" class="dialog-salas">
    <div class="d-flex justify-content-between align-items-center mb-3">
      <h5 class="text-dark">Salas registradas</h5>
      <div class="d-flex gap-2">
        <Button label="Nueva Sala" icon="pi pi-plus" class="p-button-sm" @click="abrirFormularioNuevaSala" />
        <InputText v-model="search" placeholder="Buscar..." class="p-inputtext-sm" />
      </div>
    </div>

    <table class="table table-sm table-bordered">
      <thead class="table-light">
        <tr>
          <th>Sala</th>
          <th>Acciones</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="sala in paginatedSalas" :key="sala.idSala">
          <td>{{ sala.nombre }}</td>
          <td>
            <Button icon="pi pi-pencil" class="p-button-text p-button-sm" @click="editarSala(sala)" />
            <Button icon="pi pi-trash" class="p-button-text p-button-sm text-danger" @click="eliminarSala(sala.idSala)" />
          </td>
        </tr>
        <tr v-if="filteredSalas.length === 0">
          <td colspan="2" class="text-center text-muted">No hay salas registradas.</td>
        </tr>
      </tbody>
    </table>

    <div class="text-end">
      <Button label="Cerrar" icon="pi pi-times" class="p-button-sm" @click="dialogVisible = false" />
    </div>

    <!-- Formulario de Sala -->
    <Dialog v-model:visible="formVisible" modal header="Formulario de Sala" class="dialog-form">
      <div class="p-fluid">
        <div class="field">
          <label>Nombre de la Sala</label>
          <InputText v-model="form.nombre" />
        </div>
        <div class="text-end mt-2">
          <Button label="Cancelar" class="p-button-text me-2" @click="formVisible = false" />
          <Button label="Guardar" class="p-button" @click="guardarSala" />
        </div>
      </div>
    </Dialog>
  </Dialog>
</template>

<script setup>
import { ref, watch, computed } from 'vue'
import Dialog from 'primevue/dialog'
import Button from 'primevue/button'
import InputText from 'primevue/inputtext'
import api from '@/utilities/api'
import { push } from 'notivue'

//Props
const props = defineProps({
  visible: Boolean,
  tribunalId: Number
})
const emit = defineEmits(['update:visible'])

//Computed para manejar el v-model correctamente
const dialogVisible = computed({
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

const paginatedSalas = computed(() => filteredSalas.value)

watch(() => props.visible, async (val) => {
  if (val && props.tribunalId) {
    await cargarSalas()


  }
})

async function cargarSalas() {
  try {
    const res = await api.get(`/api/Tribunales/Salas`)

    // ✅ Asegura que salas.value sea siempre un array
    const resultado = res.data
    console.log('Salas:', resultado)
    salas.value = Array.isArray(resultado) ? resultado : resultado?.data || []

  } catch (err) {
    push.error('Error al cargar salas')
    salas.value = [] // asegúrate de dejarlo vacío para evitar errores posteriores
  }
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
.dialog-salas {
  width: 50vw;
}
.dialog-form {
  width: 30vw;
}
</style>
