<!-- eslint-disable vue/multi-word-component-names -->
<template>
  <footer class="main-footer d-print-none">
    <strong>Copyright &copy; {{actualYear}} <a href="https://www.contraloria.gob.do/">Contraloría General de la República</a>.</strong>
    All rights reserved.
    <div class="float-right d-none d-sm-inline-block">
      <b>Version</b> 1.2.2
    </div>
  </footer>
</template>

<script>
export default {
  data() {
    return {
      actualYear: new Date().getFullYear()
    };
  }
}
</script>

<style scoped>
  .main-footer {
    font-size: 0.8rem;
  }

  a {
    color: #003876 !important;
  }
</style>
