<template>
  <div class="login-container">
    <h2>Login</h2>
    <form @submit.prevent="login">
      <input v-model="email" type="email" placeholder="Email" required />
      <input v-model="senha" type="password" placeholder="Senha" required />
      <button type="submit">Entrar</button>
      <div v-if="erro" class="erro">{{ erro }}</div>
    </form>
  </div>
</template>

<script>
export default {
  data() {
    return {
      email: '',
      senha: '',
      erro: ''
    };
  },
  methods: {
    async login() {
      this.erro = '';
      try {
        const response = await fetch('/api/usuario/login', {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({ email: this.email, senha: this.senha })
        });
        const data = await response.json();
        if (!response.ok) throw new Error(data);
        localStorage.setItem('token', data.token);
        this.$router.push('/');
      } catch (err) {
        this.erro = err.message || 'Erro ao fazer login';
      }
    }
  }
};
</script>

<style scoped>
.login-container {
  max-width: 400px;
  margin: 100px auto;
  padding: 2rem;
  border: 1px solid #ccc;
  border-radius: 8px;
  background: #fff;
  box-shadow: 0 2px 8px rgba(0,0,0,0.1);
}
input {
  display: block;
  width: 100%;
  margin-bottom: 1rem;
  padding: 0.5rem;
}
button {
  width: 100%;
  padding: 0.7rem;
  background: #1976d2;
  color: #fff;
  border: none;
  border-radius: 4px;
  font-size: 1rem;
}
.erro {
  color: red;
  margin-top: 1rem;
}
</style> 