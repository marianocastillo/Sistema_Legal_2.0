<script setup>
import { ref, onMounted, onBeforeUnmount, computed } from 'vue';
import { useRouter } from 'vue-router';
import { cerrarSesion } from '@/utilities/auth';
import Button from 'primevue/button';
import Menu from 'primevue/menu';
import Avatar from 'primevue/avatar';
import ListadoTribunales from '@/components/views/Tribunales/ListadoTribunales.vue';

const router = useRouter();

const mostrarDialogoTribunales = ref(false);
const isSidebarVisible = ref(true);
const menu = ref();

const usuario = ref({ nombre: '', rol: '', perfil: null });
const rutaInicio = ref('/');
const vistasPermitidas = ref([]);

const isMobile = computed(() => window.innerWidth <= 768);

const toggleSidebar = () => {
  isSidebarVisible.value = !isSidebarVisible.value;
};

const toggleMenu = (event) => {
  menu.value.toggle(event);
};

const getInitials = (name) => {
  if (!name) return '';
  return name.split(' ').map(p => p[0]).join('').slice(0, 2).toUpperCase();
};

const items = computed(() => {
  const opciones = [];

  if (usuario.value.rol === 'Administrador') {
    opciones.push({
      label: 'Manejo de usuarios',
      icon: 'pi pi-user-edit',
      command: () => router.push('/Usuarios')
    });


    opciones.push({
      label: 'Manejo de Perfiles',
      icon: 'pi pi-sitemap',
      command: () => router.push('/Perfiles')
    })
  }
  opciones.push({
    label: 'Cerrar sesión',
    icon: 'pi pi-sign-out',
    command: cerrarSesion
  });

  return opciones;
});

onMounted(() => {
  const stored = localStorage.getItem('usuario');
  const storedInicio = localStorage.getItem('rutaInicio');
  const storedVistas = localStorage.getItem('vistasPermitidas');

  if (stored) usuario.value = JSON.parse(stored);
  if (storedInicio) rutaInicio.value = storedInicio;
  if (storedVistas) vistasPermitidas.value = JSON.parse(storedVistas);
});

</script>

<template>
  <div class="layout">
    <!-- Sidebar -->
    <aside class="sidebar" :class="{ hidden: !isSidebarVisible && isMobile }">
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
            <!-- Botón Inicio -->
            <li>
              <router-link :to="rutaInicio" class="sidebar-link" exact-active-class="active">
                <i class="pi pi-home" />
                <span>Inicio</span>
              </router-link>
            </li>

            <!-- Vistas dinámicas -->
            <li v-for="vista in vistasPermitidas.filter(v => v.permiso && v.ruta && v.principal && v.ruta !== rutaInicio)
" :key="vista.ruta">
              <router-link :to="vista.ruta" class="sidebar-link" exact-active-class="active">
                <i :class="vista.iconClass || 'pi pi-home'" />
                <span>{{ vista.nombreVista }}</span>
              </router-link>
            </li>

            <!-- Botón Tribunales (siempre accesible) -->
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
    <div class="main-wrapper">
      <header class="navbar">
        <div class="menu-placeholder">
          <button class="menu-button" @click="toggleSidebar" aria-label="Abrir menú">
            <i class="pi pi-bars" />
          </button>
        </div>

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

    <!-- Diálogo de Tribunales -->
    <teleport to="body">
      <ListadoTribunales v-model:visible="mostrarDialogoTribunales" />
    </teleport>
  </div>
</template>



<script>

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


const usuario = ref({ nombre: '', rol: '', perfil: null })
const rutaInicio = ref('')
const vistasPrincipales = ref([])

const isMobile = computed(() => window.innerWidth <= 768)

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

/* const items = computed(() => {
  const opciones = []

  if (usuario.value.rol === 'Administrador') {
    opciones.push({
      label: 'Manejo de usuddddarios',
      icon: 'pi pi-user-edit',
      command: () => router.push('/Usuarios')
    })

     opciones.push({
      label: 'Manejo de Perfiles',
      icon: 'pi pi-sitemap',
      command: () => router.push('/Perfiles')
    })

    opciones.push({
      label: 'Manejo de Perfiles',
      icon: 'pi pi-sitemap',
      command: () => router.push('/Perfiles')
    })


  opciones.push({
    label: 'Cerrar sesión',
    icon: 'pi pi-sign-out',
    command: cerrarSesion
  })

  return opciones
}}) */

const handleClickOutside = (e) => {
  if (submenuRef.value && !submenuRef.value.contains(e.target)) {
    mostrarSubmenu.value = false
  }
}

onMounted(async () => {
  const stored = localStorage.getItem('usuario');
  if (stored) {
    usuario.value = JSON.parse(stored);
    const idPerfil = usuario.value.perfil;

    try {
      const res = await axios.get(`/api/Perfiles/GetVistasYInicio/${idPerfil}`);
      const { rutaInicio, vistas } = res.data;

      rutaInicio.value = rutaInicio;
      vistasPrincipales.value = vistas.filter(v => v.permiso && v.principal);
    } catch (error) {
      console.error('Error cargando vistas:', error);
      rutaInicio.value = '/unauthorized';
    }
  }
});


onBeforeUnmount(() => {
  document.removeEventListener('click', handleClickOutside)
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
  position: fixed;
  top: 0;
  left: 0;
}

.sidebar.hidden {
  display: none;

}

.contenedor-menu {
  list-style: none;
  padding-left: 0;
  margin-left: 0;
  margin-left: 0.4rem;
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
  flex: 1;
  display: flex;
  flex-direction: column;
  height: 100%;
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
  border-top: 1px solid #f34141;
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
  border-bottom: 1px solid #f34141;
  padding-bottom: 0.2rem;
}

.sidebar-title {
  margin: 0;
  color: #fff;
  font-size: 1.2rem;
}
</style>
