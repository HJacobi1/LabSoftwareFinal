<template>
  <div class="calendario">
    <div class="header">
      <h2>Calendário de Eventos</h2>
      <div class="calendar-controls">
        <button @click="mesAnterior" class="btn-nav">
          <i class="fas fa-chevron-left"></i>
        </button>
        <span class="mes-atual">{{ mesAtualNome }} {{ anoAtual }}</span>
        <button @click="mesProximo" class="btn-nav">
          <i class="fas fa-chevron-right"></i>
        </button>
      </div>
    </div>

    <div class="calendar">
      <!-- Cabeçalho dos dias da semana -->
      <div class="calendar-header">
        <div v-for="dia in diasSemana" :key="dia" class="day-header">
          {{ dia }}
        </div>
      </div>

      <!-- Dias do calendário -->
      <div class="calendar-grid">
        <div
          v-for="(dia, index) in diasCalendario"
          :key="index"
          :class="[
            'day',
            {
              'empty': !dia.data,
              'today': dia.hoje,
              'has-events': dia.eventos && dia.eventos.length > 0
            }
          ]"
          @click="dia.data ? selecionarDia(dia) : null"
        >
          <span v-if="dia.data" class="day-number">{{ dia.numero }}</span>
          <div v-if="dia.eventos && dia.eventos.length > 0" class="event-indicators">
            <div
              v-for="(evento, idx) in dia.eventos.slice(0, 3)"
              :key="idx"
              class="event-dot"
              :class="getEventClass(evento.tipo)"
              :title="`${evento.tipo} - ${evento.equipamento}`"
            ></div>
            <span v-if="dia.eventos.length > 3" class="more-events">+{{ dia.eventos.length - 3 }}</span>
          </div>
        </div>
      </div>
    </div>

    <!-- Modal de eventos do dia -->
    <div v-if="showEventModal" class="modal-overlay" @click="fecharModal">
      <div class="modal-content" @click.stop>
        <div class="modal-header">
          <h3>Eventos do dia {{ formatarData(selectedDate) }}</h3>
          <button @click="fecharModal" class="btn-close">
            <i class="fas fa-times"></i>
          </button>
        </div>
        <div class="modal-body">
          <div v-if="eventosDiaSelecionado.length === 0" class="no-events">
            <p>Nenhum evento agendado para este dia.</p>
          </div>
          <div v-else class="events-list">
            <div
              v-for="evento in eventosDiaSelecionado"
              :key="evento.id"
              class="event-card"
              :class="getEventClass(evento.tipo)"
            >
              <div class="event-header">
                <span class="event-type">{{ evento.tipo }}</span>
                <span class="event-time">{{ evento.horario }}</span>
              </div>
              <div class="event-details">
                <p><strong>Equipamento:</strong> {{ evento.equipamento }}</p>
                <p><strong>Responsável:</strong> {{ evento.responsavel }}</p>
                <p v-if="evento.descricao"><strong>Descrição:</strong> {{ evento.descricao }}</p>
              </div>
              <div class="event-actions">
                <button @click="editarEvento(evento)" class="btn btn-small btn-secondary">
                  <i class="fas fa-edit"></i> Editar
                </button>
                <button @click="excluirEvento(evento.id)" class="btn btn-small btn-danger">
                  <i class="fas fa-trash"></i> Excluir
                </button>
              </div>
            </div>
          </div>
        </div>
        <div class="modal-footer">
          <button @click="novoEvento" class="btn btn-primary">
            <i class="fas fa-plus"></i> Novo Evento
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'

// Estado do componente
const mesAtual = ref(new Date().getMonth())
const anoAtual = ref(new Date().getFullYear())
const showEventModal = ref(false)
const selectedDate = ref(null)
const eventosDiaSelecionado = ref([])

// Dados dos eventos (simulados - substituir por API)
const eventos = ref({
  "2025-07-05": [
    {
      id: 1,
      tipo: "Manutenção",
      equipamento: "Microscópio A",
      responsavel: "Carlos Silva",
      horario: "09:00",
      descricao: "Manutenção preventiva mensal"
    }
  ],
  "2025-07-12": [
    {
      id: 2,
      tipo: "Calibração",
      equipamento: "Balança B",
      responsavel: "Ana Costa",
      horario: "14:00",
      descricao: "Calibração trimestral"
    },
    {
      id: 3,
      tipo: "Manutenção",
      equipamento: "Centrífuga C",
      responsavel: "João Santos",
      horario: "16:00",
      descricao: "Verificação de funcionamento"
    }
  ],
  "2025-07-25": [
    {
      id: 4,
      tipo: "Calibração",
      equipamento: "PHmetro",
      responsavel: "Maria Oliveira",
      horario: "10:00",
      descricao: "Calibração semestral"
    }
  ]
})

// Constantes
const diasSemana = ["Dom", "Seg", "Ter", "Qua", "Qui", "Sex", "Sáb"]
const meses = [
  "Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho",
  "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro"
]

// Computed properties
const mesAtualNome = computed(() => meses[mesAtual.value])

const diasCalendario = computed(() => {
  const dias = []
  const primeiroDia = new Date(anoAtual.value, mesAtual.value, 1)
  const ultimoDia = new Date(anoAtual.value, mesAtual.value + 1, 0)
  const inicioSemana = primeiroDia.getDay()
  const totalDias = ultimoDia.getDate()
  const hoje = new Date()

  // Dias vazios no início
  for (let i = 0; i < inicioSemana; i++) {
    dias.push({ data: null, numero: '', eventos: [] })
  }

  // Dias do mês
  for (let dia = 1; dia <= totalDias; dia++) {
    const data = new Date(anoAtual.value, mesAtual.value, dia)
    const dataStr = formatarDataParaChave(data)
    const eventosDia = eventos.value[dataStr] || []
    const ehHoje = data.toDateString() === hoje.toDateString()

    dias.push({
      data: data,
      numero: dia,
      eventos: eventosDia,
      hoje: ehHoje
    })
  }

  return dias
})

// Métodos
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
  selectedDate.value = dia.data
  const dataStr = formatarDataParaChave(dia.data)
  eventosDiaSelecionado.value = eventos.value[dataStr] || []
  showEventModal.value = true
}

const fecharModal = () => {
  showEventModal.value = false
  selectedDate.value = null
  eventosDiaSelecionado.value = []
}

const formatarData = (data) => {
  return data.toLocaleDateString('pt-BR')
}

const formatarDataParaChave = (data) => {
  return `${data.getFullYear()}-${String(data.getMonth() + 1).padStart(2, '0')}-${String(data.getDate()).padStart(2, '0')}`
}

const getEventClass = (tipo) => {
  const classes = {
    'Manutenção': 'event-manutencao',
    'Calibração': 'event-calibracao',
    'Verificação': 'event-verificacao'
  }
  return classes[tipo] || 'event-default'
}

const novoEvento = () => {
  // Implementar criação de novo evento
  alert('Funcionalidade de novo evento será implementada')
}

const editarEvento = (evento) => {
  // Implementar edição de evento
  alert(`Editar evento: ${evento.tipo} - ${evento.equipamento}`)
}

const excluirEvento = (id) => {
  if (confirm('Tem certeza que deseja excluir este evento?')) {
    // Implementar exclusão de evento
    alert(`Evento ${id} excluído`)
    fecharModal()
  }
}

// Carregar eventos ao montar o componente
onMounted(() => {
  // Aqui você pode carregar eventos da API
  console.log('Calendário montado')
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

.calendar-controls {
  display: flex;
  align-items: center;
  gap: 20px;
}

.btn-nav {
  background: none;
  border: none;
  font-size: 18px;
  cursor: pointer;
  padding: 8px;
  border-radius: 5px;
  transition: background-color 0.3s;
}

.btn-nav:hover {
  background-color: #f0f0f0;
}

.mes-atual {
  font-size: 18px;
  font-weight: 600;
  color: #333;
  min-width: 150px;
  text-align: center;
}

.calendar {
  background: white;
  border-radius: 10px;
  padding: 20px;
  box-shadow: 0 2px 10px rgba(0,0,0,0.1);
}

.calendar-header {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
  gap: 1px;
  margin-bottom: 10px;
}

.day-header {
  padding: 15px 10px;
  text-align: center;
  font-weight: 600;
  color: #666;
  background-color: #f8f9fa;
  border-radius: 5px;
}

.calendar-grid {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
  gap: 1px;
}

.day {
  min-height: 100px;
  padding: 10px;
  border: 1px solid #eee;
  cursor: pointer;
  transition: all 0.3s;
  position: relative;
  display: flex;
  flex-direction: column;
}

.day:hover {
  background-color: #f8f9fa;
  transform: translateY(-1px);
  box-shadow: 0 2px 8px rgba(0,0,0,0.1);
}

.day.empty {
  background-color: #f8f9fa;
  cursor: default;
}

.day.today {
  background-color: #e3f2fd;
  border-color: #2196f3;
}

.day.has-events {
  background-color: #fff3e0;
}

.day-number {
  font-weight: 600;
  color: #333;
  margin-bottom: 5px;
}

.event-indicators {
  display: flex;
  flex-wrap: wrap;
  gap: 2px;
  margin-top: auto;
}

.event-dot {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  flex-shrink: 0;
}

.event-manutencao {
  background-color: #ff9800;
}

.event-calibracao {
  background-color: #4caf50;
}

.event-verificacao {
  background-color: #2196f3;
}

.event-default {
  background-color: #9e9e9e;
}

.more-events {
  font-size: 10px;
  color: #666;
  margin-left: 2px;
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
  max-width: 600px;
  max-height: 80vh;
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

.no-events {
  text-align: center;
  padding: 40px;
  color: #666;
}

.events-list {
  display: flex;
  flex-direction: column;
  gap: 15px;
}

.event-card {
  padding: 15px;
  border-radius: 8px;
  border-left: 4px solid;
}

.event-card.event-manutencao {
  background-color: #fff3e0;
  border-left-color: #ff9800;
}

.event-card.event-calibracao {
  background-color: #e8f5e8;
  border-left-color: #4caf50;
}

.event-card.event-verificacao {
  background-color: #e3f2fd;
  border-left-color: #2196f3;
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
}

.event-time {
  color: #666;
  font-size: 14px;
}

.event-details p {
  margin: 5px 0;
  color: #555;
  font-size: 14px;
}

.event-details strong {
  color: #333;
}

.event-actions {
  display: flex;
  gap: 10px;
  margin-top: 15px;
}

.modal-footer {
  padding: 20px;
  border-top: 1px solid #eee;
  text-align: right;
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

.btn-primary:hover {
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
  
  .calendar-controls {
    gap: 10px;
  }
  
  .mes-atual {
    min-width: 120px;
  }
  
  .day {
    min-height: 80px;
    padding: 8px;
  }
  
  .modal-content {
    width: 95%;
    margin: 10px;
  }
}
</style>