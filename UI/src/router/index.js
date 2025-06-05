import { createRouter, createWebHistory } from 'vue-router'
import AppLayout from '../components/AppLayout.vue'
import Pessoa from '../components/Pessoa.vue'
import Equipamentos from '../components/Equipamentos.vue'

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
        path: '',
        redirect: '/pessoas'
      }
    ]
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router 