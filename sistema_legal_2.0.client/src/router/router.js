import { createRouter, createWebHistory } from 'vue-router';

import LoginView from '../components/views/LoginView.vue';
import Drawer from '../layouts/Drawer.vue';
import RegistroLitigio from '../components/views/RegistrarLitigio.vue';
import ListadoTribunales from '@/components/views/Tribunales/ListadoTribunales.vue';
import TableView from '../components/views/TableView.vue';
import ModificarRegistro from '../components/views/ModificarRegistro.vue';
import ListadoUsuariosView from '../components/views/Configs/ListadoUsuariosView.vue';
import FormularioView from '../components/views/Configs/FormularioView.vue';
import BuscarLitigio from '@/components/views/BuscarLitigio.vue';
import AdministradorLitigio from '@/components/views/AdministradorLitigio.vue';
import VistaAbogado from '@/components/views/VistaAbogado.vue';
import BandejaRegistrados from '@/components/views/BandejaRegistrados.vue';
import BandejaSeguimiento from '@/components/views/BandejaSeguimiento.vue';
import Qalendar from '@/components/views/Qalendar.vue';
import PerfilesMantenimiento from '@/components/views/Perfiles/PerfilesMantenimiento.vue';
import Unauthorized from '@/components/views/Unauthorized.vue';
import LitigioDetalle from '@/components/views/LitigioMostrar.vue';

const routes = [
  {
    path: '/',
    name: 'Login',
    component: LoginView
  },
  {
    path: '/',
    component: Drawer,
    children: [
      { path: '', redirect: '/GestionLitigios' },
      {
        path: 'GestionLitigios',
        name: 'GestionLitigios',
        component: AdministradorLitigio,
        meta: { requiresAuth: true }
      },
      {
        path: 'GestionTribunales',
        name: 'GestionTribunales',
        component: ListadoTribunales,
        meta: { requiresAuth: true }
      },
      {
        path: 'abogado/inicio',
        name: 'VistaAbogado',
        component: VistaAbogado,
        meta: { requiresAuth: true }
      },
      {
        path: 'registrar',
        name: 'RegistroLitigio',
        component: RegistroLitigio,
        meta: { requiresAuth: true }
      },
      {
        path: 'Calendario',
        name: 'Calendario',
        component: Qalendar,
        meta: { requiresAuth: true }
      },
      {
        path: 'Seguimiento',
        name: 'BandejaSeguimiento',
        component: BandejaSeguimiento,
        meta: { requiresAuth: true }
      },
      {
        path: 'LitigiosRegistrados',
        name: 'BandejaRegistrados',
        component: BandejaRegistrados,
        meta: { requiresAuth: true }
      },
      {
        path: 'buscarlitigio',
        name: 'BuscarLitigio',
        component: BuscarLitigio,
        meta: { requiresAuth: true }
      },
      {
        path: 'edit',
        name: 'TableView',
        component: TableView,
        meta: { requiresAuth: true }
      },
      {
        path: 'modificarregistro',
        name: 'ModificarRegistro',
        component: ModificarRegistro,
        meta: { requiresAuth: true }
      },
      {
        path: 'Usuarios',
        name: 'Usuarios',
        component: ListadoUsuariosView,
        meta: { requiresAuth: true }
      },
      {
        path: 'Perfiles',
        name: 'Perfiles',
        component: PerfilesMantenimiento,
        meta: { requiresAuth: true }
      },
      {
        path: '/ConfiguracionUsuarios',
        name: '/ConfiguracionUsuarios',
        component: FormularioView,
        meta: { requiresAuth: true }
      },
      {
        path: 'ConfiguracionUsuarios/:idUsuario',
        name: 'ConfiguracionUsuarios',
        component: FormularioView,
        props: true,
        meta: { requiresAuth: true }
      },
      {
        path: 'Detalles/:id',
        name: 'LitigioDetalle',
        component: LitigioDetalle,
        props: true,
        meta: { requiresAuth: true }
      }
    ]
  },
  {
    path: '/unauthorized',
    name: 'Unauthorized',
    component: Unauthorized
  }
];

const router = createRouter({
  history: createWebHistory(),
  routes
});

router.beforeEach((to, from, next) => {
  const token = localStorage.getItem('token');
  const rawUser = localStorage.getItem('usuario');
  const user = rawUser ? JSON.parse(rawUser) : null;

  if (to.name === 'Login') return next();

  if (!token || !user) return next({ name: 'Login' });

  if (!to.meta.requiresAuth) return next();

  const vistasPermitidas = JSON.parse(localStorage.getItem('vistasPermitidas') || '[]');

  const rutaActual = to.path.toLowerCase();

  const autorizada = vistasPermitidas.some(v =>
    rutaActual.startsWith(v.toLowerCase())
  );

  if (!autorizada && to.name !== 'Unauthorized') {
    return next({ name: 'Unauthorized' });
  }

  next();
});

export default router;
