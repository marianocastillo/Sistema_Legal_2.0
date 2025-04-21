<template>
  <header class="navbar">
    <!-- Contenedor flexible con espacio entre título y perfil -->
    <div class="navbar-inner">
      <!-- Espacio reservado a la izquierda para equilibrar -->
      <div class="spacer"></div>

      <!-- Título centrado -->
      <h1 class="navbar-title">Expedientes</h1>

      <!-- Perfil del usuario a la derecha -->
      <div class="user-profile" v-if="usuario.nombre">
        <div class="user-info">
          <div class="user-name">{{ usuario.nombre }}</div>
          <div class="user-role">{{ usuario.rol }}</div>
        </div>
      </div>
    </div>
  </header>
</template>

<script setup>
import { ref, onMounted } from 'vue'

const usuario = ref({ nombre: '', rol: '' })

onMounted(() => {
  const stored = localStorage.getItem('usuario')
  if (stored) {
    usuario.value = JSON.parse(stored)
  }
})
</script>

<style scoped>
.navbar {
  height: 64px;
  background-color: #fff;
  border-bottom: 1px solid #ddd;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 0 1rem;
  position: relative;
}

/* Flex interno para organizar título y perfil */
.navbar-inner {
  display: flex;
  align-items: center;
  justify-content: space-between;
  width: 100%;
  max-width: 1200px;
  position: relative;
}

/* Espaciador para mantener el centro visual del título */
.spacer {
  width: 120px; /* mismo ancho estimado del perfil para equilibrio */
}

/* Título centrado dentro del navbar-inner */
.navbar-title {
  font-weight: 600;
  font-size: 1.2rem;
  text-align: center;
  flex-grow: 1;
  color: #333;
}

/* Perfil alineado a la derecha */
.user-profile {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.user-info {
  display: flex;
  flex-direction: column;
  font-size: 0.75rem;
  text-align: right;
}

.user-name {
  font-weight: bold;
}

.user-role {
  color: #888;
  font-size: 0.7rem;
}
</style>
