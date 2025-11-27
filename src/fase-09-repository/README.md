# Fase 9 — Repository JSON

Exemplo de repositório que persiste em JSON mantendo o contrato `IRepository<T>` usado pelo serviço.

Arquivos principais:
- `JsonRepository.cs` — implementação que lê/grava JSON usando `System.Text.Json`.

## Por que JSON em vez de CSV?

| Aspecto | CSV | JSON |
|--------|-----|------|
| **Estrutura** | Plana (linhas) | Aninhada (objetos) |
| **Escape** | Requer cuidado com delimitadores | Nativo |
| **Legibilidade** | Apenas valores | Chaves + valores |
| **Tipos complexos** | Difícil | Nativo |

## Como usar (exemplo):

```csharp
// Se sua classe implementa IEntity:
public class Movie : IEntity
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public int Year { get; set; }
}

// Crie o repositório JSON:
var repo = new JsonRepository<Movie>("movies.json");

// Use como qualquer outro repositório:
repo.Add(new Movie { Id = Guid.NewGuid(), Title = "Inception", Year = 2010 });
var all = repo.List();
```

## Arquivo JSON gerado

Exemplo de conteúdo em `movies.json`:

```json
[
  {
    "Id": "550e8400-e29b-41d4-a716-446655440000",
    "Title": "Inception",
    "Year": 2010
  },
  {
    "Id": "550e8400-e29b-41d4-a716-446655440001",
    "Title": "Interstellar",
    "Year": 2014
  }
]
```

## Ponto Único de Composição (Factory)

Para trocar de CSV → JSON sem alterar o cliente, use uma Factory:

```csharp
public class RepositoryFactory
{
    public static IRepository<T> CreateRepository<T>(string format, string filePath = null) 
        where T : IEntity
    {
        return format.ToLowerInvariant() switch
        {
            "json" => new JsonRepository<T>(filePath ?? $"{typeof(T).Name}.json"),
            "csv" => new CsvRepository<T>(filePath ?? $"{typeof(T).Name}.csv", /* ... */),
            "memory" => new InMemoryRepository<T>(),
            _ => throw new ArgumentException($"Formato desconhecido: {format}")
        };
    }
}
```

Agora o cliente fica agnóstico:

```csharp
// Troca apenas a Factory, cliente não muda
var repo = RepositoryFactory.CreateRepository<Movie>("json", "movies.json");
var service = new MovieService(repo);
service.PrintAllMovies(); // Funciona igual
```

## Observações

- Para testes unitários, continue usando `InMemoryRepository<T>` como dublê.
- JSON é apropriado para protótipos e volumes pequenos; para cenários reais prefira um banco de dados.
- `System.Text.Json` está disponível em .NET 5+; para versões anteriores, use `Newtonsoft.Json` (NuGet).

## Compile e rode

```powershell
# Se tiver um arquivo .cs que use JsonRepository:
csc JsonRepository.cs -out:JsonRepository.exe

# Ou usando dotnet (se disponível):
dotnet build
dotnet run
```
