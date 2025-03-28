import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '@/views/HomeView.vue'
import UsuariosListado from '@/views/Configuracion/Usuarios/ListadoView.vue'
import UsuariosFormulario from '@/views/Configuracion/Usuarios/FormularioView.vue'
import PerfilesListado from '@/views/Configuracion/Perfiles/ListadoView.vue'
import PerfilesFormulario from '@/views/Configuracion/Perfiles/FormularioView.vue'
// import InstitucionesListado from '@/views/Configuracion/Instituciones/ListadoView.vue'
// import InstitucionesFormulario from '@/views/Configuracion/Instituciones/FormularioView.vue'
// import InstitucionesDetalles from '@/views/Configuracion/Instituciones/DetallesView.vue'
// import VisitasListado from '@/views/Visitas/ListadoView.vue'
// import VisitasFormulario from '@/views/Visitas/FormularioView.vue'
import LoginView from '@/views/LoginView.vue'
import NotFound from "@/views/NotFound.vue";
import store from "@/store";
import { push } from "notivue"
// import $ from 'jquery';
// import * as AdminLte from 'admin-lte';
import api from '@/utilities/api';

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'Inicio',
      component: HomeView,
      meta: {
        authenticated: false,
      }
    },
    {
      path: '/Login',
      name: 'Login',
      component: LoginView,
      meta: {
        authenticated: false,
      }
    },
    {
      path: '/Visitas',
      name: 'Listado de Visitas',
      component: VisitasListado,
      meta: {
        authenticated: true,
      }
    },
    {
      path: '/Visitas/Nuevo',
      name: 'Nueva Visita',
      component: VisitasFormulario,
      meta: {
        authenticated: true,
      }
    },
    {
      path: '/Visitas/Editar/:idVisita',
      name: 'Editar Visita',
      component: VisitasFormulario,
      meta: {
        authenticated: true,
      }
    },
    // {
    //   path: '/Visitas/Detalles/:idVisita',
    //   name: 'Detalles Visita',
    //   component: VisitasFormulario,
    //   meta: {
    //     authenticated: true,
    //   }
    // },
    // {
    //   path: '/Visitas/Completar/:idVisita',
    //   name: 'Completar Visita',
    //   component: VisitasFormulario,
    //   meta: {
    //     authenticated: true,
    //   }
    // },
    // {
    //   path: '/Visitas',
    //   name: 'Listado de Visitas',
    //   component: VisitasListado,
    //   meta: {
    //     authenticated: true,
    //   }
    // },
    {
      path: '/Configuracion/Usuarios',
      name: 'Usuarios',
      component: UsuariosListado,
      meta: {
        authenticated: true,
      }
    },
    {
      path: '/Configuracion/Usuarios/Nuevo',
      name: "Nuevo Usuario",
      component: UsuariosFormulario,
      meta: {
        authenticated: true,
      }
    },
    {
      path: '/Configuracion/Usuarios/Editar/:idUsuario',
      name: "Editar Usuario",
      component: UsuariosFormulario,
      meta: {
        authenticated: true,
      }
    },
    {
      path: '/Configuracion/Perfiles',
      name: 'Perfiles',
      component: PerfilesListado,
      meta: {
        authenticated: true,
      }
    },
    {
      path: '/Configuracion/Perfiles/Nuevo',
      name: "Nuevo Perfil",
      component: PerfilesFormulario,
      meta: {
        authenticated: true,
      }
    },
    {
      path: '/Configuracion/Perfiles/Editar/:idPerfil',
      name: "Editar Perfil",
      component: PerfilesFormulario,
      meta: {
        authenticated: true,
      }
    },
    // {
    //   path: '/Configuracion/Instituciones',
    //   name: 'Instituciones',
    //   component: InstitucionesListado,
    //   meta: {
    //     authenticated: true,
    //   }
    // },
    // {
    //   path: '/Configuracion/Instituciones/Nuevo',
    //   name: "Nuevo Institucion",
    //   component: InstitucionesFormulario,
    //   meta: {
    //     authenticated: true,
    //   }
    // },
    // {
    //   path: '/Configuracion/Instituciones/Editar/:idInstitucion',
    //   name: "Editar Institucion",
    //   component: InstitucionesFormulario,
    //   meta: {
    //     authenticated: true,
    //   }
    // },
    // {
    //   path: '/Configuracion/Instituciones/Detalles/:idInstitucion',
    //   name: "Detalles Institucion",
    //   component: InstitucionesDetalles,
    //   meta: {
    //     authenticated: true,
    //   }
    //},
    { path: "/:pathMatch(.*)*", name: "404", component: NotFound, authenticated: false, administrador: true },
  ]
})

router.beforeEach(async (to, from) => {
  if (to == from) {
    return false;
  }

  let authenticated = await isAuthenticated(to);
  document.title = `${to.name} - Gestión de Visitas`;

  if (!authenticated && to.name != "Login") {
    localStorage.removeItem("token");
    localStorage.removeItem('user');
    localStorage.removeItem('sessionExpireTime');
    store.commit('setScreenLocked', false);
    document.title = "Gestión de Visitas — Iniciar sesión";
    return { name: "Login" };
  }

  if (authenticated && (!hasAccess(to))) {
    push.warning("No está autorizado");
    return { name: from.name };
  }

  if (from.name == "Login") {
    setTimeout(() => {
      $('[data-widget="treeview"]').each(function() {
        AdminLte.Treeview._jQueryInterface.call($(this), 'init');
        AdminLte.Layout._jQueryInterface.call($('body'));
        AdminLte.PushMenu._jQueryInterface.call($('[data-widget="pushmenu"]'));
      });
    }, "1000");
  }
});

function hasAccess(to) {
  const userViews = store.getters.getViews;
  const toName = to.name;
  const hasAccess = userViews.some((view) => view.nombre == toName) || toName == "Inicio";

  return hasAccess || !to.meta.authenticated;
}

async function isAuthenticated() {
  if (localStorage.getItem("token") == null) return false;

  try {
    const response = await api.get("/api/Account");

    if (response.data) {
      store.commit('setUser', response.data.usuario);
      store.commit('setViews', response.data.vistas);
      return true;
    }
    else {
      return false;
    }
  } catch (error) {
    return false;
  }
}

export { hasAccess };

export default router
