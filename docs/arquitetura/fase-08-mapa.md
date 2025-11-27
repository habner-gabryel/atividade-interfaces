## Fase 8 — Repository progressivo (CSV)

**Tema:** Persistência simples em CSV mantendo contrato `IRepository<T>`

### Objetivo
Evoluir a implementação de repositório em memória para uma versão que persista em arquivo CSV, sem alterar o contrato usado pelo cliente. Demonstrar leitura/escrita, tratamento de arquivo inexistente e por que o cliente permanece inalterado ao trocar a implementação.

### Operações mínimas (contrato)
- `Add(T entity)` — adiciona e persiste
- `GetById(Guid id)` — recupera pelo id
- `List()` — lista todos
- `Find(Func<T,bool> predicate)` — busca por predicado
- `Update(T entity)` — atualiza e persiste
- `Delete(Guid id)` — remove e persiste

### Leitura / Escrita (CSV)
- **Leitura:** ao inicializar, a implementação CSV tenta abrir o arquivo; se o arquivo não existir, assume-se um repositório vazio (comportamento seguro para primeiras execuções).
- **Escrita:** cada alteração que modifica o conjunto (Add/Update/Delete) grava o estado inteiro no arquivo CSV. Para evitar corrupção, a gravação é feita em arquivo temporário seguido de um rename/replace (escrita atômica no nível de arquivo disponível).
- **Serialização:** o repositório CSV delega a conversão entre `T` e `string` (linha CSV) ao construtor — assim podemos persistir qualquer entidade sem acoplar o repositório a detalhes do domínio.

### Casos de arquivo inexistente
- Inicialização sem arquivo → começa com coleção vazia; primeiro `Add` cria o arquivo.
- Falha de leitura (formato inválido) → a implementação deve lançar uma exceção clara (ou expor um resultado de erro) para que o responsável de nível superior decida política (log/backup/recovery).

### Consistência e Concorrência
- Para cenários simples, um `lock` no processo é suficiente (aplicações single-process). Em ambientes concorrentes entre processos, é necessário usar mecanismos de lock OS ou migrar para DB.

### Por que o cliente não muda ao alternar InMemory → CSV
- **Contrato estável:** o cliente depende de `IRepository<T>` (interface), não da implementação concreta. Enquanto a assinatura dos métodos e suas semânticas se mantiverem, o cliente continua funcional.
- **Inversão de dependência:** o cliente recebe (via construtor/factory/DI) uma instância de `IRepository<T>`; trocar a implementação é responsabilidade da composição da aplicação, não do cliente.
- **Serialização externa:** o `CsvRepository<T>` aceita funções de (de)serialização; o domínio não precisa conhecer a forma de persistência.

### Recomendações
- Para testes, continue usando `InMemoryRepository<T>` como dublê rápido e determinístico.
- Para produção, usar `CsvRepository<T>` apenas para cenários simples/POC; para concorrência e grandes volumes, migrar para uma solução de armazenamento dedicada.
