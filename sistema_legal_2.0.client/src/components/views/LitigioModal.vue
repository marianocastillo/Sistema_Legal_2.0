<template>
  <VaModal
    v-model="visible"
    fullscreen
    :message="message"
    hide-default-actions
  >
  <div class="card p-6 shadow-2">
    <div class="flex justify-content-between align-items-center mb-2">
      <h2 class="text-2xl font-semibold">Registros</h2>
      <surely-table
    :columns="columns"
    :dataSource="data"
    rowKey="id"
  />
    </div>

    <div class="table-container">
      <div class="table-responsive">
        <table id="vistaLirigios" class="table table-bordered text-center" cellspacing="0" width="100%">
          <thead>
            <tr>
              <th class="sticky-column">No.Acto</th>
              <th>Cédula demandante</th>
              <th>Nombre demandante</th>
              <th>Cédula demandado</th>
              <th>Nombre demandado</th>
              <th>Representante legal</th>
              <th>Tipo de proceso</th>
              <th>Número de expediente</th>
              <th>Fecha acto</th>
              <th>Hora acto</th>
              <th>Juzgado</th>
              <th>Documentos</th>
              <th>Estatus</th>
              <th>Observaciones</th>
              <th>Acciones</th>
            </tr>
          </thead>
          <tbody>
            <tr>
              <td class="sticky-column">ACT-2023-001</td>
              <td>V-12345678</td>
              <td>María Rodríguez</td>
              <td>V-87654321</td>
              <td>Carlos Pérez</td>
              <td>Dr. Juan González</td>
              <td>Civil</td>
              <td>EXP-2023-001</td>
              <td>15/04/2023</td>
              <td>09:00 AM</td>
              <td>Primero de lo Civil</td>
              <td>
                <a href="#" class="badge bg-primary">documento1.pdf</a>
                <a href="#" class="badge bg-secondary">anexo.pdf</a>
              </td>
              <td><span class="badge bg-warning text-dark">En proceso</span></td>
              <td>Primera audiencia</td>
              <td>
                <div class="btn-group">
                  <router-link to="/drawer/modal" class="btn btn-sm btn-primary">
                    <i class="pi pi-eye"></i>
                  </router-link>
                  <button class="btn btn-sm btn-info">
                    <i class="pi pi-pencil"></i>
                  </button>
                  <button class="btn btn-sm btn-danger">
                    <i class="pi pi-trash"></i>
                  </button>
                </div>
              </td>
            </tr>
            <tr>
              <td class="sticky-column">ACT-2023-002</td>
              <td>V-11223344</td>
              <td>Pedro Martínez</td>
              <td>V-44332211</td>
              <td>Ana López</td>
              <td>Dra. Luisa Fernández</td>
              <td>Mercantil</td>
              <td>EXP-2023-045</td>
              <td>20/04/2023</td>
              <td>02:30 PM</td>
              <td>Tercero de lo Mercantil</td>
              <td>
                <a href="#" class="badge bg-primary">demanda.pdf</a>
              </td>
              <td><span class="badge bg-success">Finalizado</span></td>
              <td>Sentencia ejecutoriada</td>
              <td>
                <div class="btn-group">
                  <router-link to="/drawer/modal" class="btn btn-sm btn-primary">
                    <i class="pi pi-eye"></i>
                  </router-link>
                  <button class="btn btn-sm btn-info">
                    <i class="pi pi-pencil"></i>
                  </button>
                  <button class="btn btn-sm btn-danger">
                    <i class="pi pi-trash"></i>
                  </button>
                </div>
              </td>
            </tr>
            <!-- Puedes agregar más filas según necesites -->
          </tbody>
        </table>
      </div>
    </div>

    <a id="back-to-top" href="#" class="btn btn-primary btn-lg back-to-top" role="button" title="Volver arriba">
      <span class="glyphicon glyphicon-chevron-up"></span>
    </a>
  </div>
  </VaModal>
</template>

<script setup>
import { onMounted, ref } from 'vue'
import $ from 'jquery'
import 'datatables.net-bs5'
import 'datatables.net-bs5/css/dataTables.bootstrap5.min.css'
import ViewRegistro from '@/components/views/LitigioModal.vue'

const modalRef = ref(null)

function showModal() {
  modalRef.value?.open()
}

onMounted(() => {
  $('#vistaLirigios').DataTable({
    autoWidth: true,
    ordering: false,
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
  background-color: white;
  z-index: 1;
  min-width: 100px; /* Ancho mínimo para la columna fija */
}

thead th.sticky-column {
  z-index: 3; /* Mayor z-index para el encabezado */
  background-color: #f8f9fa; /* Color de fondo del header */
}

/* Estilos para badges */
.badge {
  margin: 2px;
  font-size: 0.75rem;
}

/* Estilos para los botones de acción */
.btn-group .btn {
  margin: 0 2px;
}

/* Scrollbar personalizada */
.table-responsive::-webkit-scrollbar {
  height: 8px;
  width: 8px;
}

.table-responsive::-webkit-scrollbar-thumb {
  background: #adb5bd;
  border-radius: 4px;
}

.table-responsive::-webkit-scrollbar-track {
  background: #f1f1f1;
}
</style>

