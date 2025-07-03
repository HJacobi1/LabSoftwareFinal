<template>
  <div class="laboratorio">
    <div class="header">
      <h2>Cadastro de Laboratórios</h2>
      <button @click="showForm = !showForm" class="btn-toggle">
        {{ showForm ? 'Ocultar Formulário' : 'Novo Laboratório' }}
      </button>
    </div>

    <!-- Formulário de Cadastro -->
    <div v-if="showForm" class="form-container">
      <form @submit.prevent="salvarLaboratorio" class="laboratory-form">
        <div class="form-grid">
          <!-- Código -->
          <div class="form-group">
            <label for="codigo">Código *</label>
            <input
              id="codigo"
              v-model.number="laboratorio.codigo"
              type="number"
              required
              placeholder="Digite o código do laboratório"
              class="form-control"
            />
          </div>

          <!-- Nome -->
          <div class="form-group">
            <label for="nome">Nome *</label>
            <input
              id="nome"
              v-model="laboratorio.nome"
              type="text"
              required
              placeholder="Digite o nome do laboratório"
              class="form-control"
            />
          </div>

          <!-- Endereço -->
          <div class="form-group full-width">
            <label for="endereco">Endereço *</label>
            <textarea
              id="endereco"
              v-model="laboratorio.endereco"
              required
              placeholder="Digite o endereço completo do laboratório"
              class="form-control"
              rows="3"
            ></textarea>
          </div>
        </div>

        <div class="form-actions">
          <button type="button" @click="limparFormulario" class="btn btn-secondary">
            Limpar
          </button>
          <button type="submit" class="btn btn-primary" :disabled="loading">
            {{ loading ? 'Salvando...' : 'Salvar Laboratório' }}
          </button>
        </div>
      </form>
    </div>

    <!-- Lista de Laboratórios -->
    <div class="laboratory-list">
      <h3>Laboratórios Cadastrados</h3>
      <div v-if="laboratorios.length === 0" class="empty-state">
        <p>Nenhum laboratório cadastrado ainda.</p>
      </div>
      <div v-else class="laboratory-grid">
        <div
          v-for="lab in laboratorios"
          :key="lab.Id"
          class="laboratory-card"
        >
          <div class="card-header">
            <h4>{{ lab.nome }}</h4>
            <span class="codigo">Código: {{ lab.codigo }}</span>
          </div>
          <div class="card-body">
            <p><strong>Endereço:</strong> {{ lab.endereco }}</p>
            <p><strong>Responsáveis:</strong> {{ lab.responsaveis?.length || 0 }} pessoa(s)</p>
            <p><strong>Equipamentos:</strong> {{ lab.equipamentos?.length || 0 }} equipamento(s)</p>
            <p><strong>Data de Criação:</strong> {{ formatDate(lab.dataCriacao) }}</p>
          </div>
          <div class="card-actions">
            <button @click="editarLaboratorio(lab)" class="btn btn-small btn-secondary">
              Editar
            </button>
            <button @click="excluirLaboratorio(lab.id)" class="btn btn-small btn-danger">
              Excluir
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'

// Estado do componente
const showForm = ref(false)
const loading = ref(false)
const laboratorios = ref([])

// Modelo do laboratório
const laboratorio = reactive({
  codigo: null,
  nome: '',
  endereco: ''
})

// Métodos
const salvarLaboratorio = async () => {
  try {
    loading.value = true     
    const response = await fetch('/api/Laboratorio', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(laboratorio)
    })

    if (response.ok) {
      alert('Laboratório salvo com sucesso!')
      limparFormulario()
      carregarLaboratorios()
    } else {
      alert('Erro ao salvar laboratório')
    }
  } catch (error) {
    console.error('Erro:', error)
    alert('Erro ao salvar laboratório')
  } finally {
    loading.value = false
  }
}

const limparFormulario = () => {
  Object.assign(laboratorio, {
    codigo: null,
    nome: '',
    endereco: ''
  })
}

const carregarLaboratorios = async () => {
  try {    
    const response = await fetch('/api/laboratorio')
    if (response.ok) {
      laboratorios.value = await response.json()
    }
  } catch (error) {
    console.error('Erro ao carregar laboratórios:', error)
  }
}

const editarLaboratorio = (lab) => {
  Object.assign(laboratorio, lab)
  showForm.value = true
}

const excluirLaboratorio = async (id) => {
  if (confirm('Tem certeza que deseja excluir este laboratório?')) {
    try {
      const response = await fetch(`/api/laboratorio/${id}`, {
        method: 'DELETE'
      })
      if (response.ok) {
        alert('Laboratório excluído com sucesso!')
        carregarLaboratorios()
      } else {
        alert('Erro ao excluir laboratório')
      }
    } catch (error) {
      console.error('Erro:', error)
      alert('Erro ao excluir laboratório')
    }
  }
}

const formatDate = (date) => {
  return new Date(date).toLocaleDateString('pt-BR')
}

// Carregar laboratórios ao montar o componente
onMounted(() => {
  carregarLaboratorios()
})
</script>

<style scoped>
.laboratorio {
  max-width: 1200px;
  margin: 0 auto;
  padding: 20px;
}

.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 30px;
  padding-bottom: 20px;
  border-bottom: 2px solid #e0e0e0;
}

.btn-toggle {
  background-color: #28a745;
  color: white;
  border: none;
  padding: 10px 20px;
  border-radius: 5px;
  cursor: pointer;
  font-size: 14px;
  transition: background-color 0.3s;
}

.btn-toggle:hover {
  background-color: #218838;
}

.form-container {
  background-color: #f8f9fa;
  padding: 30px;
  border-radius: 10px;
  margin-bottom: 30px;
  box-shadow: 0 2px 10px rgba(0,0,0,0.1);
}

.laboratory-form {
  width: 100%;
}

.form-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 20px;
  margin-bottom: 30px;
}

.form-group {
  display: flex;
  flex-direction: column;
}

.form-group.full-width {
  grid-column: 1 / -1;
}

.form-group label {
  margin-bottom: 8px;
  font-weight: 600;
  color: #333;
}

.form-control {
  padding: 12px;
  border: 2px solid #ddd;
  border-radius: 5px;
  font-size: 14px;
  transition: border-color 0.3s;
}

.form-control:focus {
  outline: none;
  border-color: #28a745;
  box-shadow: 0 0 0 3px rgba(40,167,69,0.1);
}

.form-control::placeholder {
  color: #999;
}

.form-actions {
  display: flex;
  gap: 15px;
  justify-content: flex-end;
}

.btn {
  padding: 12px 24px;
  border: none;
  border-radius: 5px;
  cursor: pointer;
  font-size: 14px;
  font-weight: 600;
  transition: all 0.3s;
}

.btn-primary {
  background-color: #28a745;
  color: white;
}

.btn-primary:hover:not(:disabled) {
  background-color: #218838;
}

.btn-primary:disabled {
  background-color: #ccc;
  cursor: not-allowed;
}

.btn-secondary {
  background-color: #6c757d;
  color: white;
}

.btn-secondary:hover {
  background-color: #545b62;
}

.btn-danger {
  background-color: #dc3545;
  color: white;
}

.btn-danger:hover {
  background-color: #c82333;
}

.btn-small {
  padding: 6px 12px;
  font-size: 12px;
}

.laboratory-list h3 {
  margin-bottom: 20px;
  color: #333;
}

.empty-state {
  text-align: center;
  padding: 40px;
  background-color: #f8f9fa;
  border-radius: 8px;
  color: #666;
}

.laboratory-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
  gap: 20px;
}

.laboratory-card {
  background-color: white;
  border-radius: 10px;
  padding: 20px;
  box-shadow: 0 2px 10px rgba(0,0,0,0.1);
  transition: transform 0.3s, box-shadow 0.3s;
}

.laboratory-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 20px rgba(0,0,0,0.15);
}

.card-header {
  margin-bottom: 15px;
  padding-bottom: 10px;
  border-bottom: 1px solid #eee;
}

.card-header h4 {
  margin: 0 0 5px 0;
  color: #333;
  font-size: 18px;
}

.codigo {
  color: #666;
  font-size: 14px;
  font-weight: 500;
}

.card-body p {
  margin: 8px 0;
  color: #555;
  font-size: 14px;
}

.card-body strong {
  color: #333;
}

.card-actions {
  display: flex;
  gap: 10px;
  margin-top: 15px;
  padding-top: 15px;
  border-top: 1px solid #eee;
}

@media (max-width: 768px) {
  .form-grid {
    grid-template-columns: 1fr;
  }
  
  .laboratory-grid {
    grid-template-columns: 1fr;
  }
  
  .header {
    flex-direction: column;
    gap: 15px;
    text-align: center;
  }
  
  .form-actions {
    flex-direction: column;
  }
}
</style> 