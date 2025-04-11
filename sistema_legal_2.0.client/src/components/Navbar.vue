<template>
  <header class="navbar">
    <!-- Botón hamburguesa solo visible en pantallas pequeñas -->
    <div class="menu-placeholder">
      <button class="menu-button" @click="$emit('toggle-sidebar')" aria-label="Abrir menú">
        <i class="pi pi-bars" />
      </button>
    </div>

    <!-- Título centrado -->
    <h1 class="navbar-title">Expedientes</h1>

    <!-- Perfil del usuario -->
    <div class="user-profile" v-if="usuario.nombre">
      <!-- <img src="/user-avatar.png" alt="User" class="avatar" /> -->
      <div class="user-info">
        <div class="user-name">{{ usuario.nombre }}</div>
        <div class="user-role">{{ usuario.rol }}</div>

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
  justify-content: space-between;
  padding: 0 1rem;
  position: relative;
}

.menu-placeholder {
  width: 32px;
  height: 32px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.menu-button {
  background: none;
  border: none;
  font-size: 1.3rem;
  color: #444;
  cursor: pointer;
  display: none;
}

@media (max-width: 768px) {
  .menu-button {
    display: block;
  }
}

.navbar-title {
  position: absolute;
  left: 50%;
  transform: translateX(-50%);
  font-weight: 500;
  font-size: 1rem;
  color: #333;
}

.user-profile {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.avatar {
  width: 32px;
  height: 32px;
  border-radius: 50%;
  object-fit: cover;
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
