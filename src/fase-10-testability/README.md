# Fase 10 — Testabilidade: Dublês e Costuras

Exemplos de testes sem I/O usando dublês (Fakes) e costuras (Seams) para injetar dependências.

## Arquivos principais

- `TestabilityExamples.cs` — implementação de `MovieService` + exemplos de testes usando `InMemoryRepository<T>` como fake
- `fase-10-mapa.md` — documentação completa de conceitos, cenários e padrões

## Conceitos principais

### Dublês de Teste

| Tipo | Uso |
|------|-----|
| **Dummy** | Preenchimento (nunca usado) |
| **Stub** | Respostas pré-configuradas |
| **Mock** | Verifica chamadas (assertions) |
| **Spy** | Registra + assertions |
| **Fake** | Implementação simplificada funcional |

### Costuras (Seams)

Um "seam" é um ponto onde você muda comportamento sem editar produção:

1. **DI via Construtor** (recomendado)
   ```csharp
   public MovieService(IRepository<Movie> repo) { _repo = repo; }
   ```
   Permite passar `InMemoryRepository<T>` em testes

2. **Factory** (seam para composição)
   ```csharp
   var repo = RepositoryFactory.Create("memory"); // fake em testes
   ```

3. **Override/Herança** (menos desejável)
   - Subclassificar para sobrescrever métodos
   - Mais acoplamento

## Cenários de teste implementados

### Cenário 1: Adicionar e recuperar filme
```csharp
_service.AddMovie("Inception", 2010);
var movies = _service.ListAllMovies().ToList();
// ✅ Assert: Count == 1, Title == "Inception", Year == 2010
```

### Cenário 2: Listar múltiplos filmes
```csharp
_service.AddMovie("Inception", 2010);
_service.AddMovie("Interstellar", 2014);
// ✅ Assert: Count == 3
```

### Cenário 3: Atualizar filme
```csharp
_service.UpdateMovieTitle(movieId, "New Title");
// ✅ Assert: GetMovieById(id).Title == "New Title"
```

### Cenário 4: Erro ao atualizar inexistente
```csharp
_service.UpdateMovieTitle(unknownId, "Title");
// ✅ Assert: Lança InvalidOperationException com mensagem clara
```

### Cenário 5: Filtrar por critério
```csharp
var filtered = _service.FindMoviesByYear(2010).ToList();
// ✅ Assert: Retorna apenas filmes de 2010
```

### Cenário 6: Deletar filme
```csharp
_service.DeleteMovie(movieId);
var remaining = _service.ListAllMovies().ToList();
// ✅ Assert: Count == 0
```

### Cenário 7: Repositório vazio
```csharp
var result = _service.GetMovieById(Guid.NewGuid());
// ✅ Assert: result == null
```

## Comportamento observável vs. Implementação

✅ **BOM:** Testa saídas
```csharp
var movie = _service.GetMovieById(id);
Assert.That(movie.Title == "Inception", "Título correto");
```

❌ **RUIM:** Testa detalhes internos
```csharp
Assert.That(repo._items.Count == 1, "Item no dicionário");
```

## Padrão AAA

Cada teste segue: **Arrange → Act → Assert**

```csharp
// Arrange: preparar
var repo = new InMemoryRepository<Movie>();
var service = new MovieService(repo);

// Act: executar
service.AddMovie("Inception", 2010);

// Assert: verificar comportamento observável
Assert.That(service.ListAllMovies().Count() == 1);
```

## Por que InMemoryRepository é um bom fake?

1. ✅ Sem I/O (rápido, determinístico)
2. ✅ Implementação funcional (não é mock vazio)
3. ✅ Simples de entender
4. ✅ Reutilizável entre muitos testes
5. ✅ Isolamento completo do sistema externo

## Compile e rode

```powershell
# Compilar com csc
csc TestabilityExamples.cs -out:TestabilityExamples.exe

# Executar
.\TestabilityExamples.exe

# Resultado esperado:
# ✅ SUCESSO: Filme adicionado e recuperado corretamente
# ✅ SUCESSO: Listagem retornou exatamente 3 filmes
# ... (7 testes no total)
```

## Próximas evoluções

- **Framework de testes:** xUnit, NUnit para automatizar assertions
- **Mocks sofisticados:** Moq para cenários com múltiplas dependências
- **Integração:** Testes com CSV/JSON reais
- **Coverage:** Verificar gaps de código não testado
- **Dados parametrizados:** Mesmos testes com inputs variados

## Checklist de testabilidade

- ✅ Serviço usa interfaces (`IRepository<T>`)
- ✅ Dependências injetadas via construtor
- ✅ Sem `new` de classes concretas dentro do serviço
- ✅ Sem I/O no serviço (delegado ao repositório)
- ✅ Testes usam `InMemoryRepository<T>` como fake
- ✅ Assertions verificam comportamento (saídas)
- ✅ Cada teste isola um cenário (AAA)
