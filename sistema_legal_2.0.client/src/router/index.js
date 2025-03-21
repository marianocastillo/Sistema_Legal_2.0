import { createRouter, createWebHistory } from 'vue-router'


const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    // {
    //   path: '/',
    //   name: 'home',
    //   component: HomeView,
    // },


    {
      path: '/login',
      name: 'login',
      component: () => import('./'),
    },

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
})

export default router
