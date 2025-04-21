<template>
  <router-view />

  <!-- Notivue global -->
  <Notivue v-slot="item">
    <NotivueSwipe :item="item">
      <Notifications :item="item" />
    </NotivueSwipe>
  </Notivue>
</template>

<script setup>
import { Notivue, NotivueSwipe, Notifications } from 'notivue'
</script>
