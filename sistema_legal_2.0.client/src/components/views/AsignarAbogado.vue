<template>
  <Dialog
    v-model:visible="visible"
    modal
    class="dialog-asignacion"
    :closable="false"
    :draggable="false"
    :header="`Abogados asignados - Acto No. ${props.ltg_acto}`"
  >
    <!-- Lista de abogados asignados como tarjetas -->
    <div class="abogados-lista mt-3">
      <div
        v-for="abogado in abogadosAsignados"
        :key="abogado.idUsuario"
        class="abogado-card"
      >
        <div class="abogado-nombre">
          <span class="nombre">
            {{ abogado.nombres }} {{ abogado.apellidos }} – {{ abogado.perfil }}
          </span>
        </div>
        <Button
          icon="pi pi-trash"
          class="p-button-rounded p-button-text p-button-sm text-danger"
          v-tooltip.top="'Eliminar abogado'"
          aria-label="Eliminar"
          @click="confirmarEliminacion(abogado.idUsuario)" style="background-color: #003870;"
        />
      </div>

      <div v-if="abogadosAsignados.length === 0" class="text-center text-muted mt-2">
        No hay abogados asignados.
      </div>
    </div>

    <!-- Dropdown + botones -->
    <div class="dropdown-section mt-4 d-flex align-items-center gap-2 flex-wrap">
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
      <Button
        label="Asignar"
        icon="pi pi-user-plus"
        class="p-button-sm text-white border-0"
        :style="{ backgroundColor: '#003870' }"
        :disabled="!usuarioSeleccionado"
        @click="asignarAbogado"
      />

      <Button
        label="Cerrar"
        icon="pi pi-times"
        class="p-button-secondary p-button-sm"
        @click="emit('close')"
      />
    </div>
  </Dialog>

  <!-- ConfirmDialog visual -->
  <ConfirmDialog  />
</template>

<script setup>
import { ref, onMounted, watch } from 'vue'
import Dropdown from 'primevue/dropdown'
import Dialog from 'primevue/dialog'
import Button from 'primevue/button'
import ConfirmDialog from 'primevue/confirmdialog'
import { useConfirm } from 'primevue/useconfirm'
import axios from 'axios'
import { push } from 'notivue'

const props = defineProps({
  id_Ltg: Number,
  ltg_acto: [String, Number]
})
const emit = defineEmits(['close'])

const visible = ref(true)
const usuarioSeleccionado = ref(null)
const usuariosFiltrados = ref([])
const abogadosAsignados = ref([])

const confirm = useConfirm()

onMounted(async () => {
  await cargarAsignados()
  await cargarUsuarios()
})

watch(() => props.id_Ltg, cargarAsignados)

async function cargarUsuarios() {
  try {
    const response = await axios.get('/api/Usuarios/UsuariosConPerfil')
    const todos = response.data.filter(u => u.idPerfil === 4)

    const idsAsignados = abogadosAsignados.value.map(a => a.idUsuario)

    usuariosFiltrados.value = todos
      .filter(u => !idsAsignados.includes(u.idUsuario))
      .map(u => ({
        id: u.idUsuario,
        nombre: `${u.nombres} ${u.apellidos} – ${u.nombrePerfil}`.trim()
      }))
  } catch (error) {
    console.error("Error al cargar usuarios:", error)
    push.error('Error al cargar usuarios disponibles')
  }
}

async function cargarAsignados() {
  try {
    const { data } = await axios.get(`/api/Usuarios/Asignados/${props.id_Ltg}`)
    abogadosAsignados.value = data
  } catch (error) {
    console.error("Error al cargar asignados:", error)
    push.error('Error al cargar abogados asignados')
  }
}

async function asignarAbogado() {
  cargarAsignados()
  if (!usuarioSeleccionado.value ) return

  if (abogadosAsignados.value.some(a => a.idUsuario === usuarioSeleccionado.value)) {
    push.warning('Este abogado ya está asignado.')
    return
  }

  try {
    await axios.post('/api/Usuarios/Asignar-Litigio', {
      idUsuario: usuarioSeleccionado.value,
      idLtg: props.id_Ltg
    })

    push.success('Abogado asignado correctamente')
    usuarioSeleccionado.value = null
    await Promise.all([cargarAsignados(), cargarUsuarios()])
  } catch (error) {
    console.error("Error al asignar abogado:", error)
    push.error('Error al asignar abogado')
  }
}

// Nuevo método con ConfirmDialog
function confirmarEliminacion(idUsuario) {
  confirm.require({
    message: '¿Estás seguro de eliminar esta asignación?',
    header: 'Confirmar eliminación',
    icon: 'pi pi-exclamation-triangle',
    acceptLabel: 'Sí',
    rejectLabel: 'Cancelar',
    acceptClass: 'p-button btn-aceptar',
    rejectClass: 'p-button btn-cancelar',

    accept: async () => {
      try {
        await axios.delete(`/api/Usuarios/EliminarAsignacion`, {
          params: { idUsuario, idLtg: props.id_Ltg }
        })
        push.success('Abogado eliminado correctamente')
        cargarUsuarios()
        await Promise.all([cargarAsignados(), cargarUsuarios()])
      } catch (error) {
        console.error("Error al eliminar asignación:", error)
        push.error('Error al eliminar asignación')
      }
    }
  })
}
</script>

<style scoped>
.dialog-asignacion {
  width: 32vw;
}

.abogados-lista {
  display: flex;
  flex-direction: column;
}

.abogado-card {
  display: flex;
  justify-content: space-between;
  align-items: center;
  border: 1px solid #ddd;
  border-radius: 8px;
  padding: 0.75rem 1rem;
  margin-bottom: 0.5rem;
  background-color: #fff;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.05);
}

.abogado-nombre {
  display: flex;
  flex-direction: column;
  font-size: 0.95rem;
}

.nombre {
  font-weight: 600;
}

.apellido {
  font-size: 0.875rem;
  color: #555;
}
</style>
