<template>
  <div>
    <LoginView v-if="!isAuthenticated" @loginSuccess="handleLoginSuccess" />
    <div v-else>
      <Sidebar />
      
      <router-view />
    </div>
  </div>

</template>

<script>

import Sidebar from './layouts/Sidebar.vue';

export default {
  name: 'App',
  components: {  Sidebar },
  data() {
    return {
      isAuthenticated: !!localStorage.getItem("token"),
    };
  },
  methods: {
    handleLoginSuccess() {
      this.isAuthenticated = true;
      this.$router.push('/sidebar/home'); // Redirige correctamente
    }
  }
};
</script>
