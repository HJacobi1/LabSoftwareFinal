<template>
  <div class="equipamentos">
    <div class="header">
      <h2>Cadastro de Equipamentos</h2>
      <button @click="showForm = !showForm" class="btn-toggle">
        {{ showForm ? 'Ocultar Formulário' : 'Novo Equipamento' }}
      </button>
    </div>

    <!-- Formulário de Cadastro -->
    <div v-if="showForm" class="form-container">
      <form @submit.prevent="salvarEquipamento" class="equipment-form">
        <div class="form-grid">          
          <div class="form-group">
            <label for="identificacao">Identificação *</label>
            <input
              id="identificacao"
              v-model="equipamento.Identificacao"
              type="text"
              required
              class="form-control"
              
            />
          </div>
          <div class="form-group full-width">
            <label for="descricao">Descrição *</label>
            <textarea
              id="descricao"
              v-model="equipamento.Descricao"
              required
              class="form-control"
              rows="3"
              
            ></textarea>
          </div>
          <div class="form-group">
            <label for="tipoAD">Tipo *</label>
            <select
              id="tipoAD"
              v-model="equipamento.TipoAD"
              required
              class="form-control"
            >
              <option value="">Selecione o tipo</option>
              <option value="Analogico">Analógico</option>
              <option value="Digital">Digital</option>
              <option value="NaoAplicavel">Não Aplicável</option>
            </select>
          </div>
          <div class="form-group">
            <label for="marca">Marca *</label>
            <input
              id="marca"
              v-model="equipamento.Marca"
              type="text"
              required
              class="form-control"
            />
          </div>
        </div>
        <div class="form-actions">
          <button type="button" @click="limparFormulario" class="btn btn-secondary">
            Limpar
          </button>
          <button type="submit" class="btn btn-primary" :disabled="loading">
            {{ loading ? 'Salvando...' : 'Salvar Equipamento' }}
          </button>
        </div>
      </form>
    </div>

    <!-- Lista de Equipamentos -->
    <div class="equipment-list">
      <h3>Equipamentos Cadastrados</h3>
      <div v-if="equipamentos.length === 0" class="empty-state">
        <p>Nenhum equipamento cadastrado ainda.</p>
      </div>
      <div v-else class="equipment-grid">
        <div
          v-for="equip in equipamentos"
          :key="equip.id"
          class="equipment-card"
        >
          <div class="card-header">
            <h4>{{ equip.identificacao }}</h4>
          </div>
          <div class="card-body">
            <p><strong>Descrição:</strong> {{ equip.descricao }}</p>
            <p><strong>Marca:</strong> {{ equip.marca }}</p>
            <p><strong>Tipo:</strong> {{ getTipoLabel(equip.tipoAD) }}</p>
            <p><strong>Data de Cadastro:</strong> {{ formatDate(equip.createdAt) }}</p>
          </div>
          <div class="card-actions">
            <button @click="editarEquipamento(equip)" class="btn btn-small btn-secondary">
              Editar
            </button>
            <button @click="excluirEquipamento(equip.id)" class="btn btn-small btn-danger">
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
const equipamentos = ref([])

// Modelo do equipamento
const equipamento = reactive({
  Identificacao: '',
  Descricao: '',
  TipoAD: '',
  Marca: ''
})

const modelosEquipamento = ref([])
const modeloSelecionado = ref('')

// Métodos
const tipoADEnum = {
  'Analogico': 0,
  'Digital': 1,
  'NaoAplicavel': 2
}

const salvarEquipamento = async () => {
  try {
    loading.value = true
    // Converter TipoAD para valor numérico do enum
    const payload = { ...equipamento, TipoAD: tipoADEnum[equipamento.TipoAD] }
    const response = await fetch('/api/equipamento', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(payload)
    })
    if (response.ok) {
      alert('Equipamento salvo com sucesso!')
      limparFormulario()
      carregarEquipamentos()
    } else {
      alert('Erro ao salvar equipamento')
    }
  } catch (error) {
    console.error('Erro:', error)
    alert('Erro ao salvar equipamento')
  } finally {
    loading.value = false
  }
}

const limparFormulario = () => {
  Object.assign(equipamento, {
    Identificacao: '',
    Descricao: '',
    TipoAD: '',
    Marca: ''    
  })
}

const carregarEquipamentos = async () => {
  try {
    const response = await fetch('/api/equipamento')
    if (response.ok) {
      equipamentos.value = await response.json()
    }
  } catch (error) {
    console.error('Erro ao carregar equipamentos:', error)
  }
}

const editarEquipamento = (equip) => {
  Object.assign(equipamento, equip)
  showForm.value = true
}

const excluirEquipamento = async (id) => {
  if (confirm('Tem certeza que deseja excluir este equipamento?')) {
    try {
      const response = await fetch(`/api/equipamento/${id}`, {
        method: 'DELETE'
      })
      
      if (response.ok) {
        alert('Equipamento excluído com sucesso!')
        carregarEquipamentos()
      } else {
        alert('Erro ao excluir equipamento')
      }
    } catch (error) {
      console.error('Erro:', error)
      alert('Erro ao excluir equipamento')
    }
  }
}

const getTipoLabel = (tipo) => {
  const labels = {
    'Analogico': 'Analógico',
    'Digital': 'Digital',
    'NaoAplicavel': 'Não Aplicável'
  }
  console.log(tipo)
  return labels[tipo] || tipo
}

const formatDate = (date) => {
  return new Date(date).toLocaleDateString('pt-BR')
}

// Carregar equipamentos ao montar o componente
onMounted(() => {
  carregarEquipamentos()
})
</script>

<style scoped>
.equipamentos {
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
  background-color: #007bff;
  color: white;
  border: none;
  padding: 10px 20px;
  border-radius: 5px;
  cursor: pointer;
  font-size: 14px;
  transition: background-color 0.3s;
}

.btn-toggle:hover {
  background-color: #0056b3;
}

.form-container {
  background-color: #f8f9fa;
  padding: 30px;
  border-radius: 10px;
  margin-bottom: 30px;
  box-shadow: 0 2px 10px rgba(0,0,0,0.1);
}

.equipment-form {
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
  border-color: #007bff;
  box-shadow: 0 0 0 3px rgba(0,123,255,0.1);
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
  background-color: #007bff;
  color: white;
}

.btn-primary:hover:not(:disabled) {
  background-color: #0056b3;
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

.equipment-list h3 {
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

.equipment-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
  gap: 20px;
}

.equipment-card {
  background-color: white;
  border-radius: 10px;
  padding: 20px;
  box-shadow: 0 2px 10px rgba(0,0,0,0.1);
  transition: transform 0.3s, box-shadow 0.3s;
}

.equipment-card:hover {
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

.patrimonio {
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
  
  .equipment-grid {
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