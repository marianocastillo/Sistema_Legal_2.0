import { createRouter, createWebHistory } from 'vue-router';
import LoginView from '../components/views/LoginView.vue';
import Drawer from '../layouts/Drawer.vue';
import TodosRegistro from '../components/views/TodosRegistro.vue';
import RegistroLitigio from '../components/views/RegistrarLitigio.vue';
import TableView from '../components/views/TableView.vue';
import ModificarRegistro from '../components/views/ModificarRegistro.vue';
import ListadoUsuariosView from '../components/views/Configs/ListadoUsuariosView.vue';
import FormularioView from '../components/views/Configs/FormularioView.vue';
import BuscarLitigio from '@/components/views/BuscarLitigio.vue';
import VistaAbogado from '@/components/views/VistaAbogado.vue';

const routes = [
  { path: '/', name: 'Login', component: LoginView },
  {
    path: '/drawer',
    component: Drawer,
    children: [
      { path: '', redirect: '/drawer/home' },
      { path: 'home', name: 'TodosRegistro', component: TodosRegistro },
      { path: 'Inicio', name: 'VistaAbogado', component: VistaAbogado },

      { path: 'registrar', name: 'RegistroLitigio', component: RegistroLitigio },
      { path: 'buscarlitigio', name: 'BuscarLitigio', component: BuscarLitigio },
      { path: 'edit', name: 'TableView', component: TableView },
      { path: 'modificarregistro', name: 'ModificarRegistro', component: ModificarRegistro },
      { path: 'listadodeusuario', name: 'ListadoUsuariosVie', component: ListadoUsuariosView },
      { path: 'formulario', name: 'nuevoUsuario', component: FormularioView },
      { path: 'formulario/:idUsuario', name: 'formulario', component: FormularioView },
      {
        path: '/litigio/detalle/:id',
        name: 'LitigioDetalle',
        component: () => import('../components/views/LitigioMostrar.vue'),
        props: true
      }
    ]
  }
];

const router = createRouter({
  history: createWebHistory(),
  routes
});

// ✅ Protege rutas privadas si no hay token
router.beforeEach((to, from, next) => {
  const token = localStorage.getItem('token');
  const rutaPublica = to.name === 'Login';

  if (!rutaPublica && !token) {
    return next({ name: 'Login' });
  }

  next();
});

export default router;
