<template>
  <Toast /> <!-- Notificaciones globales -->

  <!-- Vista de login sin layout -->
  <template v-if="esLogin">
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
import { computed } from 'vue'
import LoginView from './components/views/LoginView.vue'
import Toast from 'primevue/toast'

// No necesitas export default en <script setup>
// Los componentes importados están disponibles automáticamente
const route = useRoute()
const router = useRouter()
const esLogin = computed(() => route.path === '/login')

const handleLoginSuccess = () => {
  localStorage.setItem('token', 'true')
  router.push('/drawer/home')
}
</script>
