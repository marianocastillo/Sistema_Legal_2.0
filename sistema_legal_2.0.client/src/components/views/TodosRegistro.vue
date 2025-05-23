<template>
  <div class="card p-6 shadow-2">
    <div class="flex justify-content-between align-items-center mb-4 flex-wrap gap-2">
      <h2 class="text-2xl font-semibold">Registros</h2>

      <div class="search-container">
        <span class="p-input-icon-left search-input-wrapper">
          <i class="pi pi-search search-icon" />
          <InputText v-model="filters.global.value" placeholder="Buscar litigio..." class="search-input" />
          <i v-if="filters.global.value" class="pi pi-times search-clear" @click="filters.global.value = ''" />
        </span>
      </div>

    </div>

    <DataTable :value="data" :paginator="true" :rows="5" :filters="filters"
      :globalFilterFields="['ltg_acto', 'ltg_Cedula_Demandante', 'ltg_Fecha_Acto', 'ltg_Demandante', 'estatus_Descripcion']"
      class="p-datatable-sm" responsiveLayout="scroll">
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


      <Column header="Acciones">
        <template #body="{ data }">
          <div class="btn-group">
            <router-link :to="`/litigio/detalle/${data.id_Ltg}`" class="btn btn-sm"
              style="background-color: #003870; border-color: #003870; margin-right: 0.3rem;">
              <i class="pi pi-eye white-icon"></i>
            </router-link>

            <button class="btn btn-sm" @click="togglePopUp(data)"
              style="background-color: #003870; border-color: #003870;">
              <i class="pi pi-user-edit white-icon" title="Asignar abogado"></i>
            </button>
          </div>
        </template>
      </Column>

      <teleport to="body">
        <transition name="fade">
          <AsignarAbogado v-if="popUp" :id_Ltg="litigioActual.id_Ltg" :ltg_acto="litigioActual.ltg_acto"
            @close="togglePopUp" />
        </transition>
      </teleport>
    </DataTable>
  </div>
</template>

<script setup>
import { FilterMatchMode } from '@primevue/core/api';
import AsignarAbogado from '@/components/views/AsignarAbogado.vue'; // cambia la ruta si es necesario

import { ref, onMounted } from 'vue';
import api from '@/utilities/api.js';
import DataTable from 'primevue/datatable';
import Column from 'primevue/column';
import InputText from 'primevue/inputtext';

const popUp = ref(false);
const litigioActual = ref({});

function togglePopUp(litigio = null) {
  popUp.value = !popUp.value;
  litigioActual.value = litigio || {};
}

const data = ref([]);
const filters = ref({
  global: { value: null, matchMode: FilterMatchMode.CONTAINS }
});

onMounted(async () => {
  try {
    const response = await api.get('/api/Litigio/Litigio_detallado');
    data.value = response.data;
  } catch (error) {
    console.error('Error al cargar los litigios:', error);
  }
});

// Función para determinar la severidad del estatus
function getStatusClass(status) {
  switch (status) {
    case 'Recibido':
      return 'status-recibido';
    case 'Análisis':
      return 'status-analisis';
    case 'En Tribunal':
      return 'status-tribunal';
    case 'Sentencia':
    case 'Sentencia Definitiva':
      return 'status-sentencia';
    case 'Recurso de Casación':
      return 'status-casacion';
    case 'Cierre del Caso':
      return 'status-cierre';
    default:
      return '';
  }
}

</script>


<style scoped>
/* ::v-deep(.p-datatable thead th:nth-child(7)) {
  text-align: center !important;
} */

.white-icon {
  color: white !important;
}

.btn-group .btn {
  margin: 0 2px;
}


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
  /* top right bottom left */
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

.tag {
  padding: 0.3rem 0.75rem;
  width: 100%;
  border-radius: 9999px;
  font-size: 0.85rem;
  font-weight: 600;
  display: inline-block;
  text-align: center;
}

.status-recibido {
  background-color: #c2e4fc;
  color: #0d47a1;
}

.status-analisis {
  background-color: #9cdbff;
  color: #276264;
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
  background-color: #c8e6c9;
  color: #2e7d32;
}

.custom-header-center {
  text-align: center !important;
  font-weight: 600;
  color: #2e3842;
  font-size: 1rem;
  display: block;
  width: 100%;
}

</style>
