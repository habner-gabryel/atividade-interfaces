## Fase 9 — Repository progressivo (JSON)

**Tema:** Persistência com JSON mantendo contrato `IRepository<T>`

### Objetivo
Evoluir de CSV para JSON como formato de persistência, mantendo o contrato `IRepository<T>` inalterado. Demonstrar ida/volta (serialização/desserialização), mutações seguras e o ponto único de composição que permite trocar CSV→JSON sem alterar o cliente.

### Operações mínimas (contrato)
Idênticas às fases anteriores:
- `Add(T entity)` — adiciona e persiste
- `GetById(Guid id)` — recupera pelo id
- `List()` — lista todos
- `Find(Func<T,bool> predicate)` — busca por predicado
- `Update(T entity)` — atualiza e persiste
- `Delete(Guid id)` — remove e persiste

### Ida / Volta (Serialização)
- **Ida:** objeto `T` → JSON (usando `System.Text.Json` ou similar)
- **Volta:** JSON → objeto `T` (desserialização)

#### Vantagens do JSON sobre CSV:
1. **Estrutura aninhada:** suporta objetos complexos nativamente
2. **Sem escape manual:** evita problemas com delimitadores (CSV com `|` ou `,`)
3. **Legibilidade:** formato auto-descritivo com chaves nomeadas
4. **Padrão na web:** integração fácil com APIs e terceiros

### Mutações (Operações Write)
- Cada `Add`, `Update`, `Delete` regrava o arquivo JSON inteiro (assim como CSV)
- Usa arquivo temporário + replace para evitar corrupção (escrita atômica)
- Lock no nível do objeto para concorrência within-process

**Nota:** em ambientes concorrentes entre processos, considere migrar para DB ou usar filas de eventos.

### Ponto Único de Composição
O cliente depende de `IRepository<T>`, não da implementação.

**Antes (sem ponto de composição):**
```csharp
// Cliente acoplado à escolha de formato
IRepository<Movie> repo = new CsvRepository<Movie>(...);
var service = new MovieService(repo);
```

**Depois (com ponto de composição centralizado):**
```csharp
// Factory encapsula a decisão
public class RepositoryFactory
{
    public static IRepository<T> CreateRepository<T>(string format) where T : IEntity
    {
        return format switch
        {
            "csv" => new CsvRepository<T>(...),
            "json" => new JsonRepository<T>(...),
            _ => throw new ArgumentException($"Formato desconhecido: {format}")
        };
    }
}

// Cliente usa a factory, não conhece detalhes
var repo = RepositoryFactory.CreateRepository<Movie>("json");
var service = new MovieService(repo);
```

**Troca CSV → JSON:** basta alterar o parâmetro na factory (ou variável de configuração).

### Por que não alterar o cliente?
1. **Contrato estável:** `IRepository<T>` não muda
2. **Inversão de dependência:** cliente recebe instância, não cria
3. **Política centralizada:** decisão de formato está em um único lugar (Factory ou DI container)

### Recomendações
- Use `JsonRepository<T>` para protótipos e cenários com volumes moderados
- Para testes, continue com `InMemoryRepository<T>` (rápido, determinístico)
- Para produção com concorrência, migre para um banco de dados dedicado
