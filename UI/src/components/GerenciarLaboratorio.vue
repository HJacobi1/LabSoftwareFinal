<template>
  <div class="gerenciar-laboratorio">
    <div class="header">
      <h2>Gerenciar Laboratório e Equipamentos</h2>
    </div>

    <!-- Seletor de Laboratório -->
    <div class="laboratory-selector">
      <div class="selector-container">
        <label for="laboratorioSelect">Selecione o Laboratório:</label>
        <select
          id="laboratorioSelect"
          v-model="laboratorioSelecionado"
          @change="carregarEquipamentosLaboratorio"
          class="form-control"
        >
          <option value="">Escolha um laboratório...</option>
          <option
            v-for="lab in laboratorios"
            :key="lab.Id"
            :value="lab.Id"
          >
            {{ lab.Codigo }} - {{ lab.Nome }}
          </option>
        </select>
      </div>
    </div>

    <!-- Informações do Laboratório Selecionado -->
    <div v-if="laboratorioAtual" class="laboratory-info">
      <div class="info-card">
        <div class="info-header">
          <h3>{{ laboratorioAtual.Nome }}</h3>
          <span class="codigo">Código: {{ laboratorioAtual.Codigo }}</span>
        </div>
        <div class="info-body">
          <p><strong>Endereço:</strong> {{ laboratorioAtual.Endereco }}</p>
          <p><strong>Total de Equipamentos:</strong> {{ equipamentosLaboratorio.length }}</p>
          <p><strong>Responsáveis:</strong> {{ laboratorioAtual.Responsaveis?.length || 0 }} pessoa(s)</p>
        </div>
      </div>
    </div>

    <!-- Lista de Equipamentos do Laboratório -->
    <div v-if="laboratorioAtual" class="equipment-section">
      <div class="section-header">
        <h3>Equipamentos do Laboratório</h3>
        <div class="section-actions">
          <button @click="adicionarEquipamento" class="btn btn-primary">
            <i class="fas fa-plus"></i> Adicionar Equipamento
          </button>
          <button @click="exportarRelatorio" class="btn btn-secondary">
            <i class="fas fa-download"></i> Exportar Relatório
          </button>
        </div>
      </div>

      <!-- Filtros e Busca -->
      <div class="filters">
        <div class="search-box">
          <input
            v-model="filtroBusca"
            type="text"
            placeholder="Buscar equipamentos..."
            class="form-control"
          />
          <i class="fas fa-search search-icon"></i>
        </div>
        <div class="filter-options">
          <select v-model="filtroTipo" class="form-control">
            <option value="">Todos os tipos</option>
            <option value="Analogico">Analógico</option>
            <option value="Digital">Digital</option>
            <option value="NaoAplicavel">Não Aplicável</option>
          </select>
        </div>
      </div>

      <!-- Lista de Equipamentos -->
       <div>
      <!-- <div v-if="equipamentosFiltrados.length === 0" class="empty-state">
        <p v-if="equipamentosLaboratorio.length === 0">
          Este laboratório não possui equipamentos cadastrados.
        </p>
        <p v-else>
          Nenhum equipamento encontrado com os filtros aplicados.
        </p>
      </div>
      
      <div v-else class="equipment-grid"> -->
        <div
          v-for="equip in equipamentosFiltrados"
          :key="equip.Id"
          class="equipment-card"
        >
          <div class="card-header">
            <h4>{{ equip.Identificacao }}</h4>
            <span class="patrimonio">Patrimônio: {{ equip.NroPatrimonio }}</span>
          </div>
          <div class="card-body">
            <p><strong>Descrição:</strong> {{ equip.Descricao }}</p>
            <p><strong>Marca:</strong> {{ equip.Marca }}</p>
            <p><strong>Tipo:</strong> 
              <span class="tipo-badge" :class="getTipoClass(equip.TipoAD)">
                {{ getTipoLabel(equip.TipoAD) }}
              </span>
            </p>
            <p><strong>Nº Série:</strong> {{ equip.NroSerie || 'N/A' }}</p>
            <p><strong>Certificado:</strong> {{ equip.CertificadoCalibracao || 'N/A' }}</p>
            <p><strong>Data de Entrada:</strong> {{ formatDate(equip.DataEntrada) }}</p>
          </div>
          <div class="card-actions">
            <button @click="editarEquipamento(equip)" class="btn btn-small btn-secondary">
              <i class="fas fa-edit"></i> Editar
            </button>
            <button @click="verDetalhes(equip)" class="btn btn-small btn-info">
              <i class="fas fa-eye"></i> Detalhes
            </button>
            <button @click="removerEquipamento(equip.Id)" class="btn btn-small btn-danger">
              <i class="fas fa-trash"></i> Remover
            </button>
          </div>
        </div>
      </div>

      <!-- Estatísticas -->
      <div v-if="equipamentosLaboratorio.length > 0" class="statistics">
        <div class="stat-card">
          <h4>Total</h4>
          <span class="stat-number">{{ equipamentosLaboratorio.length }}</span>
        </div>
        <div class="stat-card">
          <h4>Analógicos</h4>
          <span class="stat-number">{{ estatisticas.analogicos }}</span>
        </div>
        <div class="stat-card">
          <h4>Digitais</h4>
          <span class="stat-number">{{ estatisticas.digitais }}</span>
        </div>
        <div class="stat-card">
          <h4>Não Aplicável</h4>
          <span class="stat-number">{{ estatisticas.naoAplicavel }}</span>
        </div>
      </div>
    </div>

    <!-- Modal de Adicionar/Editar Equipamento -->
    <div v-if="showModal" class="modal-overlay" @click="fecharModal">
      <div class="modal-content" @click.stop>
        <div class="modal-header">
          <h3>{{ editandoEquipamento ? 'Editar' : 'Adicionar' }} Equipamento</h3>
          <button @click="fecharModal" class="btn-close">
            <i class="fas fa-times"></i>
          </button>
        </div>
        <div class="modal-body">
          <form @submit.prevent="salvarEquipamento">
            <div class="form-grid">
              <div class="form-group">
                <label for="nroPatrimonio">Número do Patrimônio *</label>
                <input
                  id="nroPatrimonio"
                  v-model="equipamentoForm.NroPatrimonio"
                  type="text"
                  required
                  class="form-control"
                />
              </div>
              <div class="form-group">
                <label for="identificacao">Identificação *</label>
                <input
                  id="identificacao"
                  v-model="equipamentoForm.Identificacao"
                  type="text"
                  required
                  class="form-control"
                />
              </div>
              <div class="form-group full-width">
                <label for="descricao">Descrição *</label>
                <textarea
                  id="descricao"
                  v-model="equipamentoForm.Descricao"
                  required
                  class="form-control"
                  rows="3"
                ></textarea>
              </div>
              <div class="form-group">
                <label for="tipoAD">Tipo *</label>
                <select
                  id="tipoAD"
                  v-model="equipamentoForm.TipoAD"
                  required
                  class="form-control"
                >
                  <option value="">Selecione</option>
                  <option value="Analogico">Analógico</option>
                  <option value="Digital">Digital</option>
                  <option value="NaoAplicavel">Não Aplicável</option>
                </select>
              </div>
              <div class="form-group">
                <label for="marca">Marca *</label>
                <input
                  id="marca"
                  v-model="equipamentoForm.Marca"
                  type="text"
                  required
                  class="form-control"
                />
              </div>
              <div class="form-group">
                <label for="nroSerie">Número de Série</label>
                <input
                  id="nroSerie"
                  v-model="equipamentoForm.NroSerie"
                  type="text"
                  class="form-control"
                />
              </div>
              <div class="form-group">
                <label for="certificado">Certificado de Calibração</label>
                <input
                  id="certificado"
                  v-model="equipamentoForm.CertificadoCalibracao"
                  type="text"
                  class="form-control"
                />
              </div>
              <div class="form-group">
                <label for="dataEntrada">Data de Entrada *</label>
                <input
                  id="dataEntrada"
                  v-model="equipamentoForm.DataEntrada"
                  type="date"
                  required
                  class="form-control"
                />
              </div>
            </div>
            <div class="modal-actions">
              <button type="button" @click="fecharModal" class="btn btn-secondary">
                Cancelar
              </button>
              <button type="submit" class="btn btn-primary" :disabled="loading">
                {{ loading ? 'Salvando...' : 'Salvar' }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from 'vue'
import Equipamentos from './Equipamentos.vue'

// Estado do componente
const laboratorios = ref([])
const laboratorioSelecionado = ref('')
const laboratorioAtual = {
  Codigo: 1,
  Endereco: "Rua de Teste, Nro 1",
  Nome:"Laboratorio Teste",
  Responsaveis:[],
  Equipamentos: [{
    Id: 1,
    Identificacao: 1,
    NroPatrimonio: 12,
    Marca: "Teste",
    Descricao: "Teste",
    TipoAD: 1,
    NroSerie: "123",
    CertificadoCalibracao: "ABC123",    
}]
}
//ref(null)
const equipamentosLaboratorio = ref([])
const filtroBusca = ref('')
const filtroTipo = ref('')
const showModal = ref(false)
const editandoEquipamento = ref(false)
const loading = ref(false)

// Formulário do equipamento
const equipamentoForm = reactive({
  NroPatrimonio: '',
  Identificacao: '',
  Descricao: '',
  TipoAD: '',
  Marca: '',
  CertificadoCalibracao: '',
  NroSerie: '',
  DataEntrada: new Date().toISOString().split('T')[0],
  CodLaboratorio: null
})

// Computed properties
const equipamentosFiltrados = [{
    Id: 1,
    Identificacao: "Balança",
    NroPatrimonio: 12,
    Marca: "Teste",
    Descricao: "Teste",
    TipoAD: 1,
    NroSerie: "123",
    CertificadoCalibracao: "ABC123",    
}]
// computed(() => {
//   let filtrados = equipamentosLaboratorio.value

//   if (filtroBusca.value) {
//     const busca = filtroBusca.value.toLowerCase()
//     filtrados = filtrados.filter(equip => 
//       equip.Identificacao.toLowerCase().includes(busca) ||
//       equip.NroPatrimonio.toLowerCase().includes(busca) ||
//       equip.Descricao.toLowerCase().includes(busca) ||
//       equip.Marca.toLowerCase().includes(busca)
//     )
//   }

//   if (filtroTipo.value) {
//     filtrados = filtrados.filter(equip => equip.TipoAD === filtroTipo.value)
//   }

//   return filtrados
// })

const estatisticas = computed(() => {
  const stats = {
    analogicos: 0,
    digitais: 0,
    naoAplicavel: 0
  }

  equipamentosLaboratorio.value.forEach(equip => {
    switch (equip.TipoAD) {
      case 'Analogico':
        stats.analogicos++
        break
      case 'Digital':
        stats.digitais++
        break
      case 'NaoAplicavel':
        stats.naoAplicavel++
        break
    }
  })

  return stats
})

// Métodos
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

const carregarEquipamentosLaboratorio = async () => {
  if (!laboratorioSelecionado.value) {
    laboratorioAtual.value = null
    equipamentosLaboratorio.value = []
    return
  }

  try {
    // Carregar detalhes do laboratório
    const labResponse = await fetch(`/api/laboratorios/${laboratorioSelecionado.value}`)
    if (labResponse.ok) {
      laboratorioAtual.value = await labResponse.json()
    }

    // Carregar equipamentos do laboratório
    const equipResponse = await fetch(`/api/laboratorios/${laboratorioSelecionado.value}/equipamentos`)
    if (equipResponse.ok) {
      equipamentosLaboratorio.value = await equipResponse.json()
    }
  } catch (error) {
    console.error('Erro ao carregar dados do laboratório:', error)
  }
}

const adicionarEquipamento = () => {
  editandoEquipamento.value = false
  limparFormulario()
  equipamentoForm.CodLaboratorio = laboratorioSelecionado.value
  showModal.value = true
}

const editarEquipamento = (equip) => {
  editandoEquipamento.value = true
  Object.assign(equipamentoForm, equip)
  showModal.value = true
}

const salvarEquipamento = async () => {
  try {
    loading.value = true
    
    const url = editandoEquipamento.value 
      ? `/api/equipamentos/${equipamentoForm.Id}`
      : '/api/equipamentos'
    
    const method = editandoEquipamento.value ? 'PUT' : 'POST'
    
    const response = await fetch(url, {
      method,
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(equipamentoForm)
    })

    if (response.ok) {
      alert(editandoEquipamento.value ? 'Equipamento atualizado!' : 'Equipamento adicionado!')
      fecharModal()
      carregarEquipamentosLaboratorio()
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

const removerEquipamento = async (id) => {
  if (confirm('Tem certeza que deseja remover este equipamento?')) {
    try {
      const response = await fetch(`/api/equipamentos/${id}`, {
        method: 'DELETE'
      })
      
      if (response.ok) {
        alert('Equipamento removido!')
        carregarEquipamentosLaboratorio()
      } else {
        alert('Erro ao remover equipamento')
      }
    } catch (error) {
      console.error('Erro:', error)
      alert('Erro ao remover equipamento')
    }
  }
}

const verDetalhes = (equip) => {
  // Implementar visualização detalhada
  alert(`Detalhes do equipamento: ${equip.Identificacao}`)
}

const exportarRelatorio = () => {
  // Implementar exportação de relatório
  alert('Funcionalidade de exportação será implementada')
}

const fecharModal = () => {
  showModal.value = false
  editandoEquipamento.value = false
  limparFormulario()
}

const limparFormulario = () => {
  Object.assign(equipamentoForm, {
    NroPatrimonio: '',
    Identificacao: '',
    Descricao: '',
    TipoAD: '',
    Marca: '',
    CertificadoCalibracao: '',
    NroSerie: '',
    DataEntrada: new Date().toISOString().split('T')[0],
    CodLaboratorio: laboratorioSelecionado.value
  })
}

const getTipoLabel = (tipo) => {
  const labels = {
    'Analogico': 'Analógico',
    'Digital': 'Digital',
    'NaoAplicavel': 'Não Aplicável'
  }
  return labels[tipo] || tipo
}

const getTipoClass = (tipo) => {
  const classes = {
    'Analogico': 'tipo-analogico',
    'Digital': 'tipo-digital',
    'NaoAplicavel': 'tipo-nao-aplicavel'
  }
  return classes[tipo] || ''
}

const formatDate = (date) => {
  return new Date(date).toLocaleDateString('pt-BR')
}

// Carregar dados ao montar o componente
onMounted(() => {
  carregarLaboratorios()
})
</script>

<style scoped>
.gerenciar-laboratorio {
  max-width: 1400px;
  margin: 0 auto;
  padding: 20px;
}

.header {
  margin-bottom: 30px;
  padding-bottom: 20px;
  border-bottom: 2px solid #e0e0e0;
}

.header h2 {
  margin: 0;
  color: #333;
}

.laboratory-selector {
  background-color: #f8f9fa;
  padding: 20px;
  border-radius: 10px;
  margin-bottom: 30px;
  box-shadow: 0 2px 10px rgba(0,0,0,0.1);
}

.selector-container {
  display: flex;
  align-items: center;
  gap: 15px;
}

.selector-container label {
  font-weight: 600;
  color: #333;
  min-width: 200px;
}

.form-control {
  padding: 12px;
  border: 2px solid #ddd;
  border-radius: 5px;
  font-size: 14px;
  transition: border-color 0.3s;
  flex: 1;
}

.form-control:focus {
  outline: none;
  border-color: #007bff;
  box-shadow: 0 0 0 3px rgba(0,123,255,0.1);
}

.laboratory-info {
  margin-bottom: 30px;
}

.info-card {
  background-color: white;
  padding: 20px;
  border-radius: 10px;
  box-shadow: 0 2px 10px rgba(0,0,0,0.1);
}

.info-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 15px;
  padding-bottom: 10px;
  border-bottom: 1px solid #eee;
}

.info-header h3 {
  margin: 0;
  color: #333;
}

.codigo {
  color: #666;
  font-weight: 500;
}

.info-body p {
  margin: 8px 0;
  color: #555;
}

.info-body strong {
  color: #333;
}

.equipment-section {
  background-color: white;
  border-radius: 10px;
  padding: 20px;
  box-shadow: 0 2px 10px rgba(0,0,0,0.1);
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
  padding-bottom: 15px;
  border-bottom: 1px solid #eee;
}

.section-header h3 {
  margin: 0;
  color: #333;
}

.section-actions {
  display: flex;
  gap: 10px;
}

.filters {
  display: flex;
  gap: 15px;
  margin-bottom: 20px;
  align-items: center;
}

.search-box {
  position: relative;
  flex: 1;
}

.search-box input {
  padding-right: 40px;
}

.search-icon {
  position: absolute;
  right: 12px;
  top: 50%;
  transform: translateY(-50%);
  color: #999;
}

.filter-options {
  min-width: 150px;
}

.empty-state {
  text-align: center;
  padding: 40px;
  color: #666;
  background-color: #f8f9fa;
  border-radius: 8px;
}

.equipment-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(400px, 1fr));
  gap: 20px;
  margin-bottom: 30px;
}

.equipment-card {
  background-color: #f8f9fa;
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

.tipo-badge {
  padding: 2px 8px;
  border-radius: 12px;
  font-size: 12px;
  font-weight: 600;
  text-transform: uppercase;
}

.tipo-analogico {
  background-color: #d4edda;
  color: #155724;
}

.tipo-digital {
  background-color: #cce5ff;
  color: #004085;
}

.tipo-nao-aplicavel {
  background-color: #f8d7da;
  color: #721c24;
}

.card-actions {
  display: flex;
  gap: 8px;
  margin-top: 15px;
  padding-top: 15px;
  border-top: 1px solid #eee;
}

.statistics {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
  gap: 15px;
  margin-top: 30px;
  padding-top: 20px;
  border-top: 1px solid #eee;
}

.stat-card {
  text-align: center;
  padding: 15px;
  background-color: #f8f9fa;
  border-radius: 8px;
}

.stat-card h4 {
  margin: 0 0 10px 0;
  color: #666;
  font-size: 14px;
  font-weight: 600;
}

.stat-number {
  font-size: 24px;
  font-weight: bold;
  color: #007bff;
}

.btn {
  padding: 8px 16px;
  border: none;
  border-radius: 5px;
  cursor: pointer;
  font-size: 14px;
  font-weight: 600;
  transition: all 0.3s;
  display: inline-flex;
  align-items: center;
  gap: 5px;
}

.btn-primary {
  background-color: #007bff;
  color: white;
}

.btn-primary:hover:not(:disabled) {
  background-color: #0056b3;
}

.btn-secondary {
  background-color: #6c757d;
  color: white;
}

.btn-secondary:hover {
  background-color: #545b62;
}

.btn-info {
  background-color: #17a2b8;
  color: white;
}

.btn-info:hover {
  background-color: #138496;
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

/* Modal Styles */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
}

.modal-content {
  background-color: white;
  border-radius: 10px;
  width: 90%;
  max-width: 800px;
  max-height: 90vh;
  overflow-y: auto;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.3);
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 20px;
  border-bottom: 1px solid #eee;
}

.modal-header h3 {
  margin: 0;
  color: #333;
}

.btn-close {
  background: none;
  border: none;
  font-size: 20px;
  cursor: pointer;
  color: #666;
  padding: 5px;
}

.btn-close:hover {
  color: #333;
}

.modal-body {
  padding: 20px;
}

.form-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 15px;
  margin-bottom: 20px;
}

.form-group {
  display: flex;
  flex-direction: column;
}

.form-group.full-width {
  grid-column: 1 / -1;
}

.form-group label {
  margin-bottom: 5px;
  font-weight: 600;
  color: #333;
}

.modal-actions {
  display: flex;
  gap: 10px;
  justify-content: flex-end;
  padding-top: 20px;
  border-top: 1px solid #eee;
}

@media (max-width: 768px) {
  .selector-container {
    flex-direction: column;
    align-items: stretch;
  }
  
  .section-header {
    flex-direction: column;
    gap: 15px;
    align-items: stretch;
  }
  
  .section-actions {
    justify-content: stretch;
  }
  
  .filters {
    flex-direction: column;
  }
  
  .equipment-grid {
    grid-template-columns: 1fr;
  }
  
  .statistics {
    grid-template-columns: repeat(2, 1fr);
  }
  
  .modal-content {
    width: 95%;
    margin: 10px;
  }
}
</style> 