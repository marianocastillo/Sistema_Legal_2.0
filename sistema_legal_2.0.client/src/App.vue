<template>
  <Toast /> <!-- Notificaciones globales -->

  <!-- Vista de login sin layout -->
  <template v-if="route.path === '/login'">
    <LoginView @loginSuccess="handleLoginSuccess" />
  </template>

  <!-- Resto de la app con layout -->
  <template v-else>
    <div class="min-h-screen surface-ground flex">
      <Drawer />
      <div class="flex flex-column flex-grow-1">
        <Navbar />
        <main class="flex-grow-1 p-4">
          <router-view />
        </main>
        <Footer />
      </div>
    </div>
  </template>
</template>

<script setup>
import { useRoute, useRouter } from 'vue-router'
import Drawer from './layouts/Drawer.vue' 
import Navbar from './components/Navbar.vue'
import Footer from './components/Footer.vue'
import LoginView from './views/LoginView.vue'
import Toast from 'primevue/toast'

const route = useRoute()
const router = useRouter()

const handleLoginSuccess = () => {
  localStorage.setItem('token', 'true')
  router.push('/drawer/home')
}
</script>
