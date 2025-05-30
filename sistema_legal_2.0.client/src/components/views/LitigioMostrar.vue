<template>
  <div class="card p-6 shadow-2">
    <div class="flex justify-content-between align-items-center mb-4">
      <h2 class="text-2xl font-semibold p-4">Detalle del Litigio</h2>
      <Tag :value="litigio?.desc_Sentencia" :severity="getSentenciaSeverity(litigio?.desc_Sentencia)" />

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
            <p class="text-sm text-600-dark m-0">No. Acto: <strong>{{ litigio?.ltg_acto || 'N/A' }} </strong></p>
          </div>
          <div>
            <h3 class="text-xl  m-0" style="color: #003870;">Tipo de demanda</h3>
            <p class="text-sm text-600-dark m-0">{{ litigio?.tipoDemanda_Nombre || 'N/A' }} </p>
          </div>
          <div class="text-right">
            <p class="text-sm m-0">Fecha: {{ formatDate(litigio?.ltg_Fecha_Acto) }}</p>
            <Tag :value="litigio?.estatus_Descripcion" :severity="getStatusSeverity(litigio?.estatus_Descripcion)"
              class="mt-1" style="color: #003870;" />
          </div>
        </div>
      </div>

      <div class="card surface-50 p-4 mb-5 border-round-lg border bg-white">
        <h2 class="text-xl font-semibold mb-3" style="color: #003870;">Historial del Litigio</h2>
        <div ref="scrollContainer" class="timeline-wrapper">
          <div style="min-width: max-content;">
            <Timeline :value="events" align="left" layout="horizontal" class="customized-timeline fade-timeline">
              <template #marker="slotProps">
                <div class="timeline-marker"
                  :class="{ 'no-final-line': slotProps.index === events.length - 2 && !estaCerrado }" :style="{
                    backgroundColor: slotProps.item.color,
                    width: '32px',
                    height: '32px',
                    borderRadius: '50%',
                    display: 'grid',
                    placeItems: 'center',
                    boxShadow: '0 2px 6px rgba(0,0,0,0.2)'
                  }">
                  <i :class="slotProps.item.icon" style="color: white; font-size: 16px;"></i>
                </div>
              </template>

              <template #content="slotProps">
                <Card v-if="slotProps.item.content !== null" class="mt-2" style="height: 154px;">
                  <template #title>
                    <span class="text-sm font-medium">{{ slotProps.item.status }}</span>
                  </template>
                  <template #subtitle>
                    <span class="detalle-usuario">
                      {{ dayjs(slotProps.item.date).format('YYYY-MM-DD HH:mm:ss') }} -
                      <i class="pi pi-user mr-1 icono-pequeno"></i>
                      <span class="nombre-usuario">{{ slotProps.item.usuario }}</span>
                    </span>
                  </template>
                  <template #content>
                    <p class="text-xs text-gray-700 mt-2 whitespace-pre-wrap">
                      {{ slotProps.item.content }}
                    </p>
                  </template>
                </Card>
                <div v-else class="mt-4" style="height: 153px; width: 100%; visibility: hidden;"></div>
              </template>
            </Timeline>
          </div>
        </div>
      </div>

      <!-- Sección de información principal -->
      <div class="grid mb-5">
        <div class="col-12 md:col-6">
          <div class="surface-50 p-3 border-round-lg border bg-white">
            <h4 class="mt-0 mb-3 text-lg" style="color: #003870;">Demandante</h4>
            <div class="grid">
              <div class="col-2 field">
                <label class="text-sm">
                  {{ (litigio?.ltg_Cedula_Demandante?.trim().length || 0) < 10 ? 'RNC' : 'Cédula' }} </label>
                    <p class="m-0">{{ litigio?.ltg_Cedula_Demandante || 'N/A' }}</p>
              </div>
              <div class="col-4 field">
                <label class="text-sm font-medium text-600-dark">Nombre</label>
                <p class="m-0">{{ litigio?.ltg_Demandante || 'N/A' }}</p>
              </div>
              <div class="col-2 field">
                <label class="text-sm font-medium text-600-dark">Tipo</label>
                <p class="m-0">{{ litigio?.ltg_Tipo_Demandante || 'N/A' }}</p>
              </div>
              <div class="col-4 field">
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
              <div class="col-4 field">
                <label class="text-sm font-medium text-600-dark">Nombre</label>
                <p class="m-0">{{ litigio?.ltg_Nombre_Representante || 'N/A' }}</p>
              </div>
              <div class="col-3 field">
                <label class="text-sm font-medium text-600-dark">Cédula</label>
                <p class="m-0">{{ litigio?.ltg_Cedula_Representante || 'N/A' }}</p>
              </div>
              <div class="col-4 field">
                <label class="text-sm font-medium text-600-dark">Nacionalidad</label>
                <p class="m-0">{{ litigio?.ltg_Nacionalidad_Representante || 'N/A' }}</p>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Tribunales -->
      <div class="card surface-50 p-4 mb-5 border-round-lg border bg-white">
        <h4 class="mt-0 mb-3 text-lg" style="color: #003870;">Detalles del tribunal</h4>
        <div class="grid">
          <div class="col-12 md:col-2 field">
            <label class="text-sm font-medium text-600-dark">Tribunal</label>
            <p class="m-0">{{ litigio?.nombre_Tribunal || 'N/A' }}</p>
          </div>
          <div class="col-12 md:col-2 field">
            <label class="text-sm font-medium text-600-dark">Teléfono</label>
            <p class="m-0">{{ litigio?.tribunal_Telefono || 'N/A' }}</p>
          </div>
          <div class="col-12 md:col-4 field">
            <label class="text-sm font-medium text-600-dark">Dirección</label>
            <p class="m-0">{{ litigio?.tribunal_Direccion || 'N/A' }}</p>
          </div>
          <div class="col-12 md:col-4 field">
            <label class="text-sm font-medium text-600-dark">Fecha Audiencia</label>
            <p class="m-0">{{ formatDate(litigio?.ltg_Fecha_Audiencia) || 'N/A' }}</p>
          </div>
          <div class="col-12 md:col-4 field">
            <label class="text-sm font-medium text-600-dark">Descripción</label>
            <p class="m-0">{{ litigio?.tribunal_Descripcion || 'N/A' }}</p>
          </div>
        </div>
      </div>

      <!-- Documentos y sentencia -->
      <div class="grid">
        <div v-for="item in evidenciasOrdenadas" :key="item.id_Evidencias" class="col-12">
          <Accordion :activeIndex="null" multiple>
            <AccordionTab :header="`${item.Nombre}`">
              <p class="text-sm text-500">Subido: {{ formatFecha(item.FechaSubida) }}</p>
              <div class="mb-2">
                <strong>Comentario:</strong>
                <p class="m-0">{{ item.comentario }}</p>
              </div>
              <div>
                <strong>Archivo:</strong>
                <a :href="`https://localhost:7177/api/Files/rutaspor/${item.id_Ruta}`" target="_blank"
                  class="text-blue-600 hover:underline ml-2">
                  <i :class="getFileIcon(item.NombreArchivo)" style="color: #ff0000;"></i>
                  {{ item.NombreArchivo }}
                </a>
              </div>
            </AccordionTab>
          </Accordion>
        </div>
      </div>

      <!-- Sentencia -->
      <div class="col-12 md:col-2 m-3">
        <div class="surface-50 p-4 border-round-lg border h-full bg-white">
          <h4 class="mt-0 mb-3 text-lg" style="color: #003870;">Sentencia</h4>
          <p class="m-0">{{ litigio?.desc_Sentencia || 'No registrada' }}</p>
        </div>
      </div>

      <!-- Pie de documento -->
      <div class="flex justify-content-between mt-4 pt-3 border-top-1 surface-border">
        <Button label="Agregar Evidencia y Comentario" icon="pi pi-comment" class="p-button-sm p-button-text-dark"
          @click="togglePopUp(id)" style="background-color: #003870;" />

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
import Timeline from 'primevue/timeline';
import { ref, onMounted, nextTick, computed } from 'vue';
import api from '@/utilities/api.js';
import AgregarEvidencias from '@/components/views/AgregarEvidencias.vue';
import dayjs from 'dayjs';

const scrollContainer = ref(null);
const popUp = ref(false);
const litigioactual = ref(null);
const litigio = ref(null);
const loading = ref(true);
const evidencias = ref([]);
const events = ref([]);

const props = defineProps({
  id: {
    type: [Number, String],
    required: true,
    validator: value => {
      const valid = value !== null && value !== undefined && value !== '';
      if (!valid) console.error('ID inválido recibido:', value);
      return valid;
    }
  }
});

const estaCerrado = computed(() => litigio.value?.ltg_estatus === 7);

const togglePopUp = (id) => {
  popUp.value = !popUp.value;
  litigioactual.value = id;
};

const getFileIcon = (nombre) => {
  const ext = nombre.split('.').pop().toLowerCase();
  if (ext === 'pdf') return 'pi pi-file-pdf';
  if (['jpg', 'jpeg', 'png'].includes(ext)) return 'pi pi-image';
  if (['doc', 'docx'].includes(ext)) return 'pi pi-file-word';
  if (['xls', 'xlsx'].includes(ext)) return 'pi pi-file-excel';
  return 'pi pi-file';
};

const formatDate = (dateString) => {
  if (!dateString) return 'N/A';
  return new Date(dateString).toLocaleDateString('es-ES', {
    year: 'numeric', month: 'long', day: 'numeric'
  });
};

const formatFecha = (fechaISO) => {
  const fecha = new Date(fechaISO);
  return fecha.toLocaleDateString() + ' ' + fecha.toLocaleTimeString();
};

const evidenciasOrdenadas = computed(() => {
  return [...evidencias.value].sort((a, b) => new Date(a.FechaSubida) - new Date(b.FechaSubida));
});

const getStatusSeverity = (status) => {
  const map = {
    'activo': 'success',
    'inactivo': 'warning',
    'cerrado': 'danger',
    'finalizado': 'info'
  };
  return map[status?.toLowerCase()] || null;
};

const getSentenciaSeverity = (sentencia) => {
  const val = sentencia?.toLowerCase();
  if (!val) return 'info';
  if (val.includes('contra')) return 'danger';
  if (val.includes('favor')) return 'warning';
  return 'info';
};

const obtenerComentariosConEvidencias = async () => {
  try {
    const res = await api.get(`/api/Files/comentarios-evidencias/${props.id}`);
    evidencias.value = res.data || [];
  } catch (err) {
    console.error('Error al obtener evidencias:', err);
  }
};

const cargarLineaDeTiempo = async () => {
  try {
    const res = await api.get(`/api/Litigio/historial/${props.id}`);
    events.value = res.data || [];

    events.value.push({
      status: '',
      date: '',
      icon: 'pi pi-check-circle',
      color: '#4CAF50',
      content: null,
      usuario: ''
    });
  } catch (err) {
    console.error('Error al cargar historial:', err);
  }
};

onMounted(async () => {
  try {
    const response = await api.get(`/api/Litigio/detallados/${props.id}`);
    if (!response.data) throw new Error('La respuesta no contiene datos');
    litigio.value = response.data;

    await cargarLineaDeTiempo();              // Carga eventos
    await obtenerComentariosConEvidencias();  // Carga evidencias
    await nextTick();                         // Espera render del DOM

    // Mueve scroll horizontal al final (último evento visible)
    setTimeout(() => {
      if (scrollContainer.value) {
        scrollContainer.value.scrollTo({
          left: scrollContainer.value.scrollWidth,
          behavior: 'auto' // Cambia a 'smooth' si deseas efecto suave
        });
      }
    }, 100); // Aumenta si el render necesita más tiempo

    // 🔓 Muestra el scroll después de la animación
    setTimeout(() => {
      if (scrollContainer.value) {
        scrollContainer.value.classList.add('scroll-visible');
      }
    }, 2600); // Coincide con duración de animación (ej. 2.6s)

  } catch (error) {
    console.error('Error al cargar el litigio:', error);
  } finally {
    loading.value = false;
  }
});



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
}

.md\:col-8 {
  padding-left: 1rem;
}

.card {
  border-left: 4px solid #003870;
  box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
  background-color: #f8f9fa;
}

.text-primary {
  color: #003870;
}

.timeline:hover {
  transform: scale(1.02);
  transition: transform 0.3s ease-in-out;
}

.pop-up {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  justify-content: center;
  align-items: center;
}

.text-sm {
  font-size: 1.2rem !important;
  font-weight: 600;
}

.pop-up-content {
  background: white;
  padding: 20px;
  width: 600px;
  height: 400px;
  border-radius: 8px;
}

.detalle-usuario {
  font-size: 12px;
  color: #6b7280;
  display: flex;
  align-items: center;
}

.icono-pequeno {
  font-size: 0.75rem;
  color: #003870;
  margin-right: 4px;
  vertical-align: middle;
}

.nombre-usuario {
  font-weight: 600;
  color: #003870;
}

/* Timeline scroll oculto al inicio con transición */
.timeline-wrapper {
  overflow-x: hidden;
  overflow-y: hidden;
  white-space: nowrap;
  width: 100%;
  scrollbar-width: none; /* Firefox */
  transition: opacity 0.5s ease-in-out;
}

/* Scroll visible y animado */
.timeline-wrapper.scroll-visible {
  overflow-x: auto;
  opacity: 1;
  scrollbar-width: thin;
}

.timeline-wrapper.scroll-visible::-webkit-scrollbar {
  height: 6px;
  display: block;
}

.timeline-wrapper.scroll-visible::-webkit-scrollbar-thumb {
  background-color: #999;
  border-radius: 3px;
  cursor: pointer;
}

.timeline-wrapper.scroll-visible::-webkit-scrollbar-track {
  background: transparent;
}

/* Oculta scroll en WebKit cuando no visible */
.timeline-wrapper::-webkit-scrollbar {
  display: none;
}

/* Animación Timeline */
@keyframes fadeInUp {
  from {
    opacity: 0;
    transform: translateY(12px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.fade-timeline>>>.p-timeline-event {
  opacity: 0;
  transform: translateY(12px);
  animation: fadeInUp 2.5s ease forwards;
}

.fade-timeline>>>.p-timeline-event:nth-child(1) {
  animation-delay: 300ms;
}
.fade-timeline>>>.p-timeline-event:nth-child(2) {
  animation-delay: 600ms;
}
.fade-timeline>>>.p-timeline-event:nth-child(3) {
  animation-delay: 900ms;
}
.fade-timeline>>>.p-timeline-event:nth-child(4) {
  animation-delay: 1200ms;
}
.fade-timeline>>>.p-timeline-event:nth-child(5) {
  animation-delay: 1500ms;
}
.fade-timeline>>>.p-timeline-event:nth-child(6) {
  animation-delay: 1800ms;
}

/* Línea conectora principal */
.p-timeline-event-separator::before {
  background-color: #003870 !important;
}

/* Línea conectora entre eventos */
:deep(.p-timeline-event-connector) {
  --p-timeline-event-connector-color: #3F6DAA;
  background-color: #3F6DAA;
}

/* Si no está cerrado, la línea no debe continuar */
:deep(.no-final-line + .p-timeline-event-connector) {
  background-color: #cccccc !important;
  opacity: 0.5;
}
</style>


