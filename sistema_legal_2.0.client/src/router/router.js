
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
import EdicionSalas from '@/components/views/Salas/EdicionSalas.vue';
import ListadoSalas from '@/components/views/Salas/ListadoSalas.vue';
import Qalendar from '@/components/views/Qalendar.vue';
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
        meta: { requiresAuth: true, roles: [1, 2] }
      },
      { path: '', redirect: '/GestionTribunales' },
      {
        path: 'GestionTribunales',
        name: 'GestionTribunales',
        component:ListadoTribunales ,
        meta: { requiresAuth: true, roles: [1, 2] }
      },
            { path: '', redirect: '/GestionSalas' },
      {
        path: 'GestionSalas',
        name: 'GestionSalas',
        component:ListadoSalas ,
        meta: { requiresAuth: true, roles: [1, 2, 3] }
      },
      //  { path: '', redirect: '/Calendario' },
      // {
      //   path: 'Calendario',
      //   name: 'Calendario',
      //   component:Calendar ,
      //   meta: { requiresAuth: true, roles: [1, 2,3,4] }
      // },
      {
        path: 'abogado/inicio',
        name: 'VistaAbogado',
        component: VistaAbogado,
        meta: { requiresAuth: true, roles: [4] }
      },
      {
        path: 'registrar',
        name: 'RegistroLitigio',
        component: RegistroLitigio,
        meta: { requiresAuth: true, roles: [1, 2, 3] }
      },
            {
        path: 'Calendario',
        name: 'Calendatio',
        component: Qalendar,
        meta: { requiresAuth: true, roles: [1, 2, 3] }
      },
      {
        path: '/Seguimiento',
        name: 'BandejaSeguimiento',
        component: BandejaSeguimiento,
        meta: { requiresAuth: true, roles: [1] }
      },
      {
        path: 'LitigiosRegistrados',
        name: 'BandejaRegistrados',
        component: BandejaRegistrados,
        meta: { requiresAuth: true, roles: [1, 2, 3] }
      },
      {
        path: 'buscarlitigio',
        name: 'BuscarLitigio',
        component: BuscarLitigio,
        meta: { requiresAuth: true, roles: [1, 2, 4] }
      },
      {
        path: 'edit',
        name: 'TableView',
        component: TableView,
        meta: { requiresAuth: true, roles: [1] }
      },
      {
        path: 'modificarregistro',
        name: 'ModificarRegistro',
        component: ModificarRegistro,
        meta: { requiresAuth: true, roles: [1, 2, 3, 4] }
      },
      {
        path: 'listadodeusuario',
        name: 'ListadoUsuariosVie',
        component: ListadoUsuariosView,
        meta: { requiresAuth: true, roles: [1] }
      },
      {
        path: 'formulario',
        name: 'nuevoUsuario',
        component: FormularioView,
        meta: { requiresAuth: true, roles: [1] }
      },
      {
        path: 'formulario/:idUsuario',
        name: 'formulario',
        component: FormularioView,
        meta: { requiresAuth: true, roles: [1] }
      },
      {
        path: 'litigio/detalle/:id',
        name: 'LitigioDetalle',
        component: () => import('../components/views/LitigioMostrar.vue'),
        props: true,
        meta: { requiresAuth: true, roles: [1, 2, 3, 4] }
      },
            {
        path: 'EdicionSalas/:idSala',
        name: 'EdicionSalas',
        component: EdicionSalas,
        meta: { requiresAuth: true, roles: [1, 2, 3, 4] }
      },

     {
        path: 'EdicionSalas',
        name: 'NuevaSala',
        component: EdicionSalas,
        meta: { requiresAuth: true, roles: [1, 2, 3, 4] }
      }
    ]
  },
  {
    path: '/unauthorized',
    name: 'Unauthorized',
    component: () => import('@/components/views/Unauthorized.vue')
  }
];

const router = createRouter({
  history: createWebHistory(),
  routes
});

// ✅ Protección por autenticación y perfil
router.beforeEach((to, from, next) => {
  const token = localStorage.getItem('token');
  const rawUser = localStorage.getItem('usuario');
  const user = rawUser ? JSON.parse(rawUser) : null;

  if (to.name === 'Login') return next();

  if (!token || !user) return next({ name: 'Login' });

  if (!to.meta.requiresAuth) return next();

  const perfilId = parseInt(user.perfil);
  if (to.meta.roles && !to.meta.roles.includes(perfilId)) {
    return next({ name: 'Unauthorized' });
  }
  next();
});


export default router;
