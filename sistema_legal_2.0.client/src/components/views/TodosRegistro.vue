
<template>
  <div class="card p-6 shadow-2">
    <div class="flex justify-content-between align-items-center mb-2">
      <h2 class="text-2xl font-semibold">Registros</h2>
    </div>

    <!-- Contenedor con scroll horizontal -->
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
  <button class="btn btn-sm btn-danger custom-blue">
    <i class="pi pi-eye"></i>
  </button>
</td>


          </tr>
        </tbody>
      </table>
    </div>

    <a id="back-to-top" href="#" class="btn btn-primary btn-lg back-to-top" role="button"
      title="Volver arriba">
      <span class="glyphicon glyphicon-chevron-up"></span>
    </a>
  </div>
</template>


<script setup>
import $ from 'jquery'
import 'datatables.net-bs5'
import 'datatables.net-bs5/css/dataTables.bootstrap5.min.css'
import { onMounted } from 'vue'

onMounted(() => {
  $('#vistaLirigios').DataTable({
    autoWidth: true,
    ordering: false,
    language: {
      url: '//cdn.datatables.net/plug-ins/1.13.4/i18n/es-ES.json'
    },
    columnDefs: [
      {
        targets: '_all', // aplica a todas las columnas
        className: 'text-center' // clase de Bootstrap para centrar texto
      }
    ]
  })

  $(window).scroll(function () {
    if ($(this).scrollTop() > 50) {
      $('#back-to-top').fadeIn()
    } else {
      $('#back-to-top').fadeOut()
    }
  })
})
</script>




<style>


</style>
