<template>
  <div class="calendario">
    <div class="header">
      <h2>Calendário de Solicitações</h2>
      <div class="header-actions">
        <button @click="mesAnterior" class="btn btn-secondary">
          <i class="fas fa-chevron-left"></i> Mês Anterior
        </button>
        <span class="mes-atual">{{ mesAtualLabel }}</span>
        <button @click="mesProximo" class="btn btn-secondary">
          Próximo Mês <i class="fas fa-chevron-right"></i>
        </button>
        <button @click="irParaSolicitacoes" class="btn btn-primary">
          <i class="fas fa-list"></i> Ver Solicitações
        </button>
      </div>
    </div>

    <!-- Calendário -->
    <div class="calendar-container">
      <div class="calendar">
        <!-- Cabeçalho dos dias da semana -->
        <div class="calendar-header">
          <div v-for="dia in diasSemana" :key="dia" class="day-header">
            {{ dia }}
          </div>
        </div>

        <!-- Dias do calendário -->
        <div class="calendar-body">
          <div
            v-for="(dia, index) in diasCalendario"
            :key="index"
            class="day"
            :class="{
              empty: !dia.data,
              today: dia.hoje,
              'has-events': dia.eventos && dia.eventos.length > 0,
            }"
            @click="dia.data ? selecionarDia(dia) : null"
          >
            <span v-if="dia.data" class="day-number">{{ dia.numero }}</span>
            <div
              v-if="dia.eventos && dia.eventos.length > 0"
              class="event-indicators"
            >
              <div
                v-for="(evento, idx) in dia.eventos.slice(0, 3)"
                :key="idx"
                class="event-dot"
                :class="getEventoClass(evento.TipoMC)"
                :title="`${getTipoLabel(evento.TipoMC)} - ${getEquipamentoInfo(
                  evento.IdEquipamento
                )}`"
              ></div>
              <span v-if="dia.eventos.length > 3" class="more-events"
                >+{{ dia.eventos.length - 3 }}</span
              >
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Detalhes dos eventos do dia selecionado -->
    <div v-if="diaSelecionado" class="event-details">
      <div class="details-header">
        <h3>Eventos do dia {{ formatDate(diaSelecionado.data) }}</h3>
        <button @click="diaSelecionado = null" class="btn-close">
          <i class="fas fa-times"></i>
        </button>
      </div>

      <div
        v-if="diaSelecionado.eventos && diaSelecionado.eventos.length > 0"
        class="events-list"
      >
        <div
          v-for="evento in diaSelecionado.eventos"
          :key="evento.Id"
          class="event-card"
          :class="getEventoClass(evento.TipoMC)"
        >
          <div class="event-header">
            <span class="event-type">{{ getTipoLabel(evento.TipoMC) }}</span>
            <span class="event-id">#{{ evento.Id }}</span>
          </div>
          <div class="event-body">
            <p>
              <strong>Equipamento:</strong>
              {{ getEquipamentoInfo(evento.IdEquipamento) }}
            </p>
            <p v-if="evento.Descricao">
              <strong>Descrição:</strong> {{ evento.Descricao }}
            </p>
            <p><strong>Data:</strong> {{ formatDate(evento.Data) }}</p>
          </div>
          <div class="event-actions">
            <button
              @click="editarSolicitacao(evento)"
              class="btn btn-small btn-secondary"
            >
              <i class="fas fa-edit"></i> Editar
            </button>
            <button
              @click="excluirSolicitacao(evento.Id)"
              class="btn btn-small btn-danger"
            >
              <i class="fas fa-trash"></i> Excluir
            </button>
          </div>
        </div>
      </div>

      <div v-else class="no-events">
        <p>Nenhum evento agendado para este dia.</p>
        <button @click="novaSolicitacao" class="btn btn-primary">
          <i class="fas fa-plus"></i> Nova Solicitação
        </button>
      </div>
    </div>

    <!-- Estatísticas -->
    <div class="statistics">
      <div class="stat-card">
        <h4>Total de Solicitações</h4>
        <span class="stat-number">{{ totalSolicitacoes }}</span>
      </div>
      <div class="stat-card">
        <h4>Calibrações</h4>
        <span class="stat-number">{{ estatisticas.calibracoes }}</span>
      </div>
      <div class="stat-card">
        <h4>Manutenções</h4>
        <span class="stat-number">{{ estatisticas.manutencoes }}</span>
      </div>
      <div class="stat-card">
        <h4>Este Mês</h4>
        <span class="stat-number">{{ estatisticas.esteMes }}</span>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted, watch } from "vue"
import { useRouter } from "vue-router"

const router = useRouter()

// Estado do componente
const mesAtual = ref(new Date().getMonth())
const anoAtual = ref(new Date().getFullYear())
const solicitacoes = ref([])
const equipamentos = ref([])
const diaSelecionado = ref(null)
const loading = ref(false)

// Constantes
const diasSemana = ["Dom", "Seg", "Ter", "Qua", "Qui", "Sex", "Sáb"]

// Computed properties
const mesAtualLabel = computed(() => {
  const meses = [
    "Janeiro",
    "Fevereiro",
    "Março",
    "Abril",
    "Maio",
    "Junho",
    "Julho",
    "Agosto",
    "Setembro",
    "Outubro",
    "Novembro",
    "Dezembro",
  ]
  return `${meses[mesAtual.value]} ${anoAtual.value}`
})

const diasCalendario = computed(() => {
  const dias = []
  const primeiroDia = new Date(anoAtual.value, mesAtual.value, 1)
  const ultimoDia = new Date(anoAtual.value, mesAtual.value + 1, 0)
  const inicioSemana = primeiroDia.getDay()
  const totalDias = ultimoDia.getDate()
  const hoje = new Date()

  // Dias vazios no início
  for (let i = 0; i < inicioSemana; i++) {
    dias.push({ data: null, numero: "", eventos: [] })
  }

  // Dias do mês
  for (let dia = 1; dia <= totalDias; dia++) {
    const data = new Date(anoAtual.value, mesAtual.value, dia)
    const dataStr = data.toISOString().split("T")[0]
    const eventosDia = solicitacoes.value.filter((s) => {
      const dataSolicitacao = new Date(s.Data).toISOString().split("T")[0]
      return dataSolicitacao === dataStr
    })

    dias.push({
      data: data,
      numero: dia,
      hoje: data.toDateString() === hoje.toDateString(),
      eventos: eventosDia,
    })
  }

  return dias
})

const totalSolicitacoes = computed(() => solicitacoes.value.length)

const estatisticas = computed(() => {
  const calibracoes = solicitacoes.value.filter(
    (s) => s.TipoMC === "Calibracao"
  ).length
  const manutencoes = solicitacoes.value.filter(
    (s) => s.TipoMC === "Manutencao"
  ).length

  const mesAtualStr = `${anoAtual.value}-${String(mesAtual.value + 1).padStart(
    2,
    "0"
  )}`
  const esteMes = solicitacoes.value.filter((s) => {
    const dataSolicitacao = new Date(s.Data).toISOString().split("T")[0]
    return dataSolicitacao.startsWith(mesAtualStr)
  }).length

  return {
    calibracoes,
    manutencoes,
    esteMes,
  }
})

// Métodos
const carregarSolicitacoes = async () => {
  try {
    loading.value = true
    const response = await fetch("/api/solicitacao")
    if (response.ok) {
      solicitacoes.value = await response.json()
    }
  } catch (error) {
    console.error("Erro ao carregar solicitações:", error)
  } finally {
    loading.value = false
  }
}

const carregarEquipamentos = async () => {
  try {
    const response = await fetch("/api/equipamento")
    if (response.ok) {
      equipamentos.value = await response.json()
    }
  } catch (error) {
    console.error("Erro ao carregar equipamentos:", error)
  }
}

const mesAnterior = () => {
  if (mesAtual.value === 0) {
    mesAtual.value = 11
    anoAtual.value--
  } else {
    mesAtual.value--
  }
}

const mesProximo = () => {
  if (mesAtual.value === 11) {
    mesAtual.value = 0
    anoAtual.value++
  } else {
    mesAtual.value++
  }
}

const selecionarDia = (dia) => {
  diaSelecionado.value = dia
}

const getTipoLabel = (tipo) => {
  const labels = {
    Calibracao: "Calibração",
    Manutencao: "Manutenção",
  }
  return labels[tipo] || tipo
}

const getEventoClass = (tipo) => {
  const classes = {
    Calibracao: "evento-calibracao",
    Manutencao: "evento-manutencao",
  }
  return classes[tipo] || ""
}

const getEquipamentoInfo = (idEquipamento) => {
  const equip = equipamentos.value.find((e) => e.Id === idEquipamento)
  return equip
    ? `${equip.Identificacao} (${equip.NroPatrimonio || "N/A"})`
    : "Equipamento não encontrado"
}

const formatDate = (date) => {
  return new Date(date).toLocaleDateString("pt-BR")
}

const irParaSolicitacoes = () => {
  router.push("/solicitacoes")
}

const novaSolicitacao = () => {
  router.push("/solicitacoes")
}

const editarSolicitacao = (solicitacao) => {
  // Navegar para a página de solicitações com o ID para edição
  router.push(`/solicitacoes?edit=${solicitacao.Id}`)
}

const excluirSolicitacao = async (id) => {
  if (confirm("Tem certeza que deseja excluir esta solicitação?")) {
    try {
      const response = await fetch(`/api/solicitacao/${id}`, {
        method: "DELETE",
      })

      if (response.ok) {
        alert("Solicitação excluída com sucesso!")
        await carregarSolicitacoes()
        diaSelecionado.value = null
      } else {
        alert("Erro ao excluir solicitação")
      }
    } catch (error) {
      console.error("Erro:", error)
      alert("Erro ao excluir solicitação")
    }
  }
}

// Carregar dados ao montar o componente
onMounted(async () => {
  await Promise.all([carregarSolicitacoes(), carregarEquipamentos()])
})
</script>

<style scoped>
.calendario {
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

.header h2 {
  margin: 0;
  color: #333;
}

.header-actions {
  display: flex;
  align-items: center;
  gap: 15px;
}

.mes-atual {
  font-size: 18px;
  font-weight: 600;
  color: #333;
  min-width: 150px;
  text-align: center;
}

.calendar-container {
  background-color: white;
  border-radius: 10px;
  padding: 20px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
  margin-bottom: 30px;
}

.calendar {
  width: 100%;
}

.calendar-header {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
  gap: 5px;
  margin-bottom: 10px;
}

.day-header {
  padding: 15px;
  text-align: center;
  font-weight: 600;
  color: #333;
  background-color: #f8f9fa;
  border-radius: 5px;
}

.calendar-body {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
  gap: 5px;
}

.day {
  aspect-ratio: 1;
  border: 1px solid #e0e0e0;
  border-radius: 5px;
  padding: 10px;
  position: relative;
  cursor: pointer;
  transition: all 0.3s;
  background-color: white;
}

.day:hover:not(.empty) {
  background-color: #e6f7ff;
  border-color: #007bff;
}

.day.empty {
  background-color: #f8f9fa;
  cursor: default;
}

.day.today {
  background-color: #fff3cd;
  border-color: #ffc107;
}

.day.has-events {
  background-color: #f0f8ff;
}

.day-number {
  font-weight: 600;
  color: #333;
  font-size: 16px;
}

.event-indicators {
  position: absolute;
  bottom: 5px;
  left: 50%;
  transform: translateX(-50%);
  display: flex;
  gap: 2px;
  align-items: center;
}

.event-dot {
  width: 8px;
  height: 8px;
  border-radius: 50%;
}

.evento-calibracao {
  background-color: #28a745;
}

.evento-manutencao {
  background-color: #ffc107;
}

.more-events {
  font-size: 10px;
  color: #666;
  font-weight: 600;
}

.event-details {
  background-color: white;
  border-radius: 10px;
  padding: 20px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
  margin-bottom: 30px;
}

.details-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
  padding-bottom: 15px;
  border-bottom: 1px solid #eee;
}

.details-header h3 {
  margin: 0;
  color: #333;
}

.btn-close {
  background: none;
  border: none;
  font-size: 18px;
  color: #666;
  cursor: pointer;
  padding: 5px;
}

.btn-close:hover {
  color: #333;
}

.events-list {
  display: grid;
  gap: 15px;
}

.event-card {
  border-radius: 8px;
  padding: 15px;
  border-left: 4px solid;
}

.evento-calibracao {
  background-color: #d4edda;
  border-left-color: #28a745;
}

.evento-manutencao {
  background-color: #fff3cd;
  border-left-color: #ffc107;
}

.event-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 10px;
}

.event-type {
  font-weight: 600;
  color: #333;
  text-transform: uppercase;
  font-size: 14px;
}

.event-id {
  color: #666;
  font-size: 12px;
}

.event-body p {
  margin: 5px 0;
  color: #555;
  font-size: 14px;
}

.event-body strong {
  color: #333;
}

.event-actions {
  display: flex;
  gap: 10px;
  margin-top: 15px;
  padding-top: 15px;
  border-top: 1px solid rgba(0, 0, 0, 0.1);
}

.no-events {
  text-align: center;
  padding: 40px;
  color: #666;
}

.statistics {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 20px;
}

.stat-card {
  background-color: white;
  padding: 20px;
  border-radius: 10px;
  text-align: center;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
}

.stat-card h4 {
  margin: 0 0 10px 0;
  color: #666;
  font-size: 14px;
  font-weight: 600;
}

.stat-number {
  font-size: 28px;
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

@media (max-width: 768px) {
  .header {
    flex-direction: column;
    gap: 15px;
    text-align: center;
  }

  .header-actions {
    flex-wrap: wrap;
    justify-content: center;
  }

  .calendar-body {
    gap: 2px;
  }

  .day {
    padding: 5px;
  }

  .day-number {
    font-size: 14px;
  }

  .event-dot {
    width: 6px;
    height: 6px;
  }

  .statistics {
    grid-template-columns: repeat(2, 1fr);
  }
}
</style>
