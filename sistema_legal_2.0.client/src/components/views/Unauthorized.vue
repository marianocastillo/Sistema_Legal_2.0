<template>
  <div class="unauthorized-page">
    <h1>403 - Acceso no autorizado</h1>
    <p>No tienes permisos para acceder a esta página.</p>

    <router-link :to="rutaInicio">
      <button class="back-btn">Volver al inicio</button>
    </router-link>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';

const rutaInicio = ref('/drawer/home');

onMounted(() => {
  const rawUser = localStorage.getItem('usuario');
  const user = rawUser ? JSON.parse(rawUser) : null;

  if (user && parseInt(user.perfil) === 4) {
    rutaInicio.value = '/drawer/abogado/inicio';
  }
});
</script>

<style scoped>
.unauthorized-page {
  text-align: center;
  margin-top: 100px;
  font-family: Arial, sans-serif;
}

.unauthorized-page h1 {
  font-size: 2.5rem;
  color: #c0392b;
  margin-bottom: 1rem;
}

.unauthorized-page p {
  font-size: 1.2rem;
  margin-bottom: 2rem;
}

.back-btn {
  padding: 10px 20px;
  font-size: 1rem;
  background-color: #003870;
  color: white;
  border: none;
  border-radius: 6px;
  cursor: pointer;
}

.back-btn:hover {
  background-color: #0055a4;
}
</style>
