<template>
  <div class="solicitacao">
    <div class="header">
      <h2>Cadastro de Eventos</h2>
      <button @click="showForm = !showForm" class="btn-toggle">
        {{ showForm ? 'Ocultar Formulário' : 'Nova Solicitação' }}
      </button>
    </div>

    <!-- Formulário de Cadastro -->
    <div v-if="showForm" class="form-container">
      <form @submit.prevent="salvarSolicitacao" class="request-form">
        <div class="form-grid">
          <!-- Data -->
          <div class="form-group">
            <label for="data">Data *</label>
            <input
              id="data"
              v-model="solicitacao.Data"
              type="date"
              required
              class="form-control"
            />
          </div>

          <!-- Tipo de Manutenção/Calibração -->
          <div class="form-group">
            <label for="tipoMC">Tipo de Manutenção/Calibração *</label>
            <select
              id="tipoMC"
              v-model="solicitacao.TipoMC"
              required
              class="form-control"
            >
              <option value="">Selecione o tipo</option>
              <option value="Calibracao">Calibração</option>
              <option value="Manutencao">Manutenção</option>
            </select>
          </div>

          <!-- ID do Equipamento -->
          <div class="form-group">
            <label for="idEquipamento">ID do Equipamento *</label>
            <select
              id="idEquipamento"
              v-model="solicitacao.IdEquipamento"
              required
              class="form-control"
            >
              <option value="">Selecione o equipamento</option>
              <option
                v-for="equip in equipamentos"
                :key="equip.Id"
                :value="equip.Id"
              >
                {{ equip.Identificacao }} - {{ equip.NroPatrimonio }}
              </option>
            </select>
          </div>
        </div>

        <div class="form-actions">
          <button type="button" @click="limparFormulario" class="btn btn-secondary">
            Limpar
          </button>
          <button type="submit" class="btn btn-primary" :disabled="loading">
            {{ loading ? 'Salvando...' : 'Salvar Solicitação' }}
          </button>
        </div>
      </form>
    </div>

    <!-- Lista de Solicitações -->
    <div class="request-list">
      <h3>Solicitações Cadastradas</h3>
      <div v-if="solicitacoes.length === 0" class="empty-state">
        <p>Nenhuma solicitação cadastrada ainda.</p>
      </div>
      <div v-else class="request-grid">
        <div
          v-for="solic in solicitacoes"
          :key="solic.Id"
          class="request-card"
        >
          <div class="card-header">
            <h4>Solicitação #{{ solic.Id }}</h4>
            <span class="tipo" :class="getTipoClass(solic.TipoMC)">
              {{ getTipoLabel(solic.TipoMC) }}
            </span>
          </div>
          <div class="card-body">
            <p><strong>Data:</strong> {{ formatDate(solic.Data) }}</p>
            <p><strong>Equipamento:</strong> {{ getEquipamentoInfo(solic.IdEquipamento) }}</p>
            <p><strong>Status:</strong> {{ getStatusLabel(solic) }}</p>
            <p><strong>Data de Criação:</strong> {{ formatDate(solic.CreatedAt) }}</p>
          </div>
          <div class="card-actions">
            <button @click="editarSolicitacao(solic)" class="btn btn-small btn-secondary">
              Editar
            </button>
            <button @click="excluirSolicitacao(solic.Id)" class="btn btn-small btn-danger">
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
const solicitacoes = ref([])
const equipamentos = ref([])

// Modelo da solicitação
const solicitacao = reactive({
  Data: new Date().toISOString().split('T')[0],
  TipoMC: '',
  IdEquipamento: ''
})

// Métodos
const salvarSolicitacao = async () => {
  try {
    loading.value = true
    
    // Aqui você faria a chamada para a API
    const response = await fetch('/api/solicitacoes', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(solicitacao)
    })

    if (response.ok) {
      alert('Solicitação salva com sucesso!')
      limparFormulario()
      carregarSolicitacoes()
    } else {
      alert('Erro ao salvar solicitação')
    }
  } catch (error) {
    console.error('Erro:', error)
    alert('Erro ao salvar solicitação')
  } finally {
    loading.value = false
  }
}

const limparFormulario = () => {
  Object.assign(solicitacao, {
    Data: new Date().toISOString().split('T')[0],
    TipoMC: '',
    IdEquipamento: ''
  })
}

const carregarSolicitacoes = async () => {
  try {
    const response = await fetch('/api/solicitacoes')
    if (response.ok) {
      solicitacoes.value = await response.json()
    }
  } catch (error) {
    console.error('Erro ao carregar solicitações:', error)
  }
}

const carregarEquipamentos = async () => {
  try {
    const response = await fetch('/api/equipamentos')
    if (response.ok) {
      equipamentos.value = await response.json()
    }
  } catch (error) {
    console.error('Erro ao carregar equipamentos:', error)
  }
}

const editarSolicitacao = (solic) => {
  Object.assign(solicitacao, solic)
  showForm.value = true
}

const excluirSolicitacao = async (id) => {
  if (confirm('Tem certeza que deseja excluir esta solicitação?')) {
    try {
      const response = await fetch(`/api/solicitacoes/${id}`, {
        method: 'DELETE'
      })
      
      if (response.ok) {
        alert('Solicitação excluída com sucesso!')
        carregarSolicitacoes()
      } else {
        alert('Erro ao excluir solicitação')
      }
    } catch (error) {
      console.error('Erro:', error)
      alert('Erro ao excluir solicitação')
    }
  }
}

const getTipoLabel = (tipo) => {
  const labels = {
    'Calibracao': 'Calibração',
    'Manutencao': 'Manutenção'
  }
  return labels[tipo] || tipo
}

const getTipoClass = (tipo) => {
  const classes = {
    'Calibracao': 'tipo-calibracao',
    'Manutencao': 'tipo-manutencao'
  }
  return classes[tipo] || ''
}

const getEquipamentoInfo = (idEquipamento) => {
  const equip = equipamentos.value.find(e => e.Id === idEquipamento)
  return equip ? `${equip.Identificacao} (${equip.NroPatrimonio})` : 'Equipamento não encontrado'
}

const getStatusLabel = (solicitacao) => {
  // Aqui você pode implementar lógica de status baseada em outros campos
  // Por enquanto, vamos usar um status básico
  return 'Pendente'
}

const formatDate = (date) => {
  return new Date(date).toLocaleDateString('pt-BR')
}

// Carregar dados ao montar o componente
onMounted(() => {
  carregarSolicitacoes()
  carregarEquipamentos()
})
</script>

<style scoped>
.solicitacao {
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
  background-color: #fd7e14;
  color: white;
  border: none;
  padding: 10px 20px;
  border-radius: 5px;
  cursor: pointer;
  font-size: 14px;
  transition: background-color 0.3s;
}

.btn-toggle:hover {
  background-color: #e8690b;
}

.form-container {
  background-color: #f8f9fa;
  padding: 30px;
  border-radius: 10px;
  margin-bottom: 30px;
  box-shadow: 0 2px 10px rgba(0,0,0,0.1);
}

.request-form {
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
  border-color: #fd7e14;
  box-shadow: 0 0 0 3px rgba(253,126,20,0.1);
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
  background-color: #fd7e14;
  color: white;
}

.btn-primary:hover:not(:disabled) {
  background-color: #e8690b;
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

.request-list h3 {
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

.request-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
  gap: 20px;
}

.request-card {
  background-color: white;
  border-radius: 10px;
  padding: 20px;
  box-shadow: 0 2px 10px rgba(0,0,0,0.1);
  transition: transform 0.3s, box-shadow 0.3s;
}

.request-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 20px rgba(0,0,0,0.15);
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 15px;
  padding-bottom: 10px;
  border-bottom: 1px solid #eee;
}

.card-header h4 {
  margin: 0;
  color: #333;
  font-size: 18px;
}

.tipo {
  padding: 4px 12px;
  border-radius: 20px;
  font-size: 12px;
  font-weight: 600;
  text-transform: uppercase;
}

.tipo-calibracao {
  background-color: #d4edda;
  color: #155724;
}

.tipo-manutencao {
  background-color: #fff3cd;
  color: #856404;
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
  
  .request-grid {
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
  
  .card-header {
    flex-direction: column;
    align-items: flex-start;
    gap: 10px;
  }
}
</style> 