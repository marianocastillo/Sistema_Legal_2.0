¡Buena idea! Aquí tienes una versión divertida de la página de Unauthorized (403) con un pequeño "juego estilo Dino de Google", usando solo Vue y JavaScript sin dependencias externas. Es un mini juego de salto para esquivar obstáculos 👇

✅ CÓDIGO COMPLETO
vue
Copiar
Editar
<template>
  <div class="unauthorized-page">
    <h1>403 - Acceso no autorizado</h1>
    <p>No tienes permisos para acceder a esta página.</p>

    <router-link :to="rutaInicio">
      <button class="back-btn">Volver al inicio</button>
    </router-link>

    <!-- Mini juego -->
    <div class="game-container">
      <div class="player" :class="{ jump: isJumping }"></div>
      <div class="obstacle" :style="{ left: obstacleLeft + 'px' }"></div>
    </div>
    <p v-if="gameOver" class="game-over-text">¡Perdiste! Presiona espacio para volver a intentarlo.</p>
  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted } from 'vue';

const rutaInicio = ref('/');
const isJumping = ref(false);
const obstacleLeft = ref(600);
const gameOver = ref(false);
let jumpTimeout = null;
let obstacleInterval = null;

onMounted(() => {
  const storedRuta = localStorage.getItem('rutaInicio');
  rutaInicio.value = storedRuta || '/';

  // Movimiento del obstáculo
  obstacleInterval = setInterval(() => {
    if (obstacleLeft.value > -20) {
      obstacleLeft.value -= 10;
    } else {
      obstacleLeft.value = 600;
    }

    // Colisión simple
    if (obstacleLeft.value < 50 && obstacleLeft.value > 0 && !isJumping.value) {
      gameOver.value = true;
      clearInterval(obstacleInterval);
    }
  }, 50);

  window.addEventListener('keydown', handleKeyDown);
});

onUnmounted(() => {
  clearInterval(obstacleInterval);
  window.removeEventListener('keydown', handleKeyDown);
});

function handleKeyDown(e) {
  if (e.code === 'Space') {
    if (gameOver.value) {
      restartGame();
    } else {
      jump();
    }
  }
}

function jump() {
  if (isJumping.value) return;
  isJumping.value = true;
  jumpTimeout = setTimeout(() => {
    isJumping.value = false;
  }, 500);
}

function restartGame() {
  obstacleLeft.value = 600;
  gameOver.value = false;
  isJumping.value = false;

  obstacleInterval = setInterval(() => {
    if (obstacleLeft.value > -20) {
      obstacleLeft.value -= 10;
    } else {
      obstacleLeft.value = 600;
    }

    if (obstacleLeft.value < 50 && obstacleLeft.value > 0 && !isJumping.value) {
      gameOver.value = true;
      clearInterval(obstacleInterval);
    }
  }, 50);
}
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
