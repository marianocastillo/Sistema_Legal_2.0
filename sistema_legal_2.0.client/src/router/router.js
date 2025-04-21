import { createRouter, createWebHistory } from 'vue-router'

// Vistas
import LoginView from '@/components/views/LoginView.vue'
import AppLayout from '@/layouts/AppLayout.vue'
import RegistroView from '@/components/views/RegistroView.vue'

const routes = [
  {
    path: '/login',
    name: 'Login',
    component: LoginView
  },
  {
    path: '/',
    component: AppLayout,
    children: [
      { path: '', redirect: '/home' },
      { path: 'home', name: 'Registro', component: RegistroView },
    ]
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

// ✅ Middleware de autenticación
router.beforeEach((to, from, next) => {
  const token = localStorage.getItem('token')
  if (to.path !== '/login' && !token) {
    next('/login')
  } else {
    next()
  }
})

export default router
