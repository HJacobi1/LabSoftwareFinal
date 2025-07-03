<template>
  <div class="pessoa-form">
    <h2>{{ isEditing ? 'Editar Pessoa' : 'Nova Pessoa' }}</h2>
    
    <form @submit.prevent="handleSubmit" class="form-container">
      <div class="form-group">    
        <label for="nome">Nome</label>
        <input
          id="nome"
          v-model="formData.nome"
          type="text"
          :class="{ 'error': v$.nome.$error }"
          maxlength="100"
          required
        />
        <span class="error-message" v-if="v$.nome.$error">
          {{ v$.nome.$errors[0].$message }}
        </span>
      </div>

      <div class="form-group">
        <label for="contato">Contato</label>
        <input
          id="contato"
          v-model="formData.contato"
          type="text"
          :class="{ 'error': v$.contato.$error }"
          maxlength="50"
          required
        />
        <span class="error-message" v-if="v$.contato.$error">
          {{ v$.contato.$errors[0].$message }}
        </span>
      </div>

      <div class="form-group">
        <label for="email">Email</label>
        <input
          id="email"
          v-model="formData.email"
          type="email"
          :class="{ 'error': v$.email.$error }"
          maxlength="100"
          required
        />
        <span class="error-message" v-if="v$.email.$error">
          {{ v$.email.$errors[0].$message }}
        </span>
      </div>
      <div class="form-group">
        <label for="laboratorio">Laboratório</label>
        <select
          id="laboratorio"
          v-model="formData.laboratorioId"
          :class="['form-control', { 'error': v$.laboratorioId.$error } ]"
          required
        >
          <option value="">Selecione um laboratório</option>
          <option v-for="lab in laboratorios" :key="lab.id || lab.Id" :value="lab.id || lab.Id">
            {{ lab.nome || lab.Nome }}
          </option>
        </select>
        <span class="error-message" v-if="v$.laboratorioId.$error">
          {{ v$.laboratorioId.$errors[0].$message }}
        </span>
      </div>

      <div class="form-actions">
        <button type="submit" class="btn-primary">
          {{ isEditing ? 'Atualizar' : 'Cadastrar' }}
        </button>
        <button type="button" @click="resetForm" class="btn-secondary">
          Limpar
        </button>
      </div>
    </form>

    <!-- List of Pessoas -->
    <div class="pessoas-list" v-if="pessoas.length > 0">
      <h3>Lista de Pessoas</h3>
      <table>
        <thead>
          <tr>
            <th>Id</th>
            <th>Nome</th>
            <th>Contato</th>
            <th>Ações</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="pessoa in pessoas" :key="pessoa.id">
            <td>{{ pessoa.id }}</td>
            <td>{{ pessoa.nome }}</td>
            <td>{{ pessoa.contato }}</td>
            <td>
              <button @click="editPessoa(pessoa)" class="btn-edit">Editar</button>
              <button @click="deletePessoa(pessoa.id)" class="btn-delete">Excluir</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { useVuelidate } from '@vuelidate/core'
import { required, maxLength } from '@vuelidate/validators'
import axios from 'axios'

const API_URL = '/api/pessoa'

const pessoas = ref([])
const isEditing = ref(false)

// Adicionar campo laboratorioId e email ao formData
const formData = reactive({
  id: 0,
  nome: '',
  contato: '',
  laboratorioId: '',
  email: ''
})

// Adicionar laboratório e email ao rules
const rules = {
  nome: { required, maxLength: maxLength(100) },
  contato: { required, maxLength: maxLength(50) },
  laboratorioId: { required },
  email: { required, maxLength: maxLength(100) }
}

const v$ = useVuelidate(rules, formData)

// Carregar laboratórios para o select
const laboratorios = ref([])
const loadLaboratorios = async () => {
  try {
    const response = await axios.get('/api/laboratorio')
    laboratorios.value = response.data
  } catch (error) {
    console.error('Erro ao carregar laboratórios:', error)
    alert('Erro ao carregar laboratórios')
  }
}

const resetForm = () => {
  formData.id = 0
  formData.nome = ''
  formData.contato = ''
  formData.email = ''
  formData.laboratorioId = ''
  isEditing.value = false  
  v$.value.$reset()
}

const loadPessoas = async () => {
  try {
    const response = await axios.get(API_URL)    
    pessoas.value = response.data
  } catch (error) {
    console.error('Erro ao carregar pessoas:', error)
    alert('Erro ao carregar a lista de pessoas')
  }
}

// Atualizar handleSubmit para criar usuário após cadastrar pessoa
const handleSubmit = async () => {
  const isFormCorrect = await v$.value.$validate()
  if (!isFormCorrect) return

  try {
    let pessoaId = formData.id
    if (isEditing.value) {
      await axios.put(`${API_URL}/${formData.id}`, formData)
      pessoaId = formData.id
    } else {
      const pessoaResp = await axios.post(API_URL, formData)
      pessoaId = pessoaResp.data.id
      // Criar usuário com email, senha padrão e PessoaId
      await axios.post('/api/usuario/register', {
        email: formData.email,
        senha: '12345',
        isAdmin: false,
        pessoaId: pessoaId
      })
    }
    await loadPessoas()
    resetForm()
  } catch (error) {
    console.error('Erro ao salvar pessoa:', error)
    alert('Erro ao salvar os dados')
  }
}

const editPessoa = (pessoa) => {
  formData.id = pessoa.id
  formData.nome = pessoa.nome
  formData.contato = pessoa.contato
  formData.email = pessoa.email
  formData.laboratorioId = pessoa.laboratorioId
  isEditing.value = true
}

const deletePessoa = async (id) => {
  if (!confirm('Tem certeza que deseja excluir esta pessoa?')) return

  try {
    await axios.delete(`${API_URL}/${id}`)
    await loadPessoas()
  } catch (error) {
    console.error('Erro ao excluir pessoa:', error)
    alert('Erro ao excluir pessoa')
  }
}

onMounted(() => {
  loadPessoas()
  loadLaboratorios()
})
</script>

<style scoped>
.pessoa-form {
  max-width: 800px;
  margin: 0 auto;
  padding: 20px;
}

.form-container {
  background-color: #f5f5f5;
  padding: 20px;
  border-radius: 8px;
  margin-bottom: 20px;
}

.form-group {
  margin-bottom: 15px;
}

label {
  display: block;
  margin-bottom: 5px;
  font-weight: bold;
}

input {
  width: 100%;
  padding: 8px;
  border: 1px solid #ddd;
  border-radius: 4px;
}

input.error {
  border-color: #ff4444;
}

.error-message {
  color: #ff4444;
  font-size: 0.9em;
  margin-top: 5px;
  display: block;
}

.form-actions {
  margin-top: 20px;
  display: flex;
  gap: 10px;
}

button {
  padding: 8px 16px;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-weight: bold;
}

.btn-primary {
  background-color: #4CAF50;
  color: white;
}

.btn-secondary {
  background-color: #f0f0f0;
  color: #333;
}

.btn-edit {
  background-color: #2196F3;
  color: white;
  margin-right: 5px;
}

.btn-delete {
  background-color: #f44336;
  color: white;
}

table {
  width: 100%;
  border-collapse: collapse;
  margin-top: 20px;
}

th, td {
  padding: 12px;
  text-align: left;
  border-bottom: 1px solid #ddd;
}

th {
  background-color: #f5f5f5;
  font-weight: bold;
}

tr:hover {
  background-color: #f9f9f9;
}

.form-control {
  width: 100%;
  padding: 8px;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 1rem;
  margin-bottom: 5px;
}
.form-control.error {
  border-color: #ff4444;
}
</style>