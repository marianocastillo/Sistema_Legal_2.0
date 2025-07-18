<template>
  <div class="layout">

    <!-- Sidebar -->
    <aside class="sidebar" :class="{
      collapsed: isSidebarCollapsed && isResponsive,
      expanded: !isSidebarCollapsed && isResponsive,
      hidden: isMobile && !isSidebarVisible
    }" @mouseenter="isResponsive ? isSidebarCollapsed = false : null"
      @mouseleave="isResponsive ? isSidebarCollapsed = true : null">


      <div class="sidebar-top">
        <div class="sidebar-header">
          <div class="sidebar-brand">
            <img src="../assets/LogoBlanco.png" alt="Logo" class="img-fluid" style="max-width: 50px;" />
            <h4 class="sidebar-title">Sistema Sileg 2.0</h4>
          </div>
        </div>
        <br />
        <nav class="sidebar-menu">
          <ul class="contenedor-menu">
            <!-- Inicio -->
            <li>
              <router-link :to="rutaInicio" class="sidebar-link" exact-active-class="active">
                <i class="pi pi-home" />
                <span>Inicio</span>
              </router-link>
            </li>

            <!-- Vistas dinámicas con principal === true -->
            <li v-for="vista in vistasPrincipales" :key="vista.idVista">
              <router-link :to="vista.url" class="sidebar-link" exact-active-class="active">
                <i :class="vista.iconClass || 'pi pi-home'" />
                <span>{{ vista.nombre }}</span>
              </router-link>
            </li>
            <li>
              <a href="#" class="sidebar-link" @click.prevent="mostrarDialogoTribunales = true">
                <i class="pi pi-building" />
                <span>Tribunales</span>
              </a>
            </li>


          </ul>
        </nav>
      </div>



      <footer class="sidebar-footer">
        © 2025 Sistema Sileg 2.0
      </footer>
    </aside>

    <!-- Main Content -->
    <div class="main-wrapper" :style="{ marginLeft: isSidebarCollapsed ? '60px' : '250px' }">
      <header class="navbar">
        <div class="menu-placeholder">
          <button class="menu-button" @click="toggleSidebar" aria-label="Abrir menú">
            <i class="pi pi-bars" />
          </button>
        </div>

        <!-- Bloque de usuario con dropdown -->
        <div v-if="usuario.nombre" class="user-dropdown">
          <button @click="toggleMenu" class="user-dropdown-btn">
            <Avatar :label="getInitials(usuario.nombre)" shape="circle" class="user-avatar" />
            <div class="user-info">
              <div class="user-name">{{ usuario.nombre }}</div>
              <div class="user-role">{{ usuario.rol }}</div>
            </div>
            <Menu ref="menu" :model="items" :popup="true" />
          </button>
        </div>
      </header>

      <main class="main-content">
        <router-view />
      </main>
    </div>
  </div>

  <teleport to="body">
    <ListadoTribunales v-model:visible="mostrarDialogoTribunales" />
  </teleport>

</template>

<script setup>
import { ref, onMounted, onBeforeUnmount, computed } from 'vue'
import { useRouter } from 'vue-router'
import axios from 'axios'
import { cerrarSesion } from '@/utilities/auth'
import Button from 'primevue/button'

import Menu from 'primevue/menu'
import Avatar from 'primevue/avatar'
import { push } from 'notivue'
import ListadoTribunales from '@/components/views/Tribunales/ListadoTribunales.vue'

const router = useRouter()

const mostrarDialogoTribunales = ref(false)
const mostrarSubmenu = ref(false)
const submenuRef = ref(null)
const isSidebarVisible = ref(true)
const menu = ref()
const isSidebarCollapsed = ref(true)
const isMobile = computed(() => window.innerWidth <= 768)



const usuario = ref({ nombre: '', rol: '', perfil: null })
const rutaInicio = ref('')
const vistasPrincipales = ref([])

const isResponsive = computed(() => window.innerWidth < 1200)


const toggleSubmenu = () => {
  mostrarSubmenu.value = !mostrarSubmenu.value
}

const toggleSidebar = () => {
  isSidebarVisible.value = !isSidebarVisible.value
}

const toggleMenu = (event) => {
  menu.value.toggle(event)
}

const getInitials = (name) => {
  if (!name) return ''
  return name.split(' ').map(p => p[0]).join('').slice(0, 2).toUpperCase()
}

const items = computed(() => {
  const opciones = []

  if (usuario.value.rol === 'Administrador') {
    opciones.push({
      label: 'Manejo de usuarios',
      icon: 'pi pi-user-edit',
      command: () => router.push('/Usuarios')
    })
  }

  opciones.push({
    label: 'Cerrar sesión',
    icon: 'pi pi-sign-out',
    command: cerrarSesion
  })

  return opciones
})

const handleClickOutside = (e) => {
  if (submenuRef.value && !submenuRef.value.contains(e.target)) {
    mostrarSubmenu.value = false
  }
}

const updateSidebarState = () => {
  if (!isResponsive.value) {
    isSidebarCollapsed.value = false
  } else {
    isSidebarCollapsed.value = true
  }
}

onMounted(async () => {
  // Carga de usuario y vistas
  const stored = localStorage.getItem('usuario')
  if (stored) {
    usuario.value = JSON.parse(stored)
    const idPerfil = usuario.value.perfil

    try {
      const res = await axios.get(`https://localhost:7177/api/Perfiles/GetPermisos/${idPerfil}`)
      const vistas = res.data

      vistasPrincipales.value = vistas.filter(v => v.permiso && v.principal)
      rutaInicio.value = vistasPrincipales.value[0]?.url || '/unauthorized'
    } catch (error) {
      console.error('Error cargando vistas del perfil:', error)
      rutaInicio.value = '/unauthorized'
    }
  }

  // Cierre del menú de usuario
  document.addEventListener('click', handleClickOutside)

  // Detectar tamaño inicial de pantalla
  updateSidebarState()

  // Escuchar cambios de tamaño de pantalla
  window.addEventListener('resize', updateSidebarState)
})


onBeforeUnmount(() => {
  // Limpia el listener para cerrar el menú de usuario
  document.removeEventListener('click', handleClickOutside)

  // Limpia el listener del resize que controla el colapso del sidebar
  window.removeEventListener('resize', updateSidebarState)
})

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

.main-expanded {
  margin-left: 0 !important;
}

/* Sidebar */
.sidebar {
  background-color: #003870;
  border-right: 1px solid #ddd;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  padding: 1rem;
  box-shadow: 0 0 10px hwb(200 97% 2%);
  height: 100vh;
  position: fixed;
  top: 0;
  left: 0;
  width: 250px;
  transition: width 0.3s ease;
  overflow-x: hidden;
}

/* Cuando está colapsado */
.sidebar.collapsed {
  width: 60px;
}

/* Oculta el texto de los enlaces */
.sidebar.collapsed .sidebar-title,
.sidebar.collapsed span {
  display: none;
}

/* Ajusta el ícono si deseas */
.sidebar.collapsed i {
  margin-left: 0.25rem;
  font-size: 1.2rem;
}

/* Cuando se expande */
.sidebar.expanded .sidebar-title,
.sidebar.expanded span {
  display: inline;
}


.contenedor-menu {
  list-style: none;
  padding-left: 0;
  margin-left: 0.4rem;
}


/* Centrado visual de íconos cuando colapsado */
.sidebar.collapsed .sidebar-link {
  justify-content: center;
  padding-left: 0.5rem;
  padding-right: 0.5rem;
}

.sidebar.collapsed .sidebar-link i {
  margin: 0;
}

.sidebar.expanded .sidebar-link {
  justify-content: flex-start;
}

/* Opcional: logo centrado cuando colapsado */
.sidebar.collapsed .sidebar-brand {
  justify-content: center;
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

.sidebar-link.active {
  background-color: #c00606;
  font-weight: 600;
}

/* Main wrapper */
.main-wrapper {
  flex: 1;
  display: flex;
  flex-direction: column;
  height: 100%;
  transition: margin-left 0.3s ease;
  overflow: hidden;
}

/* Navbar */
.navbar {
  height: 64px;
  background-color: #fff;
  border-bottom: 1px solid #ddd;
  display: flex;
  font-family: Arial, sans-serif;
  align-items: center;
  justify-content: space-between;
  padding: 0 1rem;
  position: sticky;
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

/* Dropdown de Usuario  */
.user-dropdown {
  display: flex;
  align-items: center;
  margin-right: 1rem;
}

.user-dropdown-btn {
  display: flex;
  align-items: center;
  padding: 6px 12px;
  background-color: rgb(255, 255, 255);
  border: none;
  border-radius: 8px;
  box-shadow: 0 1px 4px rgba(0, 0, 0, 0.1);
  cursor: pointer;
}

.user-avatar {
  margin-right: 10px;
  background-color: #003880 !important;
  color: #ffffff !important;
}

.user-info {
  display: flex;
  flex-direction: column;
  line-height: 1.2;
  text-align: left;
}

.user-name {
  font-weight: 600;
  color: #003880;
}

.user-role {
  font-size: 0.75rem;
  color: #6c757d;
}

/* Misc */
.logo {
  width: 40px;
  height: auto;
  object-fit: contain;
}

.submenu {
  list-style: none;
}

.submenu .p-button {
  justify-content: left !important;

}

.rotate-180 {
  transform: rotate(180deg);
  transition: transform 0.2s ease;
}

.sidebar-brand {
  display: flex;
  align-items: center;
  gap: 10px;
  border-bottom: 1px solid #9c9c9c;
}

.sidebar-title {
  margin: 0;
  color: #fff;
  font-size: 1.2rem;
}
</style>
