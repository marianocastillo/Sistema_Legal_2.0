<template>
  <Dialog v-model:visible="visible" modal class="dialog-asignacion" :closable="false" :draggable="false"
    :header="`Abogados asignados - Acto No. ${props.ltg_acto}`">
    <!-- Lista de abogados asignados como tarjetas -->
    <div class="abogados-lista mt-3">
      <div v-for="abogado in abogadosAsignados" :key="abogado.idUsuario" class="abogado-card">
        <div class="abogado-nombre">
          <span class="nombre">
            {{ abogado.nombres }} {{ abogado.apellidos }} – {{ abogado.perfil }} - ({{ abogado.cantidadAsignaciones }}
            casos)
          </span>
        </div>
        <Button icon="pi pi-trash thicker-icon" class="p-button-rounded p-button-text p-button-sm text-danger"
          v-tooltip.top="'Eliminar abogado'" aria-label="Eliminar" @click="confirmarEliminacion(abogado.idUsuario)"
          style="background-color: #003870;" />
      </div>

      <div v-if="abogadosAsignados.length === 0" class="text-center text-muted mt-2">
        No hay abogados asignados.
      </div>
    </div>

    <!-- Dropdown + botones -->
    <div class="dropdown-section mt-4 d-flex align-items-center gap-2 flex-wrap">
      <Dropdown v-model="usuarioSeleccionado" :options="usuariosFiltrados" optionLabel="nombre" optionValue="id"
        placeholder="Selecciona un abogado" appendTo="body" style="min-width: 300px"
        emptyMessage="-- No hay más abogados --" />
      <Button label="Asignar" icon="pi pi-user-plus" class="p-button-sm text-white border-0"
        :style="{ backgroundColor: '#003870' }" :disabled="!usuarioSeleccionado" @click="asignarAbogado" />

      <Button label="Cerrar" icon="pi pi-times" class="p-button-sm btn-aceptar" @click="emit('close')" />
    </div>
  </Dialog>
  <Dialog v-model:visible="mostrarDialogoExito" modal class="dialog-exito-style" :closable="false" :draggable="false"
    header="Asignación exitosa">
    <div class="d-flex align-items-start gap-3 p-3">
      <i class="pi pi-check-circle text-success" style="font-size: 1.8rem; flex-shrink: 0;"></i>
      <p class="m-0">El abogado litigante fue asignado correctamente.</p>
    </div>
    <div class="text-end px-3 pb-3">
      <Button label="Aceptar" class="p-button-sm" :style="{ backgroundColor: '#003870', color: '#fff', border: 'none' }"
        @click="mostrarDialogoExito = false" />
    </div>
  </Dialog>

  <Dialog v-model:visible="mostrarDialogoEliminado" modal class="dialog-eliminado-style" :closable="false"
    :draggable="false" header="Asignación eliminada">
    <div class="d-flex align-items-start gap-3 p-3">
      <i class="pi pi-trash text-danger" style="font-size: 1.8rem; flex-shrink: 0;"></i>
      <p class="m-0">El abogado fue eliminado correctamente de este acto.</p>
    </div>
    <div class="text-end px-3 pb-3">
      <Button label="Aceptar" class="p-button-sm" :style="{ backgroundColor: '#003870', color: '#fff', border: 'none' }"
        @click="mostrarDialogoEliminado = false" />
    </div>
  </Dialog>


  <!-- ConfirmDialog visual -->
  <ConfirmDialog />

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
const mostrarDialogoExito = ref(false)
const mostrarDialogoEliminado = ref(false)

const loadingUsuarios = ref(false)
const loadingAsignados = ref(false)

const confirm = useConfirm()

onMounted(() => {
  recargarDatos()
})

watch(() => props.id_Ltg, () => {
  recargarDatos()
})

async function recargarDatos() {
  await cargarAsignados()
  await cargarUsuarios()
}

async function cargarUsuarios() {
  loadingUsuarios.value = true
  try {
    const { data } = await axios.get('/api/Usuarios/AbogadosConAsignaciones')
    const idsAsignados = abogadosAsignados.value.map(a => a.idUsuario)

    usuariosFiltrados.value = data
      .filter(u => !idsAsignados.includes(u.idUsuario))
      .map(u => ({
        id: u.idUsuario,
        nombre: `${u.nombres} ${u.apellidos} (${u.cantidadAsignaciones} casos)`
      }))
  } catch (error) {
    console.error("Error al cargar usuarios:", error)
    push.error(error.response?.data?.message || 'Error al cargar usuarios disponibles')
  } finally {
    loadingUsuarios.value = false
  }
}

async function cargarAsignados() {
  loadingAsignados.value = true
  try {
    const { data } = await axios.get(`/api/Usuarios/Asignados/${props.id_Ltg}`)
    abogadosAsignados.value = data
  } catch (error) {
    console.error("Error al cargar asignados:", error)
    push.error(error.response?.data?.message || 'Error al cargar abogados asignados')
  } finally {
    loadingAsignados.value = false
  }
}

async function asignarAbogado() {
  if (!usuarioSeleccionado.value) return

  const yaAsignado = abogadosAsignados.value.some(a => a.idUsuario === usuarioSeleccionado.value)
  if (yaAsignado) {
    push.warning('Este abogado ya está asignado.')
    return
  }

  const esValido = usuariosFiltrados.value.some(u => u.id === usuarioSeleccionado.value)
  if (!esValido) {
    push.warning('El abogado seleccionado ya no está disponible.')
    return
  }

  try {
    await axios.post('/api/Usuarios/Asignar-Litigio', {
      idUsuario: usuarioSeleccionado.value,
      idLtg: props.id_Ltg
    })

    mostrarDialogoExito.value = true
    usuarioSeleccionado.value = null
    await recargarDatos()
  } catch (error) {
    console.error("Error al asignar abogado:", error)
    push.error(error.response?.data?.message || 'Error al asignar abogado')
  }
}

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
        await axios.delete('/api/Usuarios/EliminarAsignacion', {
          params: { idUsuario, idLtg: props.id_Ltg }
        })
        mostrarDialogoEliminado.value = true
        await recargarDatos()
      } catch (error) {
        console.error("Error al eliminar asignación:", error)
        push.error(error.response?.data?.message || 'Error al eliminar asignación')
      }
    }
  })
}
</script>



<style>
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

.p-dropdown.p-focus {
  border-color: #003870 !important;
  /* azul oscuro, cambia si necesitas otro tono */
  box-shadow: 0 0 0 0.2rem rgba(0, 56, 112, 0.25);
  /* sombra azul suave */
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

.thicker-icon {
  color: #fff;
  font-weight: 600 !important;
}

.dialog-exito-style .p-dialog,
.dialog-eliminado-style .p-dialog {
  border-radius: 12px;
  box-shadow: 0 6px 18px rgba(0, 0, 0, 0.12);
}

.dialog-exito-style .p-dialog-header,
.dialog-eliminado-style .p-dialog-header {
  background-color: #f8f9fa;
  color: #003870;
  font-weight: 600;
  padding: 1rem;
  border-top-left-radius: 12px;
  border-top-right-radius: 12px;
}

.dialog-exito-style .p-dialog-content,
.dialog-eliminado-style .p-dialog-content {
  padding: 0;
}
</style>
