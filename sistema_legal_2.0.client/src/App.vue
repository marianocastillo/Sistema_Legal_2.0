<template>
  <Toast /> <!-- Notificaciones globales -->

  <!-- Vista de login sin layout -->
  <template v-if="route.path === '/login'">
    <LoginView @loginSuccess="handleLoginSuccess" />
  </template>

  <!-- Resto de la app con layout -->
  <template v-else>
    <div class="min-h-screen surface-ground flex">
      <div class="flex flex-column flex-grow-1">


        <main class="flex-grow-1 p-4">
          <router-view />
        </main>

      </div>
    </div>
  </template>
</template>

<script setup>
import { useRoute, useRouter } from 'vue-router'

//import Navbar from './components/Navbar.vue'
// import Footer from './components/Footer.vue'
import LoginView from './components/views/LoginView.vue'
import Toast from 'primevue/toast'

const route = useRoute()
const router = useRouter()

const handleLoginSuccess = () => {
  localStorage.setItem('token', 'true')
  router.push('/drawer/home')
}
</script>
