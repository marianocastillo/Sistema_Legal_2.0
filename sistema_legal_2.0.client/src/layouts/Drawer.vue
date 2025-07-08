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
            <!-- Inicio: Todos los roles -->
            <li>
              <router-link :to="rutaInicio" class="sidebar-link" exact-active-class="active">
                <i class="pi pi-home" />
                <span>Inicio</span>
              </router-link>
            </li>

            <!-- Registrar: Supervisor, Digitador -->
            <li v-if="['Supervisor', 'Digitador'].includes(usuario.rol)">
              <router-link to="/registrar" class="sidebar-link" exact-active-class="active">
                <i class="pi pi-file-edit" />
                <span>Registrar</span>
              </router-link>
            </li>

            <!-- Modificar: Supervisor, Abogado Litigante -->
            <li v-if="['Supervisor'].includes(usuario.rol)">
              <router-link to="/buscarlitigio" class="sidebar-link" exact-active-class="active">
                <i class="pi pi-pencil" />
                <span>Modificar</span>
              </router-link>
            </li>
            <li v-if="['Supervisor', 'Administrador'].includes(usuario.rol)">
              <a href="#" class="sidebar-link" @click.prevent="mostrarDialogoTribunales = true">
                <i class="pi pi-pencil" />
                <span>Tribunales</span>
              </a>
            </li>


            <li v-if="['Supervisor', 'Administrador', 'Digitador'].includes(usuario.rol)">
              <router-link to="/GestionSalas" class="sidebar-link" exact-active-class="active">
                <i class="pi pi-pencil" />
                <span>Salas</span>
              </router-link>
            </li>
               <li v-if="['Supervisor', 'Administrador'].includes(usuario.rol)">
              <router-link to="/Calendario" class="sidebar-link" exact-active-class="active">
                <i class="pi pi-pencil" />
                <span>Calendario</span>
              </router-link>
            </li>

            <!-- Configuración: visible para todos -->
            <li ref="submenuRef">
              <div class="sidebar-link" @click="toggleSubmenu" style="cursor: pointer;">
                <i class="pi pi-cog" />
                <span>Configuración</span>
                <i class="pi pi-chevron-down ml-auto" :class="{ 'rotate-180': mostrarSubmenu }" />
              </div>

              <ul v-if="mostrarSubmenu" class="submenu">
                <!-- Solo Admin -->
                <li v-if="usuario.rol === 'Administrador'">
                  <router-link to="/listadodeusuario">
                    <Button label="Lista de Usuario" icon="pi pi-user" class="p-button-text p-button-sm w-full" />
                  </router-link>
                </li>

                <!-- Solo Admin -->
                <li v-if="usuario.rol === 'Administrador'">
                  <router-link to="/formulario">
                    <Button label="Añadir Usuario" icon="pi pi-user" class="p-button-text p-button-sm w-full" />
                  </router-link>
                </li>

                <!-- Todos -->
                <li>
                  <Button label="Cerrar sesión" icon="pi pi-sign-out" class="p-button-text p-button-sm w-full"
                    @click="cerrarSesion" />
                </li>
              </ul>
            </li>
          </ul>

        </nav>
      </div>

      <teleport to="body">
   <ListadoTribunales
  v-model:visible="mostrarDialogoTribunales"
/>

      </teleport>

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
</template>


<script setup>
import { ref, onMounted, onBeforeUnmount, computed } from 'vue';
import { useRouter } from 'vue-router';
import { cerrarSesion } from '@/utilities/auth';
import Button from 'primevue/button';
import Menu from 'primevue/menu';
import Avatar from 'primevue/avatar';
import { push } from 'notivue';
import ListadoTribunales from '@/components/views/Tribunales/ListadoTribunales.vue';


const router = useRouter();

const mostrarDialogoTribunales = ref(false)

const rutaInicio = ref('');
const mostrarSubmenu = ref(false);
const submenuRef = ref(null);
const isSidebarVisible = ref(true);
const menu = ref(); // Ref para el dropdown Menu

const usuario = ref({ nombre: '', rol: '', perfil: null });
const isMobile = computed(() => window.innerWidth <= 768);

// Dropdown items del usuario
const items = computed(() => {
  const opciones = []

  if (usuario.value.rol === 'Administrador') {
    opciones.push({
      label: 'Manejo de usuarios',
      icon: 'pi pi-user-edit',
      command: () => {
        router.push('/listadodeusuario')
      }
    })
  }

  // Cerrar sesión siempre disponible
  opciones.push({
    label: 'Cerrar sesión',
    icon: 'pi pi-sign-out',
    command: () => {
      cerrarSesion()
    }
  })

  return opciones
})


const toggleSubmenu = () => {
  mostrarSubmenu.value = !mostrarSubmenu.value;
};

const toggleSidebar = () => {
  isSidebarVisible.value = !isSidebarVisible.value;
};

const toggleMenu = (event) => {
  menu.value.toggle(event);
};

const handleClickOutside = (e) => {
  if (submenuRef.value && !submenuRef.value.contains(e.target)) {
    mostrarSubmenu.value = false;
  }
};

const getInitials = (name) => {
  if (!name) return '';
  return name.split(' ').map(p => p[0]).join('').slice(0, 2).toUpperCase();
};

onMounted(() => {
  const stored = localStorage.getItem('usuario');
  if (stored) {
    usuario.value = JSON.parse(stored);

    const perfilId = parseInt(usuario.value.perfil);
    const rutasPorPerfil = {
      1: '/Seguimiento',
      2: '/GestionLitigios',
      3: '/LitigiosRegistrados',
      4: '/abogado/inicio'
    };

    rutaInicio.value = rutasPorPerfil[perfilId] || '/Seguimiento';

  }

  document.addEventListener('click', handleClickOutside);
});

onBeforeUnmount(() => {
  document.removeEventListener('click', handleClickOutside);
});
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
