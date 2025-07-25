<template>
  <div class="card p-6 shadow-2">
    <div class="flex justify-between items-center mb-4 flex-wrap gap-2">
      <h2 class="text-2xl font-bold">Seguimiento de Litigios</h2>

      <!-- Filtros por estatus -->
      <div class="flex items-center gap-2 filtro-busqueda-bar">
        <Dropdown v-model="estatusSeleccionado" :options="estatusDisponibles" optionLabel="label" optionValue="value"
          placeholder="Filtrar por estatus" class="w-full md:w-16rem" />

        <span class="p-input-icon-left search-input-wrapper">
          <i class="pi pi-search search-icon" />
          <InputText v-model="filters.global.value" placeholder="Buscar litigio..." class="search-input" />
          <i v-if="filters.global.value" class="pi pi-times search-clear" @click="filters.global.value = ''" />
        </span>
      </div>
    </div>

    <!-- Tabla -->
    <DataTable :value="data" :paginator="true" :lazy="true" :rows="rows" :loading="loading" :rowHover="true"
      :totalRecords="totalRecords" :first="(currentPage - 1) * rows" :filters="filters"
      :globalFilterFields="['ltg_acto', 'ltg_Cedula_Demandante', 'ltg_Nombre_Demandante', 'estatus_Descripcion']"
      @page="onPageChange" class="p-datatable-sm" responsiveLayout="scroll">

      <Column field="ltg_acto" header="N⁰ Acto" />
      <Column field="ltg_Fecha_Acto" header="Fecha Acto" style="min-width: 110px;">
        <template #body="{ data }">
          {{ data.ltg_Fecha_Acto?.split('T')[0] || '' }}
        </template>
      </Column>
      <Column field="ltg_Cedula_Demandante" header="N⁰ Documento" />
      <Column field="ltg_Nombre_Demandante" header="Nombre Demandante" />
      <Column field="tipoDemanda_Nombre" header="Tipo de Demanda" />
      <Column field="ltg_Fecha_Audiencia" header="Fecha Audiencia" style="min-width: 110px;">
        <template #body="{ data }">
          {{ data.ltg_Fecha_Audiencia ? new Date(data.ltg_Fecha_Audiencia).toLocaleString('es-ES', { hour12: false }) :
            'Sin fecha' }}
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

      <Column header="Acciones">
        <template #body="{ data }">
          <button class="btn btn-sm btn-hover"
            style="background-color: #003870; border-color: #003870; margin-right: 0.3rem;" v-tooltip="'Ver litigio'"
            @click="$router.push({ path: `/Detalles/${data.id_Ltg}` })">
            <i class="pi pi-eye white-icon"></i>
          </button>
        </template>
      </Column>
    </DataTable>
  </div>
</template>

<script setup>
import { ref, onMounted, computed, watch } from 'vue';
import { FilterMatchMode } from '@primevue/core/api';
import InputText from 'primevue/inputtext';
import Dropdown from 'primevue/dropdown';
import DataTable from 'primevue/datatable';
import Column from 'primevue/column';
import api from '@/utilities/api.js';

const rows = ref(10);
const data = ref([]);
const totalRecords = ref(0);
const currentPage = ref(1);
const loading = ref(false);
const estatusSeleccionado = ref(null);
let debounceTimeout = null;


const filters = ref({
  global: { value: null, matchMode: FilterMatchMode.CONTAINS }
});


watch(() => filters.value.global.value?.trim(), (nuevo, viejo) => {
  if (nuevo !== viejo) {
    if (debounceTimeout) clearTimeout(debounceTimeout);
    debounceTimeout = setTimeout(() => {
      currentPage.value = 1;
      cargarLitigios();
    }, 300);
  }
});


watch(estatusSeleccionado, () => {
  currentPage.value = 1;
  cargarLitigios();
});

function onPageChange(event) {
  currentPage.value = Math.floor(event.first / event.rows) + 1;
  rows.value = event.rows;
  cargarLitigios();
}


async function cargarLitigios() {
  loading.value = true;
  const termino = filters.value.global.value?.trim();

  try {
    const { data: response } = await api.get('/api/Litigio/SeguimientoPaginado', {
      params: {
        page: currentPage.value,
        pageSize: rows.value,
        search: termino || null,
        estatus: estatusSeleccionado.value || null  // 👈 aquí agregamos el estatus
      }
    });

    data.value = response.litigios;
    totalRecords.value = response.total;
  } catch (error) {
    console.error('Error al cargar litigios paginados:', error);
  } finally {
    loading.value = false;
  }
}


const estatusDisponibles = [
  { label: 'Todos', value: null },
  { label: 'Recibido', value: 'Recibido' },
  { label: 'Análisis', value: 'Análisis' },
  { label: 'En Tribunal', value: 'En Tribunal' },
  { label: 'Sentencia', value: 'Sentencia' },
  { label: 'Recurso de Casación', value: 'Recurso de Casación' },
  { label: 'Cierre del Caso', value: 'Cierre del Caso' }
];


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

onMounted(cargarLitigios);
</script>




<style scoped>
.white-icon {
  color: white !important;
}

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

::v-deep(.p-datatable .p-datatable-tbody > tr > td),
::v-deep(.p-datatable .p-datatable-thead > tr > th) {
  border-right: 1px solid #ebebeb;
}

::v-deep(.p-datatable .p-datatable-thead > tr > th) {
  background-color: rgb(241, 242, 250)
}

.custom-header-center {
  text-align: center;
  font-weight: 600;
  color: #2e3842;
  font-size: 1rem;
  display: block;
  width: 100%;
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
  background-color: #d5f1c4;
  color: #47ac6d;
}

.filtro-busqueda-bar {
  display: flex;
  align-items: center;
  gap: 1rem;
  flex-wrap: wrap;
  margin-left: auto;
}

.search-container,
.search-input-wrapper {
  position: relative;
  display: block;
}

.search-input {
  width: 100%;
  padding: 0.5rem 2rem 0.5rem 2rem;
  border-radius: 6px;
  border: 1px solid #ccc;
}

.search-icon {
  position: absolute;
  left: 10px;
  top: 50%;
  transform: translateY(-50%);
  color: #888;
}

.search-clear {
  position: absolute;
  right: 10px;
  top: 50%;
  transform: translateY(-50%);
  color: #888;
  cursor: pointer;
}
</style>
