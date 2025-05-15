<template>
  <div class="layout">
    <!-- Sidebar (oculto en pantallas pequeñas si isSidebarVisible es false) -->
    <aside class="sidebar" :class="{ hidden: !isSidebarVisible && isMobile }">
      <div class="sidebar-top">
        <div class="sidebar-header">
          <div class="sidebar-brand">
            <img src="../assets/LogoBlanco.png" alt="Logo" class="img-fluid" style="max-width: 50px;" />
            <h4 class="sidebar-title">Sistema Sileg 2.0</h4>
          </div>
        </div>
        <br>
        <nav class="sidebar-menu">
          <ul style="color: white;">
            <li>
              <router-link to="/drawer/home" class="sidebar-link" exact-active-class="active">
                <i class="pi pi-home" />
                <span>Inicio</span>
              </router-link>
            </li>

            <li>
              <router-link to="/drawer/registrar" class="sidebar-link" exact-active-class="active">
                <i class="pi pi-file-edit" />
                <span>Registrar</span>
              </router-link>
            </li>

            <li>
              <router-link to="/drawer/buscarlitigio" class="sidebar-link" exact-active-class="active">
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
                  <router-link to="/drawer/listadodeusuario"> <Button label="Lista de Usuario" icon="pi pi-user"
                      class="p-button-text p-button-sm w-full pl-4" /></router-link>
                </li>
              </ul>

              <ul v-if="mostrarSubmenu" class="submenu">
                <li>
                  <router-link to="/drawer/formulario"> <Button label="Añadir Usuario" icon="pi pi-user"
                      class="p-button-text p-button-sm w-full pl-4" /></router-link>
                </li>
              </ul>

              <ul v-if="mostrarSubmenu" class="submenu">
                <li>
                  <Button label="Cerrar sesión" icon="pi pi-sign-out" class="p-button-text p-button-sm w-full pl-4"
                    @click="cerrarSesion" />
                </li>
              </ul>
            </li>
          </ul>
        </nav>
      </div>

      <footer class="sidebar-footer">
        © 2025 Sistema Sileg 2.0
      </footer>
    </aside>

    <!-- Contenedor principal (Header + contenido dinámico) -->
    <div class="main-wrapper">
      <!-- Header -->
      <header class="navbar">
        <div class="menu-placeholder">
          <button class="menu-button" @click="toggleSidebar" aria-label="Abrir menú">
            <i class="pi pi-bars" />
          </button>
        </div>

        <div class="user-profile" v-if="usuario.nombre">
          <div class="user-info">
            <div class="user-name">{{ usuario.nombre }}</div>
            <div class="user-role">{{ usuario.rol }}</div>
          </div>
        </div>
      </header>

      <!-- Contenido de la ruta -->
      <main class="main-content">
        <router-view />
      </main>
    </div>

    <!-- Notificaciones -->
    <Notivue v-slot="item">
      <NotivueSwipe :item="item">
        <Notifications :item="item" :theme="pastelTheme" />
      </NotivueSwipe>
    </Notivue>
  </div>

</template>

<script setup>
import { ref, onMounted, onBeforeUnmount, computed } from 'vue'
import { useRouter } from 'vue-router'

import Button from 'primevue/button'
import { Notivue, Notifications, NotivueSwipe, pastelTheme } from 'notivue'

const router = useRouter()

const mostrarSubmenu = ref(false)
const submenuRef = ref(null)
const isSidebarVisible = ref(true)
const usuario = ref({ nombre: '', rol: '' })

const isMobile = computed(() => window.innerWidth <= 768)

const toggleSubmenu = () => {
  mostrarSubmenu.value = !mostrarSubmenu.value
}

const toggleSidebar = () => {
  isSidebarVisible.value = !isSidebarVisible.value
}

const handleClickOutside = (e) => {
  if (submenuRef.value && !submenuRef.value.contains(e.target)) {
    mostrarSubmenu.value = false
  }
}

onMounted(() => {
  const stored = localStorage.getItem('usuario')
  if (stored) {
    usuario.value = JSON.parse(stored)
  }
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
*,
*::before,
*::after {
  box-sizing: border-box;
}

html,
body {
  margin: 0;
  padding: 0;
  height: 100%;
  width: 100%;
  background-color: #f8f9fa;
  overflow: hidden;
}


.layout {
  display: flex;
  width: 100%;
  height: 100%;
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: #f8f9fa;
}

/* Sidebar */
.sidebar {
  width: 250px;
  background-color: #003870;
  border-right: 1px solid #ddd;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  padding: 1rem;
  box-shadow: 0 0 10px hwb(200 97% 2%);
  height: 100vh;
  /* ✅ Esto lo hace ocupar toda la altura */
  position: fixed;
  /* ✅ Lo fija a la izquierda */
  top: 0;
  left: 0;
}

.sidebar.hidden {
  display: none;
}

.sidebar-link {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.5rem 0.75rem;
  text-decoration: none;
  color: #fff;
  border-radius: 6px;
  transition: background-color 0.2s ease;
}

.sidebar-link:hover {
  background-color: #c00606;
  font-weight: 600;
}

.sidebar-link i {
  font-size: 1rem;
}

/* Main wrapper */
.main-wrapper {
  margin-left: 250px;
  /* ancho del sidebar */
  flex: 1;
  display: flex;
  flex-direction: column;
  height: 100%;
  overflow: hidden;
}

.navbar {
  height: 64px;
  background-color: #fff;
  border-bottom: 1px solid #ddd;
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0 1rem;
  position: sticky; /* mejor que fixed en este caso */
  top: 0;
  z-index: 10;
}


.menu-placeholder {
  display: flex;
  align-items: center;
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

/* Contenido */
.main-content {
  flex: 1;
  overflow-y: auto;
  padding: 1.5rem;
  background-color: #f9f9f9;
}

.sidebar-footer {
  font-size: 0.75rem;
  text-align: center;
  color: #999;
  border-top: 1px solid #eee;
  padding-top: 1rem;
}

/* Usuario */
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

/* Misceláneo */
.logo {
  width: 62px;
  height: 62px;
  object-fit: contain;
}

.submenu {
  padding-left: 1.5rem;
  margin-top: 0.3rem;
}

.rotate-180 {
  transform: rotate(180deg);
  transition: transform 0.2s ease;
}




.sidebar-brand {
  display: flex;
  /* Usamos Flexbox para alinearlos en una fila */
  align-items: center;
  /* Centra verticalmente el contenido */
  gap: 10px;
  /* Espacio entre el texto y la imagen */
}

.sidebar-title {
  margin: 0;
  color: #fff;
  /* Elimina el margen por defecto del h4 */
  font-size: 1.2rem;
  /* Ajusta el tamaño del texto si es necesario */
}

.logo {
  width: 40px;
  /* Ajusta el tamaño de la imagen si es necesario */
  height: auto;
  object-fit: contain;
  /* Asegura que la imagen mantenga su proporción */
}
</style>
