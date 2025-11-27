# Fase 8 — Repository CSV

Exemplo de repositório que persiste em CSV mantendo o contrato `IRepository<T>` usado pelo serviço.

Arquivos principais:
- `CsvRepository.cs` — implementação que grava/ler CSV usando funções de (de)serialização passadas no construtor.

Como usar (exemplo):

1. Defina seu tipo que implementa `IEntity` (ex.: `Movie`).
2. Crie funções `serializer` e `deserializer` que convertam `Movie` ↔ string (linha CSV).
3. Instancie:

```powershell
# Exemplo rápido em C# (compilar com csc ou usar dotnet run em projeto)
# var repo = new CsvRepository<Movie>("movies.csv", m => $"{m.Id}|{m.Title}|{m.Year}", line => ParseMovie(line));
```

Troca InMemory → CSV:
- No ponto de composição (Factory/DI), basta fornecer `new CsvRepository<Movie>(...)` em vez de `new InMemoryRepository<Movie>()`.
- O `MovieService` que depende de `IRepository<Movie>` não precisa ser alterado.

Observações:
- Para testes unitários, continue usando `InMemoryRepository` como dublê.
- CSV é apropriado para protótipos e volumes pequenos; para cenários reais prefira um banco de dados.
