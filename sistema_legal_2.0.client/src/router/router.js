import { createRouter, createWebHistory } from 'vue-router';
import LoginView from '../components/views/LoginView.vue';
import Drawer from '../layouts/Drawer.vue'; // Importa el layout
import TodosRegistro from '../components/views/TodosRegistro.vue';
import RegistroLitigio from '../components/views/RegistrarLitigio.vue';
import TableView from '../components/views/TableView.vue';
import ModificarRegistro from '../components/views/ModificarRegistro.vue';
// import HomeView from '../components/views/HomeView.vue';

const routes = [
  { path: '/', name: 'Login', component: LoginView },
  {
    path: '/drawer',
    component: Drawer,
    children: [
      { path: '', redirect: '/drawer/home' }, // 👈 Redirige a 'home' cuando accedes a '/sidebar'
      // { path: 'home', name: 'HomeView', component: HomeView },
      { path: 'home', name: 'TodosRegistro', component: TodosRegistro },
      { path: 'registrar', name: 'RegistroLitigio', component: RegistroLitigio },
      { path: 'edit', name: 'TableView', component: TableView },
      { path: 'modificarregistro', name: 'ModificarRegistro', component: ModificarRegistro }
    ]
  }
];


const router = createRouter({
  history: createWebHistory(),
  routes
});

export default router;
