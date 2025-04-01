<template>
  <div>
    <LoginView v-if="!isAuthenticated" @loginSuccess="handleLoginSuccess" />
    <div v-else>
      <!-- <Drawer  /> -->
      <router-view />
    </div>
  </div>

</template>

<script>

// import Drawer  from './layouts/Drawer.vue';

export default {
  name: 'App',
  // components: {  Drawer  },
  data() {
    return {
      isAuthenticated: !!localStorage.getItem("token"),
    };
  },
  methods: {
    handleLoginSuccess() {
      this.isAuthenticated = true;
      this.$router.push('/drawer/home'); // Redirige correctamente
    }
  }
};
</script>
