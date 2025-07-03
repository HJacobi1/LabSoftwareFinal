import { createRouter, createWebHistory } from 'vue-router'
import AppLayout from '../components/AppLayout.vue'
import Pessoa from '../components/Pessoa.vue'
import Equipamentos from '../components/Equipamentos.vue'
import Laboratorio from '../components/Laboratorio.vue'
import Usuarios from '../components/Usuarios.vue'
import GerenciadorLaboratorio from '../components/GerenciarLaboratorio.vue'
import Login from '../components/Login.vue'

const routes = [
  {
    path: '/',
    component: AppLayout,
    children: [
      {
        path: 'pessoas',
        name: 'pessoas',
        component: Pessoa
      },
      {
        path: 'equipamentos',
        name: 'equipamentos',
        component: Equipamentos
      },
      {
        path: 'laboratorios',
        name: 'laboratorios',
        component: Laboratorio
      },
      {
        path: 'gerenciador-laboratorio',
        name: 'gerenciador-laboratorio',
        component: GerenciadorLaboratorio
      },
      {
        path: '',
        redirect: '/pessoas'
      }
    ]
  },
  { path: '/login', component: Login },
  { path: '/usuarios', component: Usuarios }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

router.beforeEach((to, from, next) => {
  const publicPages = ['/login'];
  const authRequired = !publicPages.includes(to.path);
  const loggedIn = localStorage.getItem('token');

  if (authRequired && !loggedIn) {
    return next('/login');
  }
  next();
});

export default router 