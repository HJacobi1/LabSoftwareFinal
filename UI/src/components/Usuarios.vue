<template>
  <div>
    <h2>Gerenciar Usuários</h2>
    <form @submit.prevent="salvarUsuario">
      <input v-model="novoUsuario.email" type="email" placeholder="Email" required />
      <input v-model="novoUsuario.senha" type="password" placeholder="Senha" :required="!editando" />
      <button type="submit">{{ editando ? 'Atualizar' : 'Cadastrar' }}</button>
      <button v-if="editando" @click.prevent="cancelarEdicao">Cancelar</button>
    </form>
    <ul>
      <li v-for="usuario in usuarios" :key="usuario.id">
        {{ usuario.email }}
        <button @click="editarUsuario(usuario)">Editar</button>
        <button @click="deletarUsuario(usuario.id)">Deletar</button>
      </li>
    </ul>
    <div v-if="erro" class="erro">{{ erro }}</div>
  </div>
</template>

<script>
export default {
  data() {
    return {
      usuarios: [],
      novoUsuario: { email: '', senha: '' },
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
        const res = await fetch('/api/usuario', {
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
        if (this.editando) {
          await fetch(`/api/usuario/${this.editId}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json', 'Authorization': 'Bearer ' + localStorage.getItem('token') },
            body: JSON.stringify(this.novoUsuario)
          });
        } else {
          await fetch('/api/usuario/register', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json', 'Authorization': 'Bearer ' + localStorage.getItem('token') },
            body: JSON.stringify(this.novoUsuario)
          });
        }
        this.novoUsuario = { email: '', senha: '' };
        this.editando = false;
        this.editId = null;
        await this.carregarUsuarios();
      } catch (e) {
        this.erro = 'Erro ao salvar usuário';
      }
    },
    editarUsuario(usuario) {
      this.novoUsuario = { email: usuario.email, senha: '' };
      this.editando = true;
      this.editId = usuario.id;
    },
    cancelarEdicao() {
      this.novoUsuario = { email: '', senha: '' };
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
.erro { color: red; margin-top: 1rem; }
form { margin-bottom: 2rem; }
input { margin-right: 0.5rem; }
button { margin-left: 0.5rem; }
</style> 