import { createRouter, createWebHistory } from 'vue-router'
import AppLayout from '../components/AppLayout.vue'
import Pessoa from '../components/Pessoa.vue'
import Equipamentos from '../components/Equipamentos.vue'
import Laboratorio from '../components/Laboratorio.vue'
import Solicitacao from '../components/Solicitacao.vue'
import GerenciarLaboratorio from '../components/GerenciarLaboratorio.vue'

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
        path: 'solicitacoes',
        name: 'solicitacoes',
        component: Solicitacao
      },
      {
        path: 'gerenciar-laboratorio',
        name: 'gerenciar-laboratorio',
        component: GerenciarLaboratorio
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