import { createRouter, createWebHistory } from "vue-router"
import AppLayout from "../components/AppLayout.vue"
import Pessoa from "../components/Pessoa.vue"
import Equipamentos from "../components/Equipamentos.vue"
import Laboratorio from "../components/Laboratorio.vue"
import Usuarios from "../components/Usuarios.vue"
import GerenciadorLaboratorio from "../components/GerenciarLaboratorio.vue"
import Solicitacao from "../components/Solicitacao.vue"
import Calendario from "../components/Calendario.vue"
import Login from "../components/Login.vue"

const routes = [
  {
    path: "/",
    component: AppLayout,
    children: [
      {
        path: "pessoas",
        name: "pessoas",
        component: Pessoa,
      },
      {
        path: "equipamentos",
        name: "equipamentos",
        component: Equipamentos,
      },
      {
        path: "laboratorios",
        name: "laboratorios",
        component: Laboratorio,
      },
      {
        path: "gerenciador-laboratorio",
        name: "gerenciador-laboratorio",
        component: GerenciadorLaboratorio,
      },
      {
        path: "usuarios",
        name: "usuarios",
        component: Usuarios,
      },
      {
        path: "solicitacoes",
        name: "solicitacoes",
        component: Solicitacao,
      },
      {
        path: "calendario",
        name: "calendario",
        component: Calendario,
      },
      {
        path: "",
        redirect: "/pessoas",
      },
    ],
  },
  { path: "/login", component: Login },
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

router.beforeEach((to, from, next) => {
  const publicPages = ["/login"]
  const authRequired = !publicPages.includes(to.path)
  const loggedIn = localStorage.getItem("token")

  if (authRequired && !loggedIn) {
    return next("/login")
  }

  // Rotas restritas para admin
  const adminRoutes = ["pessoas", "equipamentos", "laboratorios", "usuarios"]
  if (adminRoutes.includes(to.name)) {
    const token = localStorage.getItem("token")
    if (!token) return next("/login")
    try {
      const payload = JSON.parse(atob(token.split(".")[1]))
      if (!payload.isAdmin || payload.isAdmin === "false") {
        // Se não for admin, redireciona para gerenciador-laboratorio
        return next({ name: "gerenciador-laboratorio" })
      }
    } catch (e) {
      return next("/login")
    }
  }

  // Usuários não admins só podem acessar gerenciador-laboratorio
  if (to.name === "gerenciador-laboratorio") {
    // Qualquer usuário autenticado pode acessar
    return next()
  }

  next()
})

export default router
