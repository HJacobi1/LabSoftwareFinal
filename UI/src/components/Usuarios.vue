<template>
  <div class="usuarios-container">
    <h2>Gerenciar Usuários</h2>
    <form class="usuario-form" @submit.prevent="salvarUsuario">
      <input v-model="novoUsuario.email" type="email" placeholder="Email" required />
      <input v-model="novoUsuario.senha" type="password" placeholder="Senha" :required="!editando" />
      <label class="admin-checkbox">
        <input type="checkbox" v-model="novoUsuario.isAdmin" /> Admin
      </label>
      <button type="submit">{{ editando ? 'Atualizar' : 'Cadastrar' }}</button>
      <button v-if="editando" @click.prevent="cancelarEdicao">Cancelar</button>
    </form>
    <div class="usuarios-list">
      <table>
        <thead>
          <tr>
            <th>Email</th>
            <th>Admin</th>
            <th>Ações</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="usuario in usuarios" :key="usuario.id">
            <td>{{ usuario.email }}</td>
            <td>
              <span v-if="usuario.isAdmin" class="admin-badge">Sim</span>
              <span v-else>Não</span>
            </td>
            <td>
              <div v-if="!(usuario.email == 'admin@admin.com')">
                <button @click="editarUsuario(usuario)">Editar</button>
                <button @click="deletarUsuario(usuario.id)">Deletar</button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
    <div v-if="erro" class="erro">{{ erro }}</div>
  </div>
</template>

<script>
export default {
  data() {
    return {
      usuarios: [],
      novoUsuario: { email: '', senha: '', isAdmin: false },
      editando: false,
      editId: null,
      erro: ''
    };
  },
  async created() {
    await this.carregarUsuarios();
  },
  methods: {
    async carregarUsuarios() {
      try {
        const res = await fetch('/api/usuario/Ativos', {
          headers: { 'Authorization': 'Bearer ' + localStorage.getItem('token') }
        });
        this.usuarios = await res.json();
      } catch (e) {
        this.erro = 'Erro ao carregar usuários';
      }
    },
    async salvarUsuario() {
      this.erro = '';
      try {
        const payload = { ...this.novoUsuario };
        if (!this.editando) delete payload.id;
        if (this.editando) {
          await fetch(`/api/usuario/${this.editId}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json', 'Authorization': 'Bearer ' + localStorage.getItem('token') },
            body: JSON.stringify(payload)
          });
        } else {
          await fetch('/api/usuario/register', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json', 'Authorization': 'Bearer ' + localStorage.getItem('token') },
            body: JSON.stringify(payload)
          });
        }
        this.novoUsuario = { email: '', senha: '', isAdmin: false };
        this.editando = false;
        this.editId = null;
        await this.carregarUsuarios();
      } catch (e) {
        this.erro = 'Erro ao salvar usuário';
      }
    },
    editarUsuario(usuario) {
      this.novoUsuario = { email: usuario.email, senha: '', isAdmin: usuario.isAdmin };
      this.editando = true;
      this.editId = usuario.id;
    },
    cancelarEdicao() {
      this.novoUsuario = { email: '', senha: '', isAdmin: false };
      this.editando = false;
      this.editId = null;
    },
    async deletarUsuario(id) {
      try {
        await fetch(`/api/usuario/${id}`, {
          method: 'DELETE',
          headers: { 'Authorization': 'Bearer ' + localStorage.getItem('token') }
        });
        await this.carregarUsuarios();
      } catch (e) {
        this.erro = 'Erro ao deletar usuário';
      }
    }
  }
};
</script>

<style scoped>
.usuarios-container {
  max-width: 700px;
  margin: 0 auto;
  background: #fff;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.07);
  padding: 2rem;
}
.usuario-form {
  display: flex;
  gap: 1rem;
  align-items: center;
  margin-bottom: 2rem;
}
.usuario-form input[type="email"],
.usuario-form input[type="password"] {
  padding: 0.5rem;
  border-radius: 4px;
  border: 1px solid #ccc;
}
.usuario-form button {
  padding: 0.5rem 1rem;
  border: none;
  border-radius: 4px;
  background: #3498db;
  color: #fff;
  cursor: pointer;
  transition: background 0.2s;
}
.usuario-form button:hover {
  background: #217dbb;
}
.admin-checkbox {
  display: flex;
  align-items: center;
  gap: 0.3rem;
}
.usuarios-list table {
  width: 100%;
  border-collapse: collapse;
  background: #fafbfc;
  border-radius: 6px;
  overflow: hidden;
}
.usuarios-list th, .usuarios-list td {
  padding: 0.75rem 1rem;
  text-align: left;
}
.usuarios-list th {
  background: #f0f4f8;
}
.usuarios-list tr:nth-child(even) {
  background: #f7fafd;
}
.admin-badge {
  color: #fff;
  background: #27ae60;
  padding: 0.2em 0.7em;
  border-radius: 12px;
  font-size: 0.95em;
}
.usuarios-list button {
  margin-right: 0.5rem;
  padding: 0.3rem 0.8rem;
  border: none;
  border-radius: 4px;
  background: #e67e22;
  color: #fff;
  cursor: pointer;
  transition: background 0.2s;
}
.usuarios-list button:hover {
  background: #ca6f1e;
}
.erro { color: red; margin-top: 1rem; }
</style> 