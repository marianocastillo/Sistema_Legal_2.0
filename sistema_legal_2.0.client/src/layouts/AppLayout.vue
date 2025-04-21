<template>
  <div class="min-h-screen surface-ground flex">
    <SidebarMenu />
    <div class="flex flex-column flex-grow-1">
      <TopNavbar />
      <main class="flex-grow-1 p-4">
        <router-view />
      </main>
      <AppFooter />
    </div>
  </div>
</template>

<script setup>
import SidebarMenu from '@/components/SidebarMenu.vue'
import TopNavbar from '@/components/TopNavbar.vue'
import AppFooter from '@/components/AppFooter.vue'
</script>
