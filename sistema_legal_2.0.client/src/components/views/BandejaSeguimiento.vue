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
    <DataTable :value="litigiosFiltrados" :paginator="true" :rowHover="true" :rows="rows" :filters="filters" :globalFilterFields="[
      'ltg_acto',
      'ltg_Cedula_Demandante',
      'ltg_Nombre_Demandante',
      'estatus_Descripcion'
    ]" class="p-datatable-sm" responsiveLayout="scroll">
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
          <router-link :to="`/litigio/detalle/${data.id_Ltg}`" class="btn btn-sm"
            style="background-color: #003870; border-color: #003870" title="Ver litigio">
            <i class="pi pi-eye white-icon"></i>
          </router-link>
        </template>
      </Column>
    </DataTable>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue';
import { FilterMatchMode } from '@primevue/core/api';
import InputText from 'primevue/inputtext';
import Dropdown from 'primevue/dropdown';
import DataTable from 'primevue/datatable';
import Column from 'primevue/column';
import api from '@/utilities/api.js';

const rows = ref(10);
const todosLosLitigios = ref([]);
const filters = ref({
  global: { value: null, matchMode: FilterMatchMode.CONTAINS }
});

const estatusSeleccionado = ref(null);
const estatusDisponibles = [
  { label: 'Todos', value: null },
  { label: 'Recibido', value: 'Recibido' },
  { label: 'Análisis', value: 'Análisis' },
  { label: 'En Tribunal', value: 'En Tribunal' },
  { label: 'Sentencia', value: 'Sentencia' },
  { label: 'Recurso de Casación', value: 'Recurso de Casación' },
  { label: 'Cierre del Caso', value: 'Cierre del Caso' }
];

const litigiosFiltrados = computed(() => {
  if (!estatusSeleccionado.value) return todosLosLitigios.value;
  return todosLosLitigios.value.filter(
    l => l.estatus_Descripcion?.toLowerCase().trim() === estatusSeleccionado.value.toLowerCase().trim()
  );
});

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

onMounted(async () => {
  try {
    const response = await api.get('/api/Litigio/Litigio_detallado');
    todosLosLitigios.value = response.data;
  } catch (error) {
    console.error('Error al cargar litigios:', error);
  }
});
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
