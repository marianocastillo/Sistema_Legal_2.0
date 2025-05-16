// src/utilities/auth.js
import router from '@/router/router'
import store from '@/store'
import { push } from 'notivue'

export function cerrarSesion(mensaje = 'Sesión finalizada') {
  // Limpiar datos del localStorage
  localStorage.removeItem('token')
  localStorage.removeItem('usuario')
  localStorage.removeItem('user')
  localStorage.removeItem('sessionExpireTime')

  // Limpiar estado global si estás usando Vuex
  store.commit('setUser', null)
  store.commit('setScreenLocked', false)

  // Notificación opcional
  if (mensaje) push.info("sesion cerrada")

  // Redirigir al login
  router.push({ name: 'Login' })
}
