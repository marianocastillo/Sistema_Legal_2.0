<template>
    <div class="page-loader" v-if="$store.state.loading">
      <ProgressSpinner style="width: 50px; height: 50px" strokeWidth="4" animationDuration=".5s" />
    </div>
</template>

<script>
export default {
  data() {
    return {     
    };
  }
}
</script>

<style>
.page-loader {
  background-color: rgba(0, 0, 0, 0.5);
  -webkit-backdrop-filter: blur(5px);
  backdrop-filter: blur(5px);
  padding: 50px;
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  z-index: 9999;
  display: flex;
  justify-content: center;
  align-items: center;
  user-select: none;
  opacity: 1;
  transition: opacity 0.5s ease-in-out;
}

@keyframes p-progress-spinner-color {

  100%, 0% {
    stroke: #dc2626;
  }
  40% {
    stroke: #044896;
  }
  66% {
    stroke: #970e0e;
  }
  80%, 90% {
    stroke: #003876;
  }
}
</style>