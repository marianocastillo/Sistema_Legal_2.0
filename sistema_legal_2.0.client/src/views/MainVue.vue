<template>
<div class="container">
  <SideBarVue></SideBarVue>
  <div class="content">
<router-view></router-view>

  </div>

</div>
</template>


<script setup>
import SideBarVue from '../components/SideBarVue.vue';
</script>


<style>
.container{
  display: flex;
}

.content{
  flex: 1;
  padding: 20px;
}
</style>
