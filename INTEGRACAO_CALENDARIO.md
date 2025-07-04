# Integração Calendário e Solicitações

## Visão Geral

Esta integração conecta o sistema de calendário com o menu de solicitações, permitindo uma visualização unificada e interativa dos eventos de manutenção e calibração.

## Funcionalidades Implementadas

### 1. Componente Calendário (`Calendario.vue`)

- **Visualização Mensal**: Calendário interativo com navegação entre meses
- **Indicadores de Eventos**: Pontos coloridos indicam solicitações nos dias
- **Detalhes dos Eventos**: Clique em um dia para ver detalhes das solicitações
- **Estatísticas**: Resumo de solicitações por tipo e período
- **Navegação Integrada**: Botões para ir para a página de solicitações

### 2. Componente Solicitações (`Solicitacao.vue`)

- **Formulário de Cadastro**: Interface para criar novas solicitações
- **Lista de Solicitações**: Visualização em cards com ações
- **Navegação para Calendário**: Botão para visualizar no calendário
- **Integração com API**: CRUD completo via endpoints REST

### 3. Menu Principal (`MainMenu.vue`)

- **Novas Opções**: Links para Calendário e Solicitações
- **Ícones Intuitivos**: Font Awesome para melhor UX
- **Acesso Universal**: Todas as solicitações disponíveis para todos os usuários

## Estrutura da API

### Endpoints de Solicitação

- `GET /api/solicitacao` - Listar todas as solicitações
- `GET /api/solicitacao/{id}` - Obter solicitação específica
- `POST /api/solicitacao` - Criar nova solicitação
- `PUT /api/solicitacao/{id}` - Atualizar solicitação
- `DELETE /api/solicitacao/{id}` - Excluir solicitação

### Modelo de Dados

```csharp
public class Solicitacao
{
    public int Id { get; set; }
    public DateTime Data { get; set; }
    public ManutencaoCalibracao TipoMC { get; set; }
    public string IdEquipamento { get; set; }
    public string Descricao { get; set; }
}
```

## Fluxo de Integração

### 1. Criação de Solicitação

1. Usuário acessa `/solicitacoes`
2. Preenche formulário com data, tipo e equipamento
3. Solicitação é salva via API
4. Calendário é atualizado automaticamente

### 2. Visualização no Calendário

1. Usuário acessa `/calendario`
2. Solicitações são carregadas da API
3. Dias com eventos são marcados com indicadores
4. Clique em um dia mostra detalhes das solicitações

### 3. Navegação Entre Componentes

- **Solicitações → Calendário**: Botão "Ver Calendário"
- **Calendário → Solicitações**: Botão "Ver Solicitações"
- **Edição**: Clique em "Editar" no calendário redireciona para solicitações

## Características Técnicas

### Frontend (Vue.js)

- **Composition API**: Uso de `ref`, `computed`, `onMounted`
- **Router Integration**: Navegação programática entre rotas
- **Responsive Design**: Layout adaptável para mobile
- **Real-time Updates**: Atualização automática após operações CRUD

### Backend (.NET)

- **Entity Framework**: Mapeamento ORM para PostgreSQL
- **Repository Pattern**: Abstração de acesso a dados
- **Service Layer**: Lógica de negócio centralizada
- **RESTful API**: Endpoints padronizados

## Estilos e UX

### Cores dos Eventos

- **Calibração**: Verde (#28a745)
- **Manutenção**: Amarelo (#ffc107)

### Indicadores Visuais

- **Dias com eventos**: Fundo azul claro
- **Dia atual**: Borda amarela
- **Hover effects**: Interações suaves
- **Tooltips**: Informações detalhadas

## Rotas Configuradas

```javascript
{
  path: "solicitacoes",
  name: "solicitacoes",
  component: Solicitacao
},
{
  path: "calendario",
  name: "calendario",
  component: Calendario
}
```

## Próximos Passos

1. **Filtros Avançados**: Por laboratório, equipamento, período
2. **Notificações**: Alertas para solicitações próximas
3. **Relatórios**: Exportação de dados do calendário
4. **Drag & Drop**: Reagendamento via interface visual
5. **Sincronização**: Integração com calendários externos

## Como Usar

1. **Acesse o sistema** e faça login
2. **Vá para Solicitações** para criar novos eventos
3. **Navegue para Calendário** para visualização mensal
4. **Clique nos dias** para ver detalhes dos eventos
5. **Use os botões de navegação** para alternar entre as visualizações

## Troubleshooting

### Problemas Comuns

1. **Eventos não aparecem no calendário**

   - Verifique se a API está rodando
   - Confirme se as solicitações têm datas válidas

2. **Erro ao salvar solicitação**

   - Verifique se todos os campos obrigatórios estão preenchidos
   - Confirme se o equipamento existe no sistema

3. **Navegação não funciona**
   - Verifique se o Vue Router está configurado
   - Confirme se as rotas estão registradas corretamente
