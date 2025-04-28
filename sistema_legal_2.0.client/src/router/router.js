import { createRouter, createWebHistory } from 'vue-router';
import LoginView from '../components/views/LoginView.vue';
import Drawer from '../layouts/Drawer.vue'; // Importa el layout
import TodosRegistro from '../components/views/TodosRegistro.vue';
import RegistroLitigio from '../components/views/RegistrarLitigio.vue';
import TableView from '../components/views/TableView.vue';
import ModificarRegistro from '../components/views/ModificarRegistro.vue';
import LitigioModal from '../components/views/LitigioMostrar.vue';
import ListadoUsuariosView from '../components/views/Configs/ListadoUsuariosView.vue';
import FormularioView from '../components/views/Configs/FormularioView.vue';

const routes = [
  { path: '/', name: 'Login', component: LoginView },
  {
    path: '/drawer',
    component: Drawer,
    children: [
      { path: '', redirect: '/drawer/home' }, // 👈 Redirige a 'home' cuando accedes a '/sidebar'
      // { path: 'modal', name: 'LitigioModal', component: LitigioModal },
      { path: 'home', name: 'TodosRegistro', component: TodosRegistro },
      { path: 'registrar', name: 'RegistroLitigio', component: RegistroLitigio },
      { path: 'edit', name: 'TableView', component: TableView },
      { path: 'modificarregistro', name: 'ModificarRegistro', component: ModificarRegistro },
      {path: 'listadodeusuario', name: 'ListadoUsuariosVie', component: ListadoUsuariosView},
      { path: 'formulario', name: 'nuevoUsuario', component: FormularioView},
      {path: 'formulario/:idUsuario', name: 'formulario', component: FormularioView},
      {
        path: '/litigio/detalle/:id', name: 'LitigioDetalle',
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

export default router;
