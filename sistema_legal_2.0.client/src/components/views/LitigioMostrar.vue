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
      <div class=" card surface-50 p-4 mb-5 border-round-lg border bg-white ">
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
  <div
    v-for="item in evidenciasOrdenadas"
    :key="item.id_Evidencias"
    class="col-12 md:col-6"
  >
    <Accordion :activeIndex="null" multiple>
      <AccordionTab :header="`Evidencia #${item.id_Evidencias}`">
        <p class="text-sm text-500">Subido: {{ formatFecha(item.FechaSubida) }}</p>

        <div class="mb-2">
          <strong>Comentario:</strong>
          <p class="m-0">{{ item.comentario }}</p>
        </div>

        <div>
          <strong>Archivo:</strong>
          <a
            :href="`https://localhost:7177/api/Files/rutaspor/${item.id_Ruta}`"
            target="_blank"
            class="text-blue-600 hover:underline ml-2"
          >
            <i :class="getFileIcon(item.NombreArchivo)" style="color: #ff0000;"></i>
            {{ item.NombreArchivo }}
          </a>
        </div>
      </AccordionTab>
    </Accordion>
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
        <Button label="Agregar Evidencia y Comentario" icon="pi pi-comment" class="p-button-sm p-button-text-dark"
          @click="togglePopUp(id)" />

        <teleport to="body">
          <transition name="fade">
            <AgregarEvidencias v-if="popUp" :id_Ltg="litigioactual" @close="togglePopUp"
              @actualizar="obtenerComentariosConEvidencias" />
          </transition>
        </teleport>
        <small class="text-500-dark">Sistema Sileg 2.0 - {{ new Date().getFullYear() }}</small>

      </div>
    </div>
  </div>
</template>

<script setup>
import Accordion from 'primevue/accordion';
import AccordionTab from 'primevue/accordiontab';

import { ref, onMounted } from 'vue'
import api from '@/utilities/api.js'
import AgregarEvidencias from '@/components/views/AgregarEvidencias.vue';
import { computed } from 'vue';
const popUp = ref(false);
const litigioactual = ref(null);

const components = {
  Accordion,
  AccordionTab
};

function getFileIcon(nombre) {
  const ext = nombre.split('.').pop().toLowerCase();
  if (ext === 'pdf') return 'pi pi-file-pdf';
  if (['jpg', 'jpeg', 'png'].includes(ext)) return 'pi pi-image';
  if (['doc', 'docx'].includes(ext)) return 'pi pi-file-word';
  if (['xls', 'xlsx'].includes(ext)) return 'pi pi-file-excel';
  return 'pi pi-file';
}

const evidenciasOrdenadas = computed(() => {
  return [...evidencias.value].sort((a, b) => new Date(a.FechaSubida) - new Date(b.FechaSubida))
});
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
const evidencias = ref([]);
const rutaBase = `C:/Users/mariancastillo/Desktop/SistemaLitigio`;

// Función para abrir el documento en una nueva pestaña


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
function formatFecha(fechaISO) {
  const fecha = new Date(fechaISO);
  return fecha.toLocaleDateString() + ' ' + fecha.toLocaleTimeString();
}
async function obtenerComentariosConEvidencias() {
  try {
    const response = await api.get(`/api/Files/comentarios-evidencias/${props.id}`);
    evidencias.value = response.data || [];
  } catch (error) {
    console.error('Error al obtener evidencias unificadas:', error);
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
    obtenerComentariosConEvidencias();

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

.card {
  border-left: 4px solid #003870;
  box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
  background-color: #f8f9fa;
}

.text-primary {
  color: #003870;
}

@media print {
  .card {
    box-shadow: none !important;
    border: none !important;
  }
}

.pop-up {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.5);
  /* fondo semitransparente */
  display: flex;
  justify-content: center;
  align-items: center;
}

.pop-up-content {
  background: white;
  padding: 20px;
  width: 600px;
  /* <-- Este valor controla el ancho */
  height: 400px;
  /* <-- Este valor controla la altura */
  border-radius: 8px;
}
</style>
