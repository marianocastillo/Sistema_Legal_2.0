<template>
<Dialog
  v-model:visible="visible"
  modal
  :style="{ width: '32vw' }"
  :closable="false"
  :draggable="false"
  :header="`Abogados asignados - Acto No. ${props.ltg_acto}`"
>
    <!-- Tabla de asignados -->
    <table class="table table-bordered table-sm mt-3">
      <thead class="table-light">
        <tr>
          <th>Nombres</th>
          <th>Apellidos</th>
          <th>Acciones</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="abogado in abogadosAsignados" :key="abogado.idUsuario">
          <td>{{ abogado.nombres }}</td>
          <td>{{ abogado.apellidos }}</td>
          <td>
            <Button icon="pi pi-trash" class="p-button-danger p-button-sm" @click="eliminarAsignacion(abogado.idUsuario)" />
          </td>
        </tr>
        <tr v-if="abogadosAsignados.length === 0">
          <td colspan="3" class="text-center">No hay abogados asignados.</td>
        </tr>
      </tbody>
    </table>

    <!-- Dropdown + botones -->
    <div class="dropdown-section mt-4">
      <Dropdown
        v-model="usuarioSeleccionado"
        :options="usuariosFiltrados"
        optionLabel="nombre"
        optionValue="id"
        placeholder="Selecciona un abogado"
        appendTo="body"
        style="min-width: 300px"
        emptyMessage="-- No hay más abogados --"

      />
      <Button label="Asignar" @click="asignarAbogado" class="ml-2" />
      <Button label="Cerrar" class="p-button-secondary ml-2" @click="$emit('close')" />
    </div>
  </Dialog>
</template>



<script setup>
import { ref, onMounted, watch } from 'vue'
import Dropdown from 'primevue/dropdown'
import Dialog from 'primevue/dialog'
import Button from 'primevue/button'
import axios from 'axios'

const props = defineProps({
  id_Ltg: Number,
  ltg_acto: [String, Number]
})
// eslint-disable-next-line no-unused-vars
const emit = defineEmits(['close'])

const visible = ref(true)
const usuarioSeleccionado = ref(null)
const usuariosFiltrados = ref([])
const abogadosAsignados = ref([])

onMounted(() => {
  cargarUsuarios()
  cargarAsignados()
})

watch(() => props.id_Ltg, cargarAsignados)

async function cargarUsuarios() {
  try {
    const response = await axios.get('/api/Usuarios/UsuariosConPerfil')
    const todos = response.data.filter(u => u.idPerfil === 4)

    // Esperar a que los asignados ya estén cargados (por si acaso)
    if (!abogadosAsignados.value.length) {
      await cargarAsignados()
    }

    const idsAsignados = abogadosAsignados.value.map(a => a.idUsuario)

    usuariosFiltrados.value = todos
      .filter(u => !idsAsignados.includes(u.idUsuario)) // <-- 🔥 FILTRO CLAVE
      .map(u => ({
        id: u.idUsuario,
        nombre: `${u.nombres} ${u.apellidos}`.trim()
      }))
  } catch (error) {
    console.error("Error al cargar usuarios:", error)
  }
}

async function cargarAsignados() {
  const { data } = await axios.get(`/api/Usuarios/Asignados/${props.id_Ltg}`)
  abogadosAsignados.value = data
}

async function asignarAbogado() {
  if (!usuarioSeleccionado.value) return

  await axios.post('/api/Usuarios/Asignar-Litigio', {
    idUsuario: usuarioSeleccionado.value,
    idLtg: props.id_Ltg
  })

  usuarioSeleccionado.value = null
  cargarAsignados()
  await cargarUsuarios()
}

async function eliminarAsignacion(idUsuario) {
  const confirmacion = confirm("¿Eliminar esta asignación?")
  if (!confirmacion) return

  await axios.delete(`/api/Usuarios/EliminarAsignacion`, {
    params: { idUsuario, idLtg: props.id_Ltg }
  })

  cargarAsignados()
  await cargarUsuarios()
}

</script>

