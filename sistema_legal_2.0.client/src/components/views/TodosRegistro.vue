<template>
  <div class="card p-6 shadow-2">
    <div class="flex justify-content-between align-items-center mb-2">
      <h2 class="text-2xl font-semibold">Registros</h2>
      <surely-table
    :columns="columns"
    :dataSource="data"
    rowKey="id"
  />
    </div>

   <div class="table-responsive">
      <table id="vistaLirigios" class="table table-bordered text-center" cellspacing="0" width="100%">
        <thead>
          <tr>
            <th>No.Acto</th>
            <th>Cedula del demandante</th>
            <th>Nombre del demandante</th>
            <th>Nombre representante</th>
            <th>Fecha del acto</th>
            <th>Documentos</th>
            <th>Estatus actual</th>
            <th>Ver</th>
          </tr>
        </thead>
        <tbody>
          <tr>
            <td>Tiger Nixon</td>
            <td>000000001</td>
            <td>José Gómez</td>
            <td>Ana Pérez</td>
            <td>24/03/2025</td>
            <td>documento.pdf</td>
            <td>En curso</td>

            <td>
              <router-link to="/drawer/modal"><button @click="showModal" class="btn btn-sm btn-primary">
                <i class="pi pi-eye"></i></button> </router-link>

          </td>
         </tr>
        </tbody>
      </table>



    </div>

    <a id="back-to-top" href="#" class="btn btn-primary btn-lg back-to-top" role="button" title="Volver arriba">
      <span class="glyphicon glyphicon-chevron-up"></span>
    </a>







  </div>






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




<style></style>
