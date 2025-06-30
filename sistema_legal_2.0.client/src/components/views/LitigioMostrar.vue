<template>
  <div class="card p-6 shadow-2">
    <div class="flex justify-content-between align-items-center mb-4">
      <h2 class="text-2xl font-semibold p-4">Detalle del Litigio</h2>
      <Tag :value="litigio?.desc_Sentencia" :severity="getSentenciaSeverity(litigio?.desc_Sentencia)" />

      <router-link :to="rutaInicio" class="btn text-white ms-auto" style="background-color: #003870;">
        <i class="fa-solid fa-home me-2"></i> Inicio
      </router-link>

      <VaButton @click="$emit('close')" icon="close" color="danger" size="small" class="p-2" />
    </div>

    <div v-if="loading">
      <p class="text-center text-lg text-500">Cargando litigio...</p>
    </div>

    <div v-else>
      <!-- Encabezado del documento -->
      <div class="document-header full-width mb-5">
        <div class="info-table">
          <div class="row align-items-end">
            <div class="cell">
              <h4 class="section-title">Expediente Judicial</h4>
              <p><strong>No. Acto:</strong> {{ litigio?.ltg_acto || 'N/A' }}</p>
            </div>
            <div class="cell">
              <h4 class="section-title">Tipo de demanda</h4>
              <p>{{ litigio?.tipoDemanda_Nombre || 'N/A' }}</p>
            </div>
            <div class="cell text-right">
              <label class="text-sm">Fecha</label>
              <p>{{ formatDate(litigio?.ltg_Fecha_Acto) }}</p>
              <Tag :value="litigio?.estatus_Descripcion" :severity="getStatusSeverity(litigio?.estatus_Descripcion)" />
            </div>
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
                <Card v-if="slotProps.item.content !== null" class="mt-2 p-card">
                  <template #title>
                    <span class="p-card-title">{{ slotProps.item.status }}</span>
                  </template>
                  <template #subtitle>
                    <span class="p-card-subtitle detalle-usuario">
                      {{ dayjs(slotProps.item.date).format('YYYY-MM-DD HH:mm:ss') }} -
                      <i class="pi pi-user mr-1 icono-pequeno"></i>
                      <span class="nombre-usuario">{{ slotProps.item.usuario }}</span>
                    </span>
                  </template>
                  <template #content>
                    <p class="p-card-content whitespace-pre-wrap">
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

      <!-- Sección combinada: Demandante y Representante -->
      <div class="info-card-double grid mb-5">
        <!-- Demandante -->
        <div class="info-half">
          <h4 class="section-title">Demandante</h4>
          <div class="info-table">
            <div class="row">
              <div class="cell">
                <label>Cédula</label>
                <p>{{ litigio?.ltg_Cedula_Demandante || 'N/A' }}</p>
              </div>
              <div class="cell">
                <label>Nombre</label>
                <p>{{ litigio?.ltg_Nombre_Demandante || 'N/A' }}</p>
              </div>

            </div>
            <div class="row">

              <div class="cell">
                <label>Nacionalidad</label>
                <p>{{ litigio?.ltg_Nacionalidad || 'N/A' }}</p>
              </div>
              <div class="cell">
                <label>Tipo</label>
                <p>{{ litigio?.ltg_Tipo_Demandante || 'N/A' }}</p>
              </div>
            </div>
          </div>
        </div>

        <!-- Representante -->
        <div class="info-half bordered-left">
          <h4 class="section-title">Representante</h4>
          <div class="info-table">
            <div class="row">
              <div class="cell">
                <label>Cédula</label>
                <p>{{ litigio?.ltg_Cedula_Representante || 'N/A' }}</p>
              </div>
              <div class="cell">
                <label>Nombre</label>
                <p>{{ litigio?.ltg_Nombre_Representante || 'N/A' }}</p>
              </div>

            </div>
            <div class="row">
              <div class="cell">
                <label>Nacionalidad</label>
                <p>{{ litigio?.ltg_Nacionalidad_Representante || 'N/A' }}</p>
              </div>

            </div>
          </div>
        </div>
      </div>

      <!-- Detalles del tribunal -->
      <div class="info-card-double grid mb-5">
        <!-- Primera mitad -->
        <div class="info-half">
          <h4 class="section-title">Detalles del tribunal </h4>
          <div class="info-table">
            <div class="row">
              <div class="cell">
                <label>Tribunal</label>
                <p>{{ litigio?.nombre_Tribunal || 'N/A' }}</p>
              </div>
              <div class="cell">
                <label>Teléfono</label>
                <p>{{ litigio?.tribunal_Telefono || 'N/A' }}</p>
              </div>
            </div>
            <div class="row">
              <div class="cell">
                <label>Dirección</label>
                <p>{{ litigio?.tribunal_Direccion || 'N/A' }}</p>
              </div>
              <div class="cell">
                <label>Fecha Audiencia</label>
                <p>{{ formatDate(litigio?.ltg_Fecha_Audiencia) || 'N/A' }}</p>
              </div>
            </div>
          </div>
        </div>

        <!-- Segunda mitad (Descripción) -->
        <div class="info-half bordered-left">
          <h4 class="section-title">Descripción</h4>
          <div class="info-table">
            <div class="row">
              <div class="cell" style="grid-column: 1 / -1;">
                <p>{{ litigio?.tribunal_Descripcion || 'N/A' }}</p>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Audiencias con evidencias y comentarios -->
      <div class="mt-6">
        <h2 class="text-xl font-semibold mb-3" style="color: #003870;">Audiencias y Evidencias <Button
            label="Editar Audiencias" icon="pi pi-pencil" class="custom-home-btn"
            @click="togglePopUpTribunal(id)" style="background-color: #5a7cb3;" /></h2>
        <Accordion :activeIndex="null" multiple>
          <AccordionTab v-for="(audiencia, index) in audiencias" :key="index"
            :header="`-${audiencia.numeroAudiencia} - (${audiencia.tipoAudiencia}) - ${formatDate(audiencia.fechaAudiencia)}-`">
            <div v-if="audiencia.evidenciasYComentarios?.length">
              <Accordion :activeIndex="null" multiple>
                <AccordionTab v-for="(ev, i) in audiencia.evidenciasYComentarios" :key="i" :header="ev.nombreEvidencia">
                  <p><strong>Comentario:</strong> {{ ev.textoComentario }}</p>
                  <p><strong>Fecha:</strong> {{ formatDate(ev.fechaComentario) }}</p>
                  <p><strong>Archivo:</strong>
                    <a :href="`/api/Files/rutaspor/${ev.rutaArchivo}`" target="_blank"
                      class="text-blue-600 hover:underline">
                      <i :class="getFileIcon(ev.nombreArchivo)" style="color: #ff0000;"></i>
                      {{ ev.nombreArchivo }}
                    </a>
                  </p>
                </AccordionTab>
              </Accordion>
            </div>
            <div v-else class="text-gray-500">
              No hay evidencias ni comentarios para esta audiencia.
            </div>
          </AccordionTab>
        </Accordion>


      </div>



      <!-- Sentencia
      <div class="col-12 md:col-2 m-3">
        <div class="surface-50 p-4 border-round-lg border h-full bg-white">
          <h4 class="mt-0 mb-3 text-lg" style="color: #003870;">Sentencia</h4>
          <p class="m-0">{{ litigio?.desc_Sentencia || 'No registrada' }}</p>
        </div>
      </div>
 -->
      <!-- Pie de documento -->
      <div class="flex justify-content-between mt-4 pt-3 border-top-1 surface-border">
        <Button label="Agregar Evidencia y Comentario" icon="pi pi-comment" class="custom-home-btn "
          @click="togglePopUpEvidencia(id)" style="background-color: #003870;" />
        <Button label="Agregar Audiencia" icon="pi pi-calendar-plus" class="custom-home-btn "
          @click="togglePopUpAudiencia(id)" style="background-color: #37517e;" />
        <teleport to="body">
          <transition name="fade">
            <AgregarEvidencias v-if="popUpEvidencia" :id_Ltg="litigioactual" @close="togglePopUpEvidencia"
              @actualizar="obtenerComentariosConEvidencias" />
          </transition>
        </teleport>
        <teleport to="body">
          <transition name="fade">
            <AgregarAudiencias v-if="popUpAudiencia" :id_Ltg="litigioactual" @close="togglePopUpAudiencia"
              @actualizar="obtenerAudiencias" />
          </transition>
        </teleport>

        <!-- Popup 3 -->
        <teleport to="body">
          <transition name="fade">
            <EditarAudiencias v-if="popUpTribunal" :id_Ltg="litigioactual" @close="togglePopUpTribunal"
              @actualizar="obtenerTribunal" />
          </transition>
        </teleport>

        <!-- <small class="text-500-dark">Sistema Sileg 2.0 - {{ new Date().getFullYear() }}</small> -->
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
import AgregarEvidencias from '@/components/views/Popups/AgregarEvidencias.vue';
import EditarAudiencias from '@/components/views/Popups/EditarAudiencias.vue';
import AgregarAudiencias from '@/components/views/Popups/AgregarAudiencias.vue';

import dayjs from 'dayjs';

const audiencias = ref([]);
const scrollContainer = ref(null);
const popUpEvidencia = ref(false);
const popUpAudiencia = ref(false);
const popUpTribunal = ref(false);
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
const obtenerAudiencias = async () => {
  try {
    const res = await api.get(`/api/Litigio/audiencias-con-evidencias-y-tribunal/${props.id}`);
    audiencias.value = res.data.audiencias || [];

    //Mezclar los datos del tribunal dentro del objeto litigio ya existente
    if (res.data.tribunalFinal) {
      litigio.value = {
        ...litigio.value,
        ...res.data.tribunalFinal
      };
    }

  } catch (err) {
    console.error('Error al obtener audiencias:', err);
  }
};


const abrirArchivo = async (rutaRelativa) => {
  try {
    const url = `/api/Files/DescargarArchivo?rutaRelativa=${encodeURIComponent(rutaRelativa)}`
    window.open(url, '_blank')
  } catch (error) {
    console.error('Error al abrir el archivo:', error)
    push.error('No se pudo abrir el archivo.')
  }
}


const estaCerrado = computed(() => litigio.value?.ltg_estatus === 7);

const togglePopUpEvidencia = (id) => {
  popUpEvidencia.value = !popUpEvidencia.value;
  litigioactual.value = id;
};

const togglePopUpAudiencia = (id) => {
  popUpAudiencia.value = !popUpAudiencia.value;
  litigioactual.value = id;
};

const togglePopUpTribunal = (id) => {
  popUpTribunal.value = !popUpTribunal.value;
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

function formatDate(fecha) {
  return dayjs(fecha).format('DD/MM/YYYY [a las] hh:mm A');
}


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

const rutaInicio = ref('/Administrador/litigios');

onMounted(async () => {
  try {
    //Lógica de ruta según perfil
    const rawUser = localStorage.getItem('usuario');
    const user = rawUser ? JSON.parse(rawUser) : null;

    if (user) {
      const perfil = parseInt(user.perfil);
      const rutasPorPerfil = {
        1: '/Administrador/litigios',
        2: '/Administrador/litigios',
        3: '/LitigiosRegistrados',
        4: '/abogado/inicio'
      };

      rutaInicio.value = rutasPorPerfil[perfil] || '/Administrador/litigios';
    }

    //Carga datos del litigio
    const response = await api.get(`/api/Litigio/detallados/${props.id}`);
    if (!response.data) throw new Error('La respuesta no contiene datos');
    litigio.value = response.data;
    await obtenerAudiencias();
    await cargarLineaDeTiempo();              // Carga eventos
    await nextTick();                         // Espera render del DOM

    //Mueve scroll al final
    setTimeout(() => {
      if (scrollContainer.value) {
        scrollContainer.value.scrollTo({
          left: scrollContainer.value.scrollWidth,
          behavior: 'auto'
        });
      }
    }, 100);

    //Muestra scroll después de animación
    setTimeout(() => {
      if (scrollContainer.value) {
        scrollContainer.value.classList.add('scroll-visible');
      }
    }, 2600);

  } catch (error) {
    console.error('Error al cargar el litigio:', error);
  } finally {
    loading.value = false;
  }
});




</script>

<style scoped>
/* POPUP GENERAL */
.pop-up {
  position: fixed;
  inset: 0;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 9999;
}

.pop-up-inner {
  background: white;
  color: black;
  padding: 30px;
  border-radius: 10px;
  width: 90%;
  max-width: 600px;
  box-shadow: 0 5px 15px rgba(0, 0, 0, 0.3);
  position: relative;
}

.pop-up-close {
  position: absolute;
  top: 8px;
  right: 12px;
  font-size: 3rem;
  color: #333;
  cursor: pointer;
}


.custom-home-btn {
  background-color: #003870 !important;
  border-color: #003870 !important;
}

.custom-home-btn:hover {
  background-color: #c00606 !important;
  border-color: #c00606  !important;
  box-shadow: 0 6px 16px rgba(121, 1, 51, 0.3) !important;
  transform: translateY(-1px) !important;
  transition: background-color 0.2s !important;
}

/*CALENDARIO DE PRIMEVUE */
::v-deep(.p-datepicker) {
  z-index: 100000 !important;
}

/*CONTENEDORES DEL POPUP */
.file-container,
.comment-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  margin-bottom: 15px;
}

/*TEXTAREAS */
textarea {
  width: 100%;
  max-width: 400px;
  padding: 10px;
  font-size: 1rem;
  border: 1px solid #ccc;
  border-radius: 5px;
  resize: none;
  box-sizing: border-box;
}

textarea::placeholder {
  color: #888;
}

/*TRANSICIONES */
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.25s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}

/*TARJETAS */
.card,
.surface-50,
.info-card-double {
  border-left: 4px solid #003870;
  box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
  background-color: #f8f9fa;
  border-radius: 6px;
  border: 1px solid #e0e0e0;
  padding: 1.25rem;
}

/*TABLA DE INFO */
.info-card-double {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  overflow: hidden;
}

.info-half {
  padding: 1rem;
}

.bordered-left {
  border-left: 1px solid #d1d5db;
}

/*ETIQUETAS */
.section-title,
h4,
.card h4,
.surface-50 h4 {
  color: #003870 !important;
  font-weight: 600;
  font-size: 1.1rem;
  margin-bottom: 1rem;
  border-left: 4px solid #003870;
  padding-left: 0.75rem;
  margin-top: 0;
}

label {
  font-weight: 600;
  color: #555;
  font-size: 0.85rem;
}

.text-sm {
  font-size: 0.85rem;
  font-weight: 600;
}

.mt-0 {
  margin-top: 0;
}

.mb-3 {
  margin-bottom: 1rem;
}

.m-0 {
  margin: 0;
}

p {
  margin: 0.2rem 0 1rem 0;
}

/* 📋 CELDAS */
.info-table {
  display: grid;
  gap: 1rem;
}

.row {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
  gap: 1rem;
}

.cell label {
  font-weight: 600;
  font-size: 0.85rem;
  color: #444;
}

.cell p {
  margin: 0;
  color: #222;
  font-size: 0.95rem;
}

/* 👤 DETALLES DE USUARIO */
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
}

.nombre-usuario {
  font-weight: 600;
  color: #003870;
}

/* ⏳ TIMELINE */
.timeline-wrapper {
  overflow-x: hidden;
  overflow-y: hidden;
  white-space: nowrap;
  width: 100%;
  transition: opacity 0.5s ease-in-out;
  scrollbar-width: none;
}

.timeline-wrapper.scroll-visible {
  overflow-x: auto;
  opacity: 1;
  scrollbar-width: thin;
}

.timeline-wrapper.scroll-visible::-webkit-scrollbar {
  height: 6px;
}

.timeline-wrapper.scroll-visible::-webkit-scrollbar-thumb {
  background-color: #999;
  border-radius: 3px;
}

.timeline-wrapper.scroll-visible::-webkit-scrollbar-track {
  background: transparent;
}

.timeline-wrapper::-webkit-scrollbar {
  display: none;
}

/* ANIMACIÓN */
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
  animation: fadeInUp 2s ease forwards;
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

.p-timeline-event-separator::before {
  background-color: #003870 !important;
}

:deep(.p-timeline-event-connector) {
  background-color: #3F6DAA;
}

:deep(.no-final-line + .p-timeline-event-connector) {
  background-color: #cccccc !important;
  opacity: 0.5;
}

/* 🗃️ CARD DETALLE */
.p-card {
  padding: 1rem !important;
  border-radius: 8px;
  box-shadow: 0 2px 6px rgba(0, 0, 0, 0.08);
}

.p-card-title {
  font-size: 1.2rem !important;
  font-weight: 600;
  color: #003870;
  margin-bottom: 0.5rem;
}

.p-card-subtitle {
  font-size: 0.875rem !important;
  color: #555;
  margin-bottom: 0.75rem;
}

.p-card-content p {
  font-size: 0.95rem !important;
  color: #222;
  line-height: 1.6;
  margin: 0;
}

/* 🖨️ IMPRESIÓN */
@media print {
  .card {
    box-shadow: none !important;
    border: none !important;
  }
}

/* UTILIDADES */
.text-right {
  text-align: right;
}

.align-items-end {
  align-items: end;
}

.full-width {
  width: 100%;
}

.document-header {
  width: 100%;
  padding: 1.25rem;
  border-radius: 6px;
  border-left: 4px solid #003870;
  border: 1px solid #e0e0e0;
  background-color: #f8f9fa;
  box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
  margin-bottom: 1.5rem;
}
</style>
