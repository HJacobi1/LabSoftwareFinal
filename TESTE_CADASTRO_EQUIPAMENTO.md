# Teste de Cadastro de Equipamento - Padrão Given-When-Then

## Cenário Principal

**Dado que** o admin preenche os dados, **quando** submete, **então** o equipamento é salvo.

## Estrutura dos Testes

### 1. Teste de Sucesso - Cadastro Válido

```csharp
[Fact]
public async Task CadastroEquipamento_AdminPreencheDadosValidos_EquipamentoSalvoComSucesso()
{
    // Given (Dado que o admin preenche os dados)
    var dadosEquipamento = new ModeloEquipamento
    {
        Id = 0, // ID zero indica novo equipamento
        Identificacao = "MIC-001",
        Descricao = "Microscópio Eletrônico de Varredura",
        TipoAD = AnalogicoDigital.Digital,
        Marca = "Zeiss",
        CreatedAt = DateTime.Now
    };

    // When (quando submete)
    var resultado = await _service.CreateEquipamentoAsync(dadosEquipamento);

    // Then (então o equipamento é salvo)
    Assert.NotNull(resultado);
    Assert.Equal(1, resultado.Id);
    Assert.Equal("MIC-001", resultado.Identificacao);
    // ... outras validações
}
```

### 2. Teste de Validação - Dados Inválidos

```csharp
[Fact]
public async Task CadastroEquipamento_DadosInvalidos_RetornaErroValidacao()
{
    // Given (Dado que o admin preenche dados inválidos)
    var dadosInvalidos = new ModeloEquipamento
    {
        Id = 0,
        Identificacao = "", // Identificação vazia
        Descricao = "", // Descrição vazia
        TipoAD = AnalogicoDigital.Digital,
        Marca = "", // Marca vazia
        CreatedAt = DateTime.Now
    };

    // When & Then (quando submete, então retorna erro)
    var exception = await Assert.ThrowsAsync<ArgumentException>(
        () => _service.CreateEquipamentoAsync(dadosInvalidos)
    );
}
```

### 3. Teste de Duplicação - Identificação Duplicada

```csharp
[Fact]
public async Task CadastroEquipamento_IdentificacaoDuplicada_RetornaErroDuplicacao()
{
    // Given (Dado que já existe um equipamento com a mesma identificação)
    var dadosEquipamento = new ModeloEquipamento { /* ... */ };

    // When & Then (quando submete, então retorna erro de duplicação)
    var exception = await Assert.ThrowsAsync<InvalidOperationException>(
        () => _service.CreateEquipamentoAsync(dadosEquipamento)
    );
}
```

### 4. Teste Parametrizado - Diferentes Tipos

```csharp
[Theory]
[InlineData("MIC-001", "Microscópio Eletrônico", "Zeiss", AnalogicoDigital.Digital)]
[InlineData("BAL-002", "Balança Analítica", "Mettler Toledo", AnalogicoDigital.Analógico)]
[InlineData("PHM-003", "pHmetro", "Hanna Instruments", AnalogicoDigital.Digital)]
public async Task CadastroEquipamento_DiferentesTipos_EquipamentosSalvosComSucesso(
    string identificacao, string descricao, string marca, AnalogicoDigital tipoAD)
{
    // Given (Dado que o admin preenche diferentes tipos de equipamentos)
    // When (quando submete)
    // Then (então o equipamento é salvo corretamente)
}
```

### 5. Teste de Lote - Múltiplos Equipamentos

```csharp
[Fact]
public async Task CadastroEquipamento_LoteEquipamentos_TodosSalvosEListados()
{
    // Given (Dado que o admin preenche múltiplos equipamentos)
    var equipamentos = new List<ModeloEquipamento> { /* ... */ };

    // When (quando submete todos os formulários)
    var resultados = new List<ModeloEquipamento>();
    foreach (var equipamento in equipamentos)
    {
        var resultado = await _service.CreateEquipamentoAsync(equipamento);
        resultados.Add(resultado);
    }

    // Then (então todos são salvos e listados corretamente)
    Assert.Equal(3, resultados.Count);
}
```

### 6. Teste de Validação de Campos Obrigatórios

```csharp
[Theory]
[InlineData("", "Descrição válida", "Marca válida", "Identificação é obrigatória")]
[InlineData("ID-001", "", "Marca válida", "Descrição é obrigatória")]
[InlineData("ID-001", "Descrição válida", "", "Marca é obrigatória")]
public async Task CadastroEquipamento_CamposObrigatoriosVazios_RetornaErroEspecifico(
    string identificacao, string descricao, string marca, string mensagemEsperada)
{
    // Given (Dado que o admin deixa campos obrigatórios vazios)
    // When & Then (quando submete, então retorna erro específico)
}
```

## Padrão Given-When-Then

### Given (Dado que...)

- Define o contexto inicial
- Prepara os dados de entrada
- Configura os mocks necessários
- Estabelece as premissas do teste

### When (Quando...)

- Executa a ação principal
- Chama o método que está sendo testado
- Simula a interação do usuário

### Then (Então...)

- Valida os resultados esperados
- Verifica se as asserções são verdadeiras
- Confirma que o comportamento está correto

## Benefícios desta Abordagem

1. **Legibilidade**: Os testes são fáceis de entender
2. **Manutenibilidade**: Estrutura clara e organizada
3. **Cobertura**: Testa cenários de sucesso e erro
4. **Documentação**: Serve como documentação viva do comportamento
5. **Colaboração**: Facilita a comunicação entre desenvolvedores e testadores

## Execução dos Testes

Para executar os testes:

```bash
# Executar todos os testes de equipamento
dotnet test --filter "EquipamentoCadastroIntegrationTests"

# Executar apenas o teste principal
dotnet test --filter "CadastroEquipamento_AdminPreencheDadosValidos_EquipamentoSalvoComSucesso"

# Executar testes parametrizados
dotnet test --filter "CadastroEquipamento_DiferentesTipos_EquipamentosSalvosComSucesso"
```

## Próximos Passos

1. Implementar validações reais no `EquipamentoService`
2. Adicionar testes de integração com banco de dados
3. Criar testes de performance para cadastro em lote
4. Implementar testes de UI para o formulário de cadastro
