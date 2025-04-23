<template>

  <div class="card p-6 shadow-2">
    <div class="flex justify-content-between align-items-center mb-2">
      <h2 class="text-2xl font-semibold">Registros</h2>

    </div>

    <div class="table-container">
      <div class="table-responsive">
        <table id="vistaLirigios" class="table table-bordered table-sm text-center" cellspacing="0" width="110%">
          <thead>
            <tr>
              <th class="sticky-column accent-column">No.Acto</th>
              <th>Fecha acto</th>
              <th>Cédula demandante</th>
              <th>Nombre demandante</th>
              <th>Cédula Representante</th>
              <th>Nombre Representante</th>
              <th>Tipo de Demanda</th>
              <th>Nacionalidad</th>
              <th>Tipo de Demandante</th>
              <th>Fecha audiencia</th>
              <th>Fecha actualizacion</th>
              <th>Tribunal</th>
              <th>Digitador</th>
              <th>Documentos</th>
              <th>Estatus</th>
              <th>Sentencia</th>
              <th>Acciones</th>
            </tr>
          </thead>
          <tbody>
  <tr v-for="litigio in data" :key="litigio.ltg_acto">
    <td class="sticky-column  accent-column fw-bold ">{{ litigio.ltg_acto }}</td>
    <td>{{ litigio.ltg_Fecha_Acto }}</td>
    <td>{{ litigio.ltg_Cedula_Demandante }}</td>
    <td>{{ litigio.ltg_Demandante }}</td>
    <td>{{ litigio.ltg_Cedula_Representante }}</td>
    <td>{{ litigio.ltg_Nombre_Representante }}</td>
    <td>{{ litigio.tipoDemanda_Nombre }}</td>
    <td>{{ litigio.ltg_Nacionalidad }}</td>
    <td>{{ litigio.ltg_Tipo_Demandante }}</td>
    <td>{{ litigio.ltg_Fecha_Audiencia }}</td>
    <td>{{ litigio.ltg_Fecha_Actualizacion }}</td>
    <td>{{ litigio.nombre_Tribunal }}</td>
    <td>{{ litigio.nombreUsuario }}</td>
    <td>{{ litigio.ruta_Nombre }}</td>
    <td>{{ litigio.estatus_Descripcion}}</td>
    <td>{{ litigio.desc_Sentencia }}</td>
    <td> <!-- Este es el TD que faltaba para la columna Acciones -->
      <div class="btn-group">
        <router-link @click="mostrarDetalle(litigio.ltg_acto)" to="/drawer/modal" class="btn btn-sm" style="background-color: #003870; border-color: #003870;">
  <i class="pi pi-eye white-icon"></i>
</router-link>
<router-link to="/drawer/modificarregistro" class="btn btn-sm" style="background-color: #003870; border-color: #003870;">
  <i class="pi pi-pencil white-icon"></i>
</router-link>
      </div>
    </td>
  </tr>
</tbody>
        </table>
      </div>
    </div>

    <a id="back-to-top" href="#" class="btn btn-primary btn-lg back-to-top" role="button" title="Volver arriba">
      <span class="glyphicon glyphicon-chevron-up"></span>
    </a>
  </div>

</template>

<script setup>
import { onMounted, ref, nextTick } from 'vue'
import $ from 'jquery'
import 'datatables.net-bs5'
import 'datatables.net-bs5/css/dataTables.bootstrap5.min.css'
import api from '@/utilities/api.js'
import LitigioMostrar from './LitigioMostrar.vue'// Asegúrate que este sea tu archivo api
// import ViewRegistro from '@/components/views/LitigioMostrar.vue'
import { left } from '@popperjs/core'

const detalleModal = ref(null)
const modalRef = ref(null)
const data = ref([])

const abrirDetalle = (litigioId) => {
  modalDetalle.value?.open(litigioId) // Pasa solo el ID
}

const mostrarDetalle = (litigio) => {
  detalleModal.value?.open(litigio)
}

onMounted(async () => {
  try {
    const response = await api.get('/api/Files/detallados') // Ajusta si tu endpoint cambia
    data.value = response.data

    await nextTick()

    $('#vistaLirigios').DataTable({
      autoWidth: true,
      ordering: false,
      paging: true,
      lengthChange: false,
      pageLength: 5,
      searching: true,
      searching: left,
      responsive: true,
      language: {
        url: '//cdn.datatables.net/plug-ins/1.13.4/i18n/es-ES.json'
      },
      columnDefs: [
        {
          targets: '_all',
          className: 'text-center'
        }
      ]
    })
  } catch (error) {
    console.error('Error al cargar litigios detallados:', error)
  }
})
</script>



<style scoped>
.table-container {
  position: relative;
  overflow-x: auto;
  -webkit-overflow-scrolling: touch;
  max-height: 600px; /* Altura máxima con scroll vertical */
}

.sticky-column {
  position: sticky;
  left: 0;
   z-index: 1;
  min-width: 120px; /* Ancho mínimo para la columna fija */
}

/* Efecto hover para mejor interactividad */


/* Si usas sticky, asegura que el color se mantenga */


/* Estilos para los botones de acción */
.btn-group .btn {
  margin: 0 2px;
}

/* Scrollbar personalizada */
/* .table-responsive::-webkit-scrollbar {
  height: 8px;
  width: 8px;
} */

.table-responsive::-webkit-scrollbar-thumb {
  background: #adb5bd;
  border-radius: 4px;
}

.table-responsive::-webkit-scrollbar-track {
  background: #f1f1f1;
}

.table-sm td,
.table-sm th {
  font-size: 0.8rem; /* Puedes ajustar el tamaño a tu gusto */
}

.white-icon {
  color: white !important;
}

.blue-icon {
  color: #003870 !important;
}
</style>


