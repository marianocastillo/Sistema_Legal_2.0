<!-- eslint-disable vue/multi-word-component-names -->
<template>
  <footer class="main-footer flex justify-content-between align-items-center">
    <div>
      <strong>Copyright &copy; 2024 <a href="https://www.contraloria.gob.do/">Contraloría General de la República</a>.</strong>
      All rights reserved.
    </div>
    <div>
      <b>Version</b> 0.1
    </div>
  </footer>
</template>

<script>
export default {
  data() {
    return {     
    };
  }
}
</script>

<style scoped>
  .main-footer {
    font-size: 0.8rem;
    padding: 10px;
    height: 45px;
    color: #3a3a3a
  }

  a {
    color: #003876 !important;
  }
</style>