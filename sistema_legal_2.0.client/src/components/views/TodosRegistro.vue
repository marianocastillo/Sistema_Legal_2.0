<template>
  <div class="card p-6 shadow-2">
    <div class="flex justify-content-between align-items-center mb-4">
      <h2 class="text-2xl font-semibold">Registros</h2>

      <div class="flex justify-content-end">
        <span class="p-input-icon-left">
          <InputText v-model="filters.global.value" placeholder="Buscar..." />
        </span>
      </div>
    </div>

    <DataTable :value="data" :paginator="true" :rows="5" :filters="filters"
      :globalFilterFields="['ltg_acto', 'ltg_Cedula_Demandante', 'ltg_Fecha_Acto' ,'ltg_Demandante']" class="p-datatable-sm"
      responsiveLayout="scroll">
      <Column field="ltg_acto" header="No.Acto" />
      <Column field="ltg_Fecha_Acto" header="Fecha acto">
        <template #body="{ data }">
          {{ data.ltg_Fecha_Acto?.split('T')[0] || '' }}
        </template>
      </Column>
      <Column field="ltg_Cedula_Demandante" header="Cédula demandante" />
      <Column field="ltg_Demandante" header="Nombre demandante" />
      <Column field="tipoDemanda_Nombre" header="Tipo de Demanda" />
      <Column field="ltg_Tipo_Demandante" header="Tipo de Demandante" />
      <Column field="ltg_Fecha_Audiencia" header="Fecha audiencia">
        <template #body="{ data }">
          {{ data.ltg_Fecha_Audiencia?.split('T')[0] || 'Sin fecha' }}
        </template>
      </Column>
      <Column header="Acciones" style="width: 140px">
        <template #body="{ data }">
          <div class="btn-group">
            <router-link :to="`/litigio/detalle/${data.id_Ltg}`" class="btn btn-sm"
              style="background-color: #003870; border-color: #003870;">
              <i class="pi pi-eye white-icon"></i>
            </router-link>
          </div>
        </template>
      </Column>
    </DataTable>
  </div>
</template>

<script setup>
import { FilterMatchMode } from '@primevue/core/api';


import { ref, onMounted } from 'vue';
import api from '@/utilities/api.js';
import DataTable from 'primevue/datatable';
import Column from 'primevue/column';
import InputText from 'primevue/inputtext';

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
</script>

<style scoped>
.white-icon {
  color: white !important;
}

.btn-group .btn {
  margin: 0 2px;
}
</style>
