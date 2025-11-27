# Fase 7 — Repository progressivo (InMemory)

## Objetivo

Modelar um repositório em memória para o domínio (ex.: `Movie`) que exponha um contrato simples e permita que serviços consumam o repositório sem conhecer sua implementação interna.

---

## Contrato mínimo (IRepository<T>)

Operações essenciais que todo repositório deve fornecer:

- `T Add(T entity)` — adiciona e retorna a entidade persistida (com Id preenchido)
- `T GetById(Guid id)` — obtém por id (ou `null`)
- `IEnumerable<T> List()` — lista todos
- `IEnumerable<T> Find(Func<T,bool> predicate)` — consulta com predicado
- `bool Update(T entity)` — atualiza (por id)
- `bool Delete(Guid id)` — remove

Assuma que `T` tem uma propriedade `Id` do tipo `Guid`.

---

## Implementação em memória (InMemoryRepository)

- Usa um `ConcurrentDictionary<Guid, T>` (thread-safe) para armazenar entidades.
- Garante que `Add` preencha `Id` quando necessário.
- `Find` usa `IEnumerable<T>`/LINQ sobre os valores internos.
- Não tem persistência durável — adequado para testes, desenvolvimento e demos.

---

## Exemplo de domínio: `Movie`

```csharp
public class Movie : IEntity
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public int Year { get; set; }
}
```

---

## Serviço que usa o repositório (MovieService)

- `MovieService` depende de `IRepository<Movie>` (injeção via construtor).
- Não conhece se o repositório é em memória, arquivo, DB ou remoto.
- Usa apenas o contrato: `Add`, `GetById`, `List`, `Update`, `Delete`, `Find`.

Benefícios:
- Desacoplamento: implementação trocável via factory/DI
- Testabilidade: injetar `InMemoryRepository` ou um dublê em testes
- Reutilização: mesmo serviço funciona com qualquer implementação

---

## Operações mínimas e comportamento esperado

- Adição: preenche `Id` e retorna entidade completa
- Consulta: `GetById` retorna `null` se não existir
- Atualização: retorna `true` se entidade atualizada, `false` caso contrário
- Remoção: retorna `true` se removida, `false` caso contrário

---

## Considerações

- InMemory é ótimo para testes unitários; para cargas reais usar repositório persistente.
- Repositórios devem expor operações de consistência (transações) apenas quando a implementação suportar.
- Prefira contratos pequenos (ISP): `IReadRepository<T>` e `IWriteRepository<T>` quando necessário.

---

## Onde encontrar
- Código: `src/fase-07-repository/InMemoryRepository.cs`
- Exemplo de uso: `src/fase-07-repository/RepositoryDemo.cs`
- README da fase: `src/fase-07-repository/README.md`
