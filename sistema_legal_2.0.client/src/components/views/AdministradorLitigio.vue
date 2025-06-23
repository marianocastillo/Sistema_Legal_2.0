<template>
  <div class="card p-6 shadow-2">
    <div class="flex justify-between items-center mb-4 flex-wrap gap-2">
      <h2 class="text-2xl font-semibold">{{ tituloLitigio }}</h2>

      <div class="flex items-center gap-2 filtro-busqueda-bar">
        <div class="btn-filtro-group">
          <button class="btn-filtro" :class="{ active: filtroActivo === 'sinAsignar' }" @click="mostrarSinAsignar">
            Sin asignar
            <span class="badge" :class="{ 'badge-active': filtroActivo === 'sinAsignar' }">
              ({{ totalSinAsignar }})
            </span>
          </button>

          <button class="btn-filtro" :class="{ active: filtroActivo === 'asignado' }" @click="mostrarAsignados">
            Asignados
            <span class="badge" :class="{ 'badge-active': filtroActivo === 'asignado' }">
              ({{ totalAsignados }})
            </span>
          </button>
        </div>


        <div class="search-container">
          <span class="p-input-icon-left search-input-wrapper">
            <i class="pi pi-search search-icon" />
            <InputText v-model="filters.global.value" placeholder="Buscar litigio..." class="search-input" />
            <i v-if="filters.global.value" class="pi pi-times search-clear" @click="filters.global.value = ''" />
          </span>
        </div>
      </div>
    </div>

    <DataTable :value="data" :paginator="true" :rows="rows" :filters="filters" :globalFilterFields="[
      'ltg_acto',
      'ltg_Cedula_Demandante',
      'ltg_Fecha_Acto',
      'ltg_Demandante',
      'estatus_Descripcion'
    ]" class="p-datatable-sm" responsiveLayout="scroll">
      <Column field="ltg_acto" header="No.Acto" />
      <Column field="ltg_Fecha_Acto" header="Fecha acto">
        <template #body="{ data }">
          {{ data.ltg_Fecha_Acto?.split('T')[0] || '' }}
        </template>
      </Column>
      <Column field="ltg_Cedula_Demandante" header="Cédula demandante" />
      <Column field="ltg_Demandante" header="Nombre demandante" />
      <Column field="tipoDemanda_Nombre" header="Tipo de Demanda" />
      <Column field="ltg_Fecha_Audiencia" header="Fecha audiencia">
        <template #body="{ data }">
          {{ data.ltg_Fecha_Audiencia?.split('T')[0] || 'Sin fecha' }}
        </template>
      </Column>
      <Column field="estatus_Descripcion">
        <template #header>
          <div class="custom-header-center">Estatus</div>
        </template>
        <template #body="slotProps">
          <div class="text-center">
            <span class="tag" :class="getStatusClass(slotProps.data.estatus_Descripcion)">
              {{ slotProps.data.estatus_Descripcion }}
            </span>
          </div>
        </template>
      </Column>

      <Column header="Acciones" style="width: 180px">
        <template #body="{ data }">
          <div class="btn-group">
            <!-- Ver -->
            <router-link :to="`/litigio/detalle/${data.id_Ltg}`" class="btn btn-sm"
              style="background-color: #003870; border-color: #003870; margin-right: 0.3rem;" title="Ver litigio">
              <i class="pi pi-eye white-icon"></i>
            </router-link>

            <!-- Modificar -->
            <button class="btn btn-sm" @click="modificarLitigio(data)"
              style="background-color: #003870; border-color: #003870; margin-right: 0.3rem;" title="Modificar litigio">
              <i class="pi pi-pencil white-icon"></i>
            </button>

            <!-- Asignar -->
            <button v-if="mostrarAsignar" class="btn btn-sm" @click="togglePopUp(data)"
              style="background-color: #003870; border-color: #003870;" title="Asignar o cambiar abogado">
              <i class="pi pi-user-edit white-icon"></i>
            </button>
          </div>
        </template>
      </Column>



      <!-- <Column header="Acciones" style="width: 140px">
        <template #body="{ data }">
          <div class="btn-group">
            <router-link :to="`/litigio/detalle/${data.id_Ltg}`" class="btn btn-sm"
              style="background-color: #003870; border-color: #003870; margin-right: 0.3rem;">
              <i class="pi pi-eye white-icon"></i>
            </router-link>

            <button v-if="mostrarAsignar" class="btn btn-sm" @click="togglePopUp(data)"
              style="background-color: #003870; border-color: #003870;">
              <i class="pi pi-user-edit white-icon" title="Asignar o cambiar abogado"></i>
            </button>
          </div>
        </template>
      </Column> -->

      <teleport to="body">
        <transition name="fade">
          <AsignarAbogado v-if="popUp" :id_Ltg="litigioActual.id_Ltg" :ltg_acto="litigioActual.ltg_acto"
            @close="togglePopUp" @asignado-con-exito="handleAsignacionExitosa" />
        </transition>
      </teleport>
    </DataTable>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted } from 'vue';
import { FilterMatchMode } from '@primevue/core/api';
import InputText from 'primevue/inputtext';
import DataTable from 'primevue/datatable';
import Column from 'primevue/column';
import api from '@/utilities/api.js';
import { useRouter } from 'vue-router'
import AsignarAbogado from '@/components/views/AsignarAbogado.vue';
const router = useRouter()

const data = ref([]);
const filtroActivo = ref('sinAsignar');
const totalSinAsignar = ref(0);
const totalAsignados = ref(0);
const todosLosLitigios = ref([]);

const tituloLitigio = computed(() =>
  filtroActivo.value === 'sinAsignar' ? 'Litigios Sin asignar' : 'Litigios Asignados'
);
const filters = ref({
  global: { value: null, matchMode: FilterMatchMode.CONTAINS }
});

const rows = ref(10);
const popUp = ref(false);
const litigioActual = ref({});
const mostrarAsignar = ref(true);

function togglePopUp(litigio = null) {
  popUp.value = !popUp.value;
  litigioActual.value = litigio || {};
}


function getStatusClass(status) {
  switch (status) {
    case 'Recibido': return 'status-recibido';
    case 'Análisis': return 'status-analisis';
    case 'En Tribunal': return 'status-tribunal';
    case 'Sentencia':
    case 'Sentencia Definitiva': return 'status-sentencia';
    case 'Recurso de Casación': return 'status-casacion';
    case 'Cierre del Caso': return 'status-cierre';
    default: return '';
  }
}


function modificarLitigio(litigio) {
  localStorage.setItem('litigioModificacion', JSON.stringify(litigio))
  router.push('/modificarregistro')
}


function calculateRows() {
  const tableHeight = window.innerHeight - 300;
  const estimatedRowHeight = 50;
  rows.value = Math.max(Math.floor(tableHeight / estimatedRowHeight) - 1, 1);
}

// Botones de filtro
function mostrarSinAsignar() {
  filtroActivo.value = 'sinAsignar';
  const filtrados = todosLosLitigios.value.filter(
    l => l.estatus_Descripcion?.toLowerCase().trim() === 'recibido'
  );
  data.value = filtrados;
  totalSinAsignar.value = filtrados.length;
}

function mostrarAsignados() {
  filtroActivo.value = 'asignado';
  const filtrados = todosLosLitigios.value.filter(
    l => l.estatus_Descripcion?.toLowerCase().trim() !== 'recibido'
  );
  data.value = filtrados;
  totalAsignados.value = filtrados.length; // NUEVO
}

function actualizarTotales() {
  totalSinAsignar.value = todosLosLitigios.value.filter(
    l => l.estatus_Descripcion?.toLowerCase().trim() === 'recibido'
  ).length;

  totalAsignados.value = todosLosLitigios.value.filter(
    l => l.estatus_Descripcion?.toLowerCase().trim() !== 'recibido'
  ).length;
}



function handleAsignacionExitosa() {
  // Recargar todos los litigios desde el backend
  api.get('/api/Litigio/Litigio_detallado')
    .then(response => {
      todosLosLitigios.value = response.data;
      actualizarTotales();
      // Dependiendo del filtro actual, muestra la tabla correcta
      filtroActivo.value === 'sinAsignar' ? mostrarSinAsignar() : mostrarAsignados();
    })
    .catch(error => {
      console.error("Error al actualizar tabla:", error);
    });
}


onMounted(async () => {
  calculateRows();
  window.addEventListener('resize', calculateRows);

  try {
    const response = await api.get('/api/Litigio/Litigio_detallado');
    todosLosLitigios.value = response.data;

    // Calcular ambos totales al inicio
    totalSinAsignar.value = todosLosLitigios.value.filter(
      l => l.estatus_Descripcion?.toLowerCase().trim() === 'recibido'
    ).length;

    totalAsignados.value = todosLosLitigios.value.filter(
      l => l.estatus_Descripcion?.toLowerCase().trim() !== 'recibido'
    ).length;

    // Mostrar por defecto los sin asignar
    actualizarTotales();
    mostrarSinAsignar();
  } catch (error) {
    console.error('Error al cargar litigios:', error);
  }

});

onUnmounted(() => {
  window.removeEventListener('resize', calculateRows);
});
</script>

<style scoped>
.white-icon {
  color: white !important;
}

.btn-group .btn {
  margin: 0 2px;
}

/* === Contenedor de botones + búsqueda === */
.filtro-busqueda-bar {
  display: flex;
  align-items: center;
  gap: 1rem;
  flex-wrap: nowrap;
  margin-left: auto;
}

/* === Botones de filtro === */
.btn-filtro-group {
  display: flex;
  gap: 0.5rem;
}

.btn-filtro {
  padding: 0.35rem 1.2rem;
  height: 38px;
  border-radius: 6px;
  font-size: 0.88rem;
  font-weight: 500;
  background-color: #e6ecf3;
  border: 1px solid #ccc;
  color: #003870;
  transition: all 0.2s ease;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  line-height: 1;
  white-space: nowrap;
}

.btn-filtro:hover {
  background-color: #72889e;
  color: white;
  border-color: #72889e;
}

.btn-filtro.active {
  background-color: #72889e;
  color: white;
  border-color: #72889e;
}

/* === Input de búsqueda === */
.search-container {
  max-width: 250px;
  width: 100%;
  position: relative;
}

.search-input-wrapper {
  position: relative;
  display: block;
}

.search-input {
  width: 100%;
  padding: 0.5rem 2.5rem 0.5rem 2rem;
  font-size: 14px;
  border: 1px solid #ccc;
  border-radius: 6px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
  transition: border-color 0.3s, box-shadow 0.3s;
}

.search-input:focus {
  outline: none;
  border-color: #003870;
  box-shadow: 0 0 5px rgba(0, 56, 112, 0.3);
}

.search-icon {
  position: absolute;
  left: 10px;
  top: 50%;
  transform: translateY(-50%);
  color: #888;
  pointer-events: none;
  font-size: 1rem;
}

.search-clear {
  position: absolute;
  right: 10px;
  top: 50%;
  transform: translateY(-50%);
  color: #888;
  font-size: 1rem;
  cursor: pointer;
}

/* === Etiquetas de estatus === */
.tag {
  padding: 0.4rem 0.75rem;
  border-radius: 9999px;
  font-size: 0.8rem;
  font-weight: 600;
  display: inline-block;
  text-align: center;
  min-width: 120px;
  text-transform: capitalize;
}

.status-recibido {
  background-color: #d0eaff;
  color: #004085;
}

.status-analisis {
  background-color: #b8ecff;
  color: #07506c;
}

.status-tribunal {
  background-color: #fff3cd;
  color: #856404;
}

.status-sentencia {
  background-color: #d4edda;
  color: #155724;
}

.status-casacion {
  background-color: #f8d7da;
  color: #721c24;
}

.status-cierre {
  background-color: #e2f0d9;
  color: #2e7d32;
}

/* === Tabla === */
.custom-header-center {
  text-align: center;
  font-weight: 600;
  color: #2e3842;
  font-size: 1rem;
  display: block;
  width: 100%;
}

::v-deep(.p-datatable .p-datatable-tbody > tr:nth-child(even)) {
  background-color: #f9fafb;
}

::v-deep(.p-datatable .p-datatable-tbody > tr > td),
::v-deep(.p-datatable .p-datatable-thead > tr > th) {
  border-right: 1px solid #ebebeb;
}

::v-deep(.p-datatable .p-datatable-tbody > tr > td:last-child),
::v-deep(.p-datatable .p-datatable-thead > tr > th:last-child) {
  border-right: none;
}

.badge {
  color: #003870;
  font-size: 0.75rem;
  margin-bottom: 12px;
  transition: all 0.2s ease;
}

.badge-active {
  color: #e6f3ff;
  font-weight: bold;
}



/* === Responsivo === */
@media (max-width: 768px) {
  .filtro-busqueda-bar {
    flex-direction: column;
    align-items: flex-end;
    width: 100%;
    margin-left: 0;
    gap: 0.5rem;
  }

  .btn-filtro-group,
  .search-container {
    width: 100%;
    justify-content: flex-end;
  }
}
</style>
