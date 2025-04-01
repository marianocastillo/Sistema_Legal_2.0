import { createRouter, createWebHistory } from 'vue-router';
import LoginView from '../components/views/LoginView.vue';
import Sidebar from '../layouts/Sidebar.vue'; // Importa el layout
import TodosRegistro from '../components/views/TodosRegistro.vue';
import TableView from '../components/views/TableView.vue';
import EditarCursos from '../components/views/EditarCursos.vue';

const routes = [
  { path: '/', name: '/Login', component: LoginView },
  {
    path: '/sidebar',
    component: Sidebar,
    children: [
      { path: '', redirect: 'home' }, // 👈 Redirige a 'home' cuando accedes a '/sidebar'
      { path: 'home', name: 'TodosRegistro', component: TodosRegistro },
      { path: 'registro', name: 'TableView', component: TableView },
      { path: 'edit', name: 'EditarCursos', component: EditarCursos }
    ]
  }
];


const router = createRouter({
  history: createWebHistory(),
  routes
});

export default router;
