<template>
  <aside class="sidebar">
    <!-- Contenido superior agrupado -->
    <div class="sidebar-top">
      <!-- Encabezado -->
      <div class="sidebar-header">
        <div class="sidebar-brand">
          <img src="../assets/contraloria azul.png" alt="Logo" class="logo" />
          <h2 class="sidebar-title">Registro de Litigio</h2>
        </div>

        <!-- Buscador -->
        <div class="sidebar-search">
          <span class="p-input-icon-left w-full block">
            <i class="pi pi-search" />
            <InputText v-model="search" placeholder="Buscar..." class="w-full custom-search-input" />
          </span>
        </div>
      </div>

      <!-- Menú -->
      <nav class="sidebar-menu">
        <ul>
          <li>
            <router-link to="/" class="sidebar-link">
              <i class="pi pi-home" />
              <span>Inicio</span>
            </router-link>
          </li>
          <li>
            <router-link to="/expedientes" class="sidebar-link red">
              <i class="pi pi-folder" />
              <span>Expedientes</span>
            </router-link>
          </li>
          <li class="sidebar-link badge-wrapper">
            <router-link to="/audiencias" class="sidebar-link">
              <i class="pi pi-calendar" />
              <span>Audiencias</span>
            </router-link>
            <span class="badge">3</span>
          </li>
          <li>
            <router-link to="/abogado" class="sidebar-link">
              <i class="pi pi-user" />
              <span>Abogado</span>
            </router-link>
          </li>
          <li>
            <router-link to="/perfiles" class="sidebar-link">
              <i class="pi pi-users" />
              <span>Perfiles</span>
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
</template>

<script setup>
import { ref, onMounted, onBeforeUnmount } from 'vue'
import { useRouter } from 'vue-router'
import InputText from 'primevue/inputtext'
import Button from 'primevue/button'

const router = useRouter()
const search = ref('')
const mostrarSubmenu = ref(false)

const toggleSubmenu = () => {
  mostrarSubmenu.value = !mostrarSubmenu.value
}

const submenuRef = ref(null)

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
.sidebar {
  width: 360px;
  height: 100vh;
  background-color: #ffffff;
  border-right: 1px solid #ddd;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  padding: 1rem;
  box-shadow: 0 0 10px rgba(0, 0, 0, 0.05);
}

.sidebar-top {
  display: flex;
  flex-direction: column;
}

.sidebar-header {
  margin-bottom: 1rem;
}

.sidebar-title {
  padding-top: 15px;
  font-size: 1.25rem;
  font-weight: bold;
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

.sidebar-link.red {
  color: #c0392b;
  font-weight: 600;
}

.badge-wrapper {
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.badge {
  background-color: #1976d2;
  color: #fff;
  font-size: 0.75rem;
  font-weight: bold;
  padding: 2px 8px;
  border-radius: 999px;
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

/* ✅ Corrección de lupa */
:deep(.p-input-icon-left) {
  position: relative;
  display: flex;
  align-items: center;
}

:deep(.p-input-icon-left > i.pi-search) {
  position: absolute;
  left: 0.75rem;
  top: 50%;
  transform: translateY(-50%);
  color: #888;
  font-size: 1rem;
  pointer-events: none;
  z-index: 1;
}

:deep(.custom-search-input) {
  padding-left: 2rem !important;
  border-radius: 8px !important;
  border: 1px solid #ccc !important;
  background-color: #f9f9f9 !important;
  font-size: 0.85rem !important;
  transition: all 0.2s ease-in-out;
}
</style>
