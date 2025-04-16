<script type="text/javascript">

$(document).ready(function () {
  $('#tablaLirigios').DataTable();
});

</script>

<template>
  <div class="card p-6 shadow-2">
    <div class="flex justify-content-between align-items-center mb-2">
      <h2 class="text-2xl font-semibold">Registro de Litigio</h2>
    </div>
    <div class="container">
      <div class="row">
        <table id="tablaLirigios" class="table table-striped table-bordered" cellspacing="0" width="100%">
          <thead>

          </thead>
          <tfoot>

          </tfoot>
          <tbody>
          </tbody>
        </table>

        <a id="back-to-top" href="#" class="btn btn-primary btn-lg back-to-top" role="button"
          title="Click to return on the top page" data-toggle="tooltip" data-placement="left"><span
            class="glyphicon glyphicon-chevron-up"></span></a>

      </div>
    </div>
  </div>


</template>



<script setup>
import $ from 'jquery'
import 'datatables.net-bs5'
import 'datatables.net-bs5/css/dataTables.bootstrap5.min.css'
import { onMounted } from 'vue'

onMounted(() => {
  $('#tablaLirigios').DataTable({
    language: {
      url: '//cdn.datatables.net/plug-ins/1.13.6/i18n/es-ES.json'
    }
  });

  $(window).scroll(function () {
    if ($(this).scrollTop() > 50) {
      $('#back-to-top').fadeIn();
    } else {
      $('#back-to-top').fadeOut();
    }
  });

  $('#back-to-top').click(function () {
    $('body,html').animate({ scrollTop: 0 }, 800);
    return false;
  });
});



</script>
