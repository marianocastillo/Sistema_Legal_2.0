import { createRouter, createWebHistory } from 'vue-router';
import store from "@/store";
import { push } from "notivue";
import api from "@/utilities/api";
import PerfilesEnum from "@/utilities/PerfilesEnum";

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/home',
      name: 'home',
      component: () => import ('../views/HomeView.vue')
    },


    {
      path: '/login',
      name: 'login',
      component: () => import('./'),
    }

    // {
    //   path: '/lista-de-tarea',
    //   name: 'lista-de-tarea',
    //   component: () => import('../components/ListaDeTareas.vue'),
    // },

    // {
    //   path: '/formulario',
    //   name: 'formulario',
    //   component: () => import('../components/FormularioPrime.vue'),
    // }
  ]
});


router.beforeEach(async (to, from) => {
  if (to == from) {
    return false;
  }
//if (localStorage.getItem("DarkMode") == null) {
  //if (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches) {
    //localStorage.setItem("DarkMode", true);
  //} else {
    //localStorage.setItem("DarkMode", false);
 // }
//}

//store.commit('setIsDarkMode', localStorage.getItem("DarkMode") == "true");
//document.documentElement.classList.toggle('p-dark', localStorage.getItem("DarkMode") == "true");

  let authenticated = await isAuthenticated(to);
  document.title = `${to.name} - Ejecución Gastos`;

  if (!authenticated && to.name != "Login") {
    localStorage.removeItem("token");
    localStorage.removeItem("user");
    localStorage.removeItem("sessionExpireTime");
    store.commit("setScreenLocked", false);
    document.title = "Ejecución Gastos — Iniciar sesión";
    return { name: "Login" };
  }

  if (authenticated && !hasAccess(to)) {
    push.warning("No está autorizado");
    return { name: from.name };
  }
});

function hasAccess(to) {
  const user = store.getters.getUser;
  const toName = to.name;
  const hasAccess = user.idPerfil == PerfilesEnum.Administrador || toName == "Inicio";

  return hasAccess || !to.meta.authenticated;
}

async function isAuthenticated() {
  if (localStorage.getItem("token") == null) return false;

  try {
    const response = await api.get("/api/Account");

    if (response.data) {
      store.commit("setUser", response.data.usuario);
      store.commit("setViews", response.data.vistas);
      return true;
    } else {
      return false;
    }
  } catch (error) {
    return false;

  }
}
export { hasAccess };
export default router;

