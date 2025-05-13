<template>
  <div class="card p-6 shadow-2">
    <div class="flex justify-content-between align-items-center mb-4">
      <h2 class="text-2xl font-semibold">Detalle del Litigio</h2>

      <router-link to="/drawer/home" class="btn text-white ms-auto" style="background-color: #003870;">
        <i class="fa-solid fa-home me-2"></i> Inicio
      </router-link>

      <VaButton @click="$emit('close')" icon="close" color="danger" size="small" class="p-2" />
    </div>

    <div v-if="loading">
      <p class="text-center text-lg text-500">Cargando litigio...</p>
    </div>

    <div v-else>
      <!-- Encabezado del documento -->
      <div class="document-header mb-5 p-4 border-round-lg" style="background: #f8f9fa;">
        <div class="flex align-items-center justify-content-between">
          <div>
            <h3 class="text-xl m-0" style="color: #003870;">Expediente Judicial</h3>
            <p class="text-sm text-600-dark m-0">No. Acto: <strong>{{ litigio?.ltg_acto || 'N/A' }}</strong></p>
          </div>
          <div class="text-right">
            <p class="text-sm m-0">Fecha: {{ formatDate(litigio?.ltg_Fecha_Acto) }}</p>
            <Tag :value="litigio?.estatus_Descripcion" :severity="getStatusSeverity(litigio?.estatus_Descripcion)"
              class="mt-1" style="color: #003870;" />
          </div>
        </div>
      </div>

      <!-- Sección de información principal -->
      <div class="grid mb-5">
        <div class="col-12 md:col-6">
          <div class="surface-50 p-3 border-round-lg border bg-white">
            <h4 class="mt-0 mb-3 text-lg" style="color: #003870;">Demandante</h4>
            <div class="grid ">
              <div class="col-6 field ">
                <label class="text-sm font-medium text-600-dark">Nombre</label>
                <p class="m-0">{{ litigio?.ltg_Demandante || 'N/A' }}</p>
              </div>
              <div class="col-6 field">
                <label class="text-sm font-medium text-600-dark">Cédula</label>
                <p class="m-0">{{ litigio?.ltg_Cedula_Demandante || 'N/A' }}</p>
              </div>
              <div class="col-6 field">
                <label class="text-sm font-medium text-600-dark">Tipo</label>
                <p class="m-0">{{ litigio?.ltg_Tipo_Demandante || 'N/A' }}</p>
              </div>
              <div class="col-6 field">
                <label class="text-sm font-medium text-600-dark">Nacionalidad</label>
                <p class="m-0">{{ litigio?.ltg_Nacionalidad || 'N/A' }}</p>
              </div>
            </div>
          </div>
        </div>

        <div class="col-12 md:col-6">
          <div class="surface-50 p-3 border-round-lg border bg-white">
            <h4 class="mt-0 mb-3 text-lg" style="color: #003870;">Representante</h4>
            <div class="grid">
              <div class="col-6 field">
                <label class="text-sm font-medium text-600-dark">Nombre</label>
                <p class="m-0">{{ litigio?.ltg_Nombre_Representante || 'N/A' }}</p>
              </div>
              <div class="col-6 field">
                <label class="text-sm font-medium text-600-dark">Cédula</label>
                <p class="m-0">{{ litigio?.ltg_Cedula_Representante || 'N/A' }}</p>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Detalles del proceso -->
      <div class="surface-50 p-4 mb-5 border-round-lg border bg-white">
        <h4 class="mt-0 mb-3 text-lg" style="color: #003870;">Detalles del Proceso</h4>
        <div class="grid">
          <div class="col-12 md:col-4 field">
            <label class="text-sm font-medium text-600-dark">Tipo de Demanda</label>
            <p class="m-0">{{ litigio?.tipoDemanda_Nombre || 'N/A' }}</p>
          </div>
          <div class="col-12 md:col-4 field">
            <label class="text-sm font-medium text-600-dark">Tribunal</label>
            <p class="m-0">{{ litigio?.nombre_Tribunal || 'N/A' }}</p>
          </div>
          <div class="col-12 md:col-4 field">
            <label class="text-sm font-medium text-600-dark">Digitador</label>
            <p class="m-0">{{ litigio?.nombreUsuario || 'N/A' }}</p>
          </div>
          <div class="col-12 md:col-4 field">
            <label class="text-sm font-medium text-600-dark">Fecha Audiencia</label>
            <p class="m-0">{{ formatDate(litigio?.ltg_Fecha_Audiencia) || 'N/A' }}</p>
          </div>
          <div class="col-12 md:col-4 field">
            <label class="text-sm font-medium text-600-dark">Fecha Actualización</label>
            <p class="m-0">{{ formatDate(litigio?.ltg_Fecha_Actualizacion) || 'N/A' }}</p>
          </div>
        </div>
      </div>

      <!-- Documentos y sentencia -->
      <div class="grid">
        <!-- Columna izquierda: Documentos (4 unidades en pantallas medianas/grandes) -->
        <div class="col-12 md:col-6">
          <div class="surface-50 p-4 border-round-lg h-full bg-white">
            <h4 class="mt-0 mb-3 text-lg" style="color: #003870;">Documentos</h4>
            <div v-if="rutas.length > 0">
              <div v-for="(ruta, index) in rutas" :key="index"
                class="flex align-items-center mb-3 p-3 border rounded-3 surface-100 bg-white cursor-pointer hover:surface-200 transition-duration-150"
                @click="mostrarArchivo(ruta.id_Ruta)">
                <i class="pi pi-file-pdf mr-2" style="color: #e74c3c;"></i>
                <span>{{ ruta.ruta_Nombre }}</span>
              </div>
            </div>
            <p v-else class="m-0 text-500-dark">No hay documentos adjuntos</p>
          </div>
        </div>

        <!-- Columna derecha: Comentarios (8 unidades en pantallas medianas/grandes) -->
        <div class="col-12 md:col-6">
          <div class="surface-50 p-4 border-round-lg h-full bg-white">
            <h4 class="mt-0 mb-3 text-lg" style="color: #003870;">Comentarios</h4>
            <div v-if="comentarios.length > 0">
              <div v-for="(item, index) in comentarios" :key="index"
                class="mb-3 p-3 border rounded-3 surface-100 bg-white">
                <p class="m-0 font-medium">{{ item.comentario }}</p>
                <small class="text-500-dark">{{ formatCommentDate(item.fecha) }}</small>
              </div>
            </div>
            <p v-else class="m-0 text-500">Sin comentarios</p>
          </div>
        </div>
      </div>
      <div class="col-12 md:col-2 m-3">
        <div class="surface-50 p-4 border-round-lg border h-full bg-white">
          <h4 class="mt-0 mb-3 text-lg" style="color: #003870;">Sentencia</h4>
          <p class="m-0">{{ litigio?.desc_Sentencia || 'No registrada' }}</p>
        </div>
      </div>

      <!-- Pie de documento -->
      <div class="flex justify-content-between mt-4 pt-3 border-top-1 surface-border ">
        <button class="btn" @click="togglePopUp(id)">Agregar Evidencia</button>

    <teleport to="body">
      <transition name="fade">
        <AgregarEvidencias v-if="popUp"
        :id ="litigioactual"
        @close="togglePopUp" />
      </transition>
    </teleport>

        <small class="text-500-dark">Sistema Judicial - {{ new Date().getFullYear() }}</small>
        <small class="text-500-dark">Sistema Sileg 2.0 - {{ new Date().getFullYear() }}</small>
        <Button label="Imprimir" icon="pi pi-print" class="p-button-sm p-button-text-dark" @click="printDocument" />
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
// import axios from 'axios';
import api from '@/utilities/api.js'
import AgregarEvidencias from '@/components/views/AgregarEvidencias.vue';

const popUp = ref(false);
const litigioactual = ref(null);

// Métodos
function togglePopUp(id) {
  popUp.value = !popUp.value;
  litigioactual.value = id;
  console.log('popup:', popUp.value);
}


const props = defineProps({
  id: {
    type: [Number, String],
    required: true,
    validator: (value) => {
      const isValid = value !== null && value !== undefined && value !== ''
      if (!isValid) {
        console.error('ID inválido recibido:', value)
      }
      return isValid
    }
  }
})

const litigio = ref(null)
const loading = ref(true)
const comentarios = ref([])
const rutas = ref([])

// Función para abrir el documento en una nueva pestaña
const mostrarArchivo = (idRuta) => {
  const apiUrl = import.meta.env.VITE_API_URL || ''; // O déjalo en '' si no usas variable de entorno
  window.open(`${apiUrl}/api/Files/rutas/${idRuta}`, '_blank');
}

// Función para formatear fechas
const formatDate = (dateString) => {
  if (!dateString) return 'N/A'
  return new Date(dateString).toLocaleDateString('es-ES', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  })
}

// Función para formatear la fecha de comentarios
const formatCommentDate = (dateString) => {
  if (!dateString) return 'Fecha no disponible'
  try {
    const options = {
      day: '2-digit',
      month: 'short',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    }
    return new Date(dateString).toLocaleDateString('es-ES', options)
  } catch {
    return dateString // Si falla el formateo, muestra la fecha original
  }
}

// Cargar información cuando el componente se monta
onMounted(async () => {
  if (!props.id) {
    console.error('ID no proporcionado o inválido:', props.id)
    loading.value = false
    return
  }

  try {
    const response = await api.get(`/api/Litigio/detallados/${props.id}`)

    if (!response.data) {
      throw new Error('La respuesta no contiene datos')
    }

    litigio.value = response.data

    // Obtener comentarios
    const comentariosResponse = await api.get(`/api/Litigio/comentarios/${props.id}`)
    comentarios.value = comentariosResponse.data || []

    // Obtener rutas
    const rutasResponse = await api.get(`/api/Files/rutas/${props.id}`)
    rutas.value = rutasResponse.data || []

  } catch (error) {
    console.error('Error al cargar litigio:', error)
  } finally {
    loading.value = false
  }
})

// Obtener estilo según el estado
const getStatusSeverity = (status) => {
  const statusMap = {
    'activo': 'success',
    'inactivo': 'warning',
    'cerrado': 'danger',
    'finalizado': 'info'
  }
  return statusMap[status?.toLowerCase()] || null
}

// Imprimir documento
const printDocument = () => {
  window.print()
}




</script>




<style scoped>
.field {
  margin-bottom: 1rem;
}

.document-header {
  border-left: 4px solid #003870;
}

@media print {
  .card {
    box-shadow: none !important;
    border: none !important;
  }
}

.md\:col-4 {
  padding-right: 1rem;
  /* Espacio a la derecha de documentos */
}

.md\:col-8 {
  padding-left: 1rem;
  /* Espacio a la izquierda de comentarios */
}

</style>


