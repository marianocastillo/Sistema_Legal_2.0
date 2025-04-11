<template>
  <div class="layout">
    <!-- Sidebar -->
    <aside class="sidebar">
      <div class="sidebar-top">
        <!-- Header -->
        <div class="sidebar-header">
          <div class="sidebar-brand">
            <img src="../assets/contraloria azul.png" alt="Logo" class="logo" />
            <h2 class="sidebar-title">Sistema de Litigio</h2>
          </div>

          <!-- Buscador -->
          <div class="sidebar-search">
            <span class="p-input-icon-left w-full block">
              <i class="pi pi-search" />
              <InputText v-model="search" placeholder="Buscar" class="w-full p-inputtext-sm" />
            </span>
          </div>
        </div>

        <!-- Navegación -->
        <nav class="sidebar-menu">
          <ul>
            <li>
              <router-link to="/drawer/home" class="sidebar-link">
                <i class="pi pi-home" />
                <span>Inicio</span>
              </router-link>
            </li>
            <li>
              <router-link to="/drawer/registro" class="sidebar-link">
                <i class="pi pi-file-edit" />
                <span>Registrar</span>
              </router-link>
            </li>
            <li>
              <router-link to="/drawer/edit" class="sidebar-link">
                <i class="pi pi-pencil" />
                <span>Modificar</span>
              </router-link>
            </li>
            <li ref="submenuRef">
              <div class="sidebar-link" @click="toggleSubmenu" style="cursor: pointer;">
                <i class="pi pi-cog" />
                <span>Configuración</span>
                <i class="pi pi-chevron-down ml-auto" :class="{ 'rotate-180': mostrarSubmenu }" />
              </div>
              <ul v-if="mostrarSubmenu" class="submenu">
                <li>
                  <Button
                    label="Cerrar sesión"
                    icon="pi pi-sign-out"
                    class="p-button-text p-button-sm w-full pl-4"
                    @click="cerrarSesion"
                  />
                </li>
              </ul>
            </li>
          </ul>
        </nav>
      </div>

      <!-- Footer -->
      <footer class="sidebar-footer">
        © 2025 Sistema Legal
      </footer>
    </aside>

    <!-- Contenido principal -->
    <main class="main-content">
      <router-view />
    </main>

    <!-- Notificaciones -->
    <Notivue v-slot="item">
      <NotivueSwipe :item="item">
        <Notifications :item="item" :theme="pastelTheme" />
      </NotivueSwipe>
    </Notivue>
  </div>
</template>

<script setup>
import { ref, onMounted, onBeforeUnmount } from 'vue'
import { useRouter } from 'vue-router'
import InputText from 'primevue/inputtext'
import Button from 'primevue/button'
import { Notivue, Notifications, NotivueSwipe, pastelTheme } from 'notivue'

const router = useRouter()
const search = ref('')
const mostrarSubmenu = ref(false)
const submenuRef = ref(null)

const toggleSubmenu = () => {
  mostrarSubmenu.value = !mostrarSubmenu.value
}

const handleClickOutside = (e) => {
  if (submenuRef.value && !submenuRef.value.contains(e.target)) {
    mostrarSubmenu.value = false
  }
}

onMounted(() => {
  document.addEventListener('click', handleClickOutside)
})

onBeforeUnmount(() => {
  document.removeEventListener('click', handleClickOutside)
})

const cerrarSesion = () => {
  localStorage.removeItem('usuario')
  router.push('/login')
}
</script>

<style scoped>
.layout {
  display: flex;
  min-height: 100vh;
}

.sidebar {
  width: 320px;
  background-color: #ffffff;
  border-right: 1px solid #ddd;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  padding: 1rem;
  box-shadow: 0 0 10px rgba(0, 0, 0, 0.05);
}

.main-content {
  flex: 1;
  padding: 1.5rem;
  background-color: #f9f9f9;
}

.sidebar-top {
  display: flex;
  flex-direction: column;
}

.sidebar-header {
  margin-bottom: 1rem;
}

.sidebar-brand {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  margin-bottom: 1rem;
}

.logo {
  width: 62px;
  height: 62px;
  object-fit: contain;
}

.sidebar-title {
  padding-top: 15px;
  font-size: 1.25rem;
  font-weight: bold;
}

.sidebar-search {
  margin-bottom: 1rem;
}

.sidebar-menu ul {
  list-style: none;
  padding: 0;
  margin: 0;
}

.sidebar-link {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.5rem 0.75rem;
  text-decoration: none;
  color: #333;
  border-radius: 6px;
  transition: background-color 0.2s ease;
}

.sidebar-link:hover {
  background-color: #f2f2f2;
}

.sidebar-link i {
  font-size: 1rem;
}

.sidebar-footer {
  font-size: 0.75rem;
  text-align: center;
  color: #999;
  border-top: 1px solid #eee;
  padding-top: 1rem;
}

.submenu {
  padding-left: 1.5rem;
  margin-top: 0.3rem;
}

.rotate-180 {
  transform: rotate(180deg);
  transition: transform 0.2s ease;
}
</style>
