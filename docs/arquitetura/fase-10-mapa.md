## Fase 10 — Testabilidade: Dublês e Costuras

**Tema:** Planejar testes sem I/O usando dublês adequados e costuras (Seams)

### Objetivo
Demonstrar como testar componentes que dependem de repositórios sem executar operações reais de I/O (arquivo CSV/JSON ou banco de dados). Usar dublês (Mock, Stub, Dummy) injetados via construtor e explorar "costuras" (Seams) que permitem trocar implementações sem alterar código.

### Conceitos principais

#### Dublês de Teste
Um "dublê de teste" é uma implementação falsa de uma dependência usada para isolar o comportamento em teste.

| Tipo | Propósito | Exemplo |
|------|----------|---------|
| **Dummy** | Preenchimento, nunca é usado | Um `IRepository` vazio passado a construtor |
| **Stub** | Fornece respostas pré-definidas | `GetById()` retorna sempre um `Movie` específico |
| **Mock** | Verifica se foi chamado (assertions em chamadas) | Verificar que `Add()` foi chamado com entidade específica |
| **Spy** | Registra chamadas + permite assertions | Contar quantas vezes `Update()` foi chamado |
| **Fake** | Implementação funcional simplificada | `InMemoryRepository<T>` (tudo em memória, sem I/O) |

#### Costuras (Seams)
Um "seam" é um lugar na aplicação onde você pode alterar o comportamento sem editar a produção. Exemplos:

1. **Seam por Dependência Injetada** (DI)
   - Passar `IRepository<T>` via construtor
   - Cliente não conhece implementação concreta
   - Fácil trocar em testes

2. **Seam por Factory**
   - Uma factory decide qual implementação usar
   - Testes podem configurar a factory para retornar fake

3. **Seam por Override/Herança**
   - Subclassificar em testes para sobrescrever métodos
   - Menos desejável (acoplamento ao código de teste)

### Cenários de teste para `MovieService`

#### Cenário 1: Adicionar filme e recuperá-lo
```
Dado: serviço com repositório fake vazio
Quando: adiciono um filme com título "Inception" e ano 2010
Então: o método GetMovieById retorna o filme com os dados corretos
```

**Resultado esperado:** `Movie { Title = "Inception", Year = 2010 }`

#### Cenário 2: Listar filmes
```
Dado: repositório fake com 3 filmes
Quando: chamo ListAllMovies()
Então: retorna exatamente 3 filmes
```

**Resultado esperado:** `Count = 3`

#### Cenário 3: Atualizar filme inexistente
```
Dado: repositório fake vazio
Quando: tento atualizar filme com Id desconhecido
Então: lança InvalidOperationException
```

**Resultado esperado:** `Exceção com mensagem clara`

#### Cenário 4: Buscar filmes por critério
```
Dado: repositório fake com filmes de vários anos
Quando: busco filmes posteriores a 2010
Então: retorna apenas filmes do intervalo esperado
```

**Resultado esperado:** `Count = 2` (se DB tiver filmes de 2011, 2014, etc.)

#### Cenário 5: Deletar filme
```
Dado: repositório fake com 1 filme
Quando: deleto o filme pelo Id
Então: ListAllMovies retorna vazio
```

**Resultado esperado:** `Count = 0`

### Comportamento observável

**Foco em SAÍDA, não implementação:**
- Não testa "como" o repositório persiste (não importa se CSV/JSON/DB)
- Testa "o quê" o serviço produz (resultados, exceções, estado)
- Uso de `InMemoryRepository` como fake garante determinismo

**Exemplo de teste BATER:**
```csharp
// ❌ Ruim: testa implementação interna
Assert.That(repo._items.Count == 1, "Item adicionado ao dicionário");

// ✅ Bom: testa comportamento observável
var result = repo.GetById(movieId);
Assert.That(result != null, "Filme deve existir após adicionar");
Assert.That(result.Title == "Inception", "Título deve ser 'Inception'");
```

### Padrões de testabilidade

#### 1. Constructor Injection
```csharp
public class MovieService
{
    private readonly IRepository<Movie> _repo;
    public MovieService(IRepository<Movie> repo)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
    }
}
```
✅ Fácil injetar fake em testes

#### 2. Factory Method (Seam para composição)
```csharp
public class RepositoryFactory
{
    public static IRepository<T> Create(string type)
    {
        return type switch
        {
            "memory" => new InMemoryRepository<T>(),
            "csv" => new CsvRepository<T>(...),
            _ => throw new ArgumentException()
        };
    }
}
```
✅ Testes configuram `type = "memory"`

#### 3. Property Injection (menos preferido)
```csharp
public class MovieService
{
    public IRepository<Movie> Repository { get; set; }
}
```
⚠️ Permite sobrescrever depois de construção, mas menos seguro

### Dados de teste

Para testes reproduzíveis, use dados fixos:

```csharp
private Movie CreateTestMovie(string title = "Test Movie", int year = 2020)
{
    return new Movie
    {
        Id = Guid.NewGuid(),
        Title = title,
        Year = year
    };
}
```

### Checklist de testabilidade

- ✅ Código produção usa interfaces (`IRepository<T>`)
- ✅ Dependências injetadas via construtor
- ✅ Sem `new` direto de classe concreta dentro do serviço
- ✅ Sem acesso a IO dentro do serviço (deixar para repositório)
- ✅ Testes usam `InMemoryRepository<T>` como fake
- ✅ Assertions verificam comportamento observável (saídas)
- ✅ Cada teste isola um cenário (AAA: Arrange, Act, Assert)

### Próximos passos

- Adicionar framework de testes (xUnit, NUnit, etc.)
- Implementar testes de integração (com CSV/JSON reais)
- Usar mocks sofisticados (Moq) para cenários complexos
- Coverage de código para verificar gaps
