<template>
  <div class="card p-6 shadow-2">
    <div class="flex justify-content-between align-items-center mb-4 flex-wrap gap-2">
      <h2 class="text-2xl font-semibold">Litigios Asignados</h2>

      <div class="search-container">
        <span class="p-input-icon-left search-input-wrapper">
          <i class="pi pi-search search-icon" />
          <InputText v-model="filters.global.value" placeholder="Buscar litigio..." class="search-input" />
          <i v-if="filters.global.value" class="pi pi-times search-clear" @click="filters.global.value = ''" />
        </span>
      </div>
    </div>

    <DataTable :value="data" :paginator="true" :rowHover="true" :rows="rows" :filters="filters" :globalFilterFields="[
      'ltg_acto',
      'ltg_Cedula_Demandante',
      'ltg_Fecha_Acto',
      'ltg_Nombre_Demandante',
      'estatus_Descripcion'
    ]" class="p-datatable-sm" responsiveLayout="scroll">
      <Column field="ltg_acto" header="No.Acto" />
      <Column field="ltg_Fecha_Acto" header="Fecha acto" style="min-width: 110px;">
        <template #body="{ data }">
          {{ data.ltg_Fecha_Acto?.split('T')[0] || '' }}
        </template>
      </Column>
      <Column field="ltg_Cedula_Demandante" header="Cédula demandante" />
      <Column field="ltg_Nombre_Demandante" header="Nombre demandante" />
      <Column field="nombre_Tipo_Demanda" header="Tipo de Demanda" />
      <Column field="ltg_Fecha_Audiencia" header="Fecha audiencia">
        <template #body="{ data }">
          {{ data.ltg_Fecha_Audiencia?.split('T')[0] || 'Sin fecha' }}
        </template>
      </Column>
      <Column field="ltg_description">
        <template #header>
          <div class="custom-header-center">Estatus</div>
        </template>
        <template #body="slotProps">
          <div class="text-center">
            <span class="tag" :class="getStatusClass(slotProps.data.ltg_description)">
              {{ slotProps.data.ltg_description }}
            </span>
          </div>
        </template>
      </Column>
      <Column header="Acciones" >
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
import AsignarAbogado from '@/components/views/AsignarAbogado.vue';
import router from '@/router/router';
import { ref, onMounted, onUnmounted } from 'vue';
import api from '@/utilities/api.js';
import DataTable from 'primevue/datatable';
import Column from 'primevue/column';
import InputText from 'primevue/inputtext';

const popUp = ref(false);
const litigioActual = ref({});
const data = ref([]);
const filters = ref({
  global: { value: null, matchMode: FilterMatchMode.CONTAINS }
});
const rows = ref(10); // Número de filas a mostrar

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

function calculateRows() {
  const tableHeight = window.innerHeight - 300; // Ajusta según tu layout
  const estimatedRowHeight = 50;
  rows.value = Math.max(Math.floor(tableHeight / estimatedRowHeight) - 1, 1);
}



async function modificarLitigio(litigio) {
  try {
    const response = await api.get(`/api/Litigio/detallados/${litigio.id_Ltg}`);
    const litigioCompleto = response.data;

    if (!litigioCompleto) {
      throw new Error('No se encontró información del litigio.');
    }

    localStorage.setItem('litigioModificacion', JSON.stringify(litigioCompleto));
    router.push('/modificarregistro');
  } catch (error) {
    console.error('Error al cargar litigio desde el backend:', error);
    push.error('No se pudo cargar la información del litigio.');
  }
}

// async function modificarLitigio(litigio) {
//   try {
//     const response = await api.get(`/api/Litigio/Litigio_detallado`);
//     const litigioCompleto = response.data.find(l => l.id_Ltg === litigio.id_Ltg);

//     if (!litigioCompleto) {
//       throw new Error('Litigio no encontrado en el listado completo');
//     }

//     localStorage.setItem('litigioModificacion', JSON.stringify(litigioCompleto));
//     router.push('/modificarregistro');
//   } catch (error) {
//     console.error('Error al obtener litigio completo:', error);
//   }
// }



onMounted(async () => {
  calculateRows();
  window.addEventListener('resize', calculateRows);

  const idUsuario = localStorage.getItem('idUsuario');
  if (!idUsuario) return router.push('/login');

  try {
    const response = await api.get(`/api/Litigio/Litigio_Asignaciones?idUsuario=${idUsuario}`);
    data.value = response.data;
  } catch (error) {
    console.error('Error al cargar los litigios:', error);
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

::v-deep(.p-datatable .p-datatable-tbody > tr:nth-child(even)) {
  background-color: #f9fafb;
}

::v-deep(.p-datatable .p-datatable-thead > tr > th) {
  background-color: rgb(241, 242, 250)
}

::v-deep(.p-datatable .p-datatable-tbody > tr > td),
::v-deep(.p-datatable .p-datatable-thead > tr > th) {
  border-right: 1px solid #ebebeb;
}

::v-deep(.p-datatable .p-datatable-tbody > tr > td:last-child),
::v-deep(.p-datatable .p-datatable-thead > tr > th:last-child) {
  border-right: none;
}

.custom-header-center {
  text-align: center;
  font-weight: 600;
  color: #2e3842;
  font-size: 1rem;
  display: block;
  width: 100%;
}
</style>
