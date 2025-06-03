<template>
  <div class="card p-6 shadow-2">
    <div class="flex justify-content-between align-items-center mb-4 flex-wrap gap-2">
      <h2 class="text-2xl font-semibold">Gestión de Litigios</h2>

      <div class="flex align-items-center gap-3">
        <Dropdown v-model="filtroSeleccionado" :options="opcionesFiltro" optionLabel="label" optionValue="value"
                  placeholder="Filtrar por estado" class="w-full md:w-16rem" />
      </div>
    </div>

    <DataTable :value="data" :paginator="true" :rows="7" :filters="filters"
               :globalFilterFields="['ltg_acto', 'ltg_Cedula_Demandante', 'ltg_Fecha_Acto', 'ltg_Demandante']"
               class="p-datatable-sm" responsiveLayout="scroll">

      <Column field="ltg_acto" header="No.Acto" />
      <Column field="ltg_Fecha_Acto" header="Fecha acto">
        <template #body="{ data }">
          {{ data.ltg_Fecha_Acto?.split('T')[0] || '' }}
        </template>
      </Column>
      <Column field="ltg_Cedula_Demandante" header="Cédula demandante" />
      <Column field="ltg_Demandante" header="Nombre demandante" />
      <Column field="nombre_Tipo_Demanda" header="Tipo de Demanda" />
      <Column field="ltg_Fecha_Audiencia" header="Fecha audiencia">
        <template #body="{ data }">
          {{ data.ltg_Fecha_Audiencia?.split('T')[0] || 'Sin fecha' }}
        </template>
      </Column>
      <Column field="ltg_description" header="Estatus" />

      <Column header="Acciones" style="width: 140px">
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
import { ref, onMounted, watch } from 'vue';
import { FilterMatchMode } from '@primevue/core/api';
import Dropdown from 'primevue/dropdown';
import DataTable from 'primevue/datatable';
import Column from 'primevue/column';
// import InputText from 'primevue/inputtext';
import api from '@/utilities/api.js';
import AsignarAbogado from '@/components/views/AsignarAbogado.vue';

const data = ref([]);
const filters = ref({
  global: { value: null, matchMode: FilterMatchMode.CONTAINS }
});

const filtroSeleccionado = ref('sinAsignar');
const opcionesFiltro = [
  { label: 'Sin asignar', value: 'sinAsignar' },
  { label: 'Asignados', value: 'asignados' }
];

const popUp = ref(false);
const litigioActual = ref({});
const mostrarAsignar = ref(true);

function togglePopUp(litigio = null) {
  popUp.value = !popUp.value;
  litigioActual.value = litigio || {};
}

const cargarLitigios = async () => {
  try {
    if (filtroSeleccionado.value === 'sinAsignar') {
      const response = await api.get('/api/Litigio/Litigio_detallado');
      data.value = response.data;
      mostrarAsignar.value = true;
    } else {
      const idSupervisor = localStorage.getItem('idUsuario');
      const response = await api.get(`/api/Litigio/LitigiosAsignadosSupervisor?idSupervisor=${idSupervisor}`);
      data.value = response.data;
      mostrarAsignar.value = true;
    }
  } catch (error) {
    console.error('Error al cargar litigios:', error);
  }
};

onMounted(cargarLitigios);
watch(filtroSeleccionado, cargarLitigios);
</script>

<style scoped>
.white-icon {
  color: white !important;
}

.btn-group .btn {
  margin: 0 2px;
}
</style>
