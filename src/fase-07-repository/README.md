# Fase 7 — Repository progressivo (InMemory)

## Visão Geral

Implementação de um repositório em memória genérico para o domínio (ex.: `Movie`). Demonstra o contrato (`IRepository<T>`), operações mínimas e como um serviço (`MovieService`) usa o repositório sem conhecer sua implementação.

## Arquivos

- `InMemoryRepository.cs` — contém `IEntity`, `IRepository<T>`, `InMemoryRepository<T>`, `Movie`, `MovieService` e `RepositoryDemo.Main`.
- `docs/arquitetura/fase-07-mapa.md` — documento com objetivo e contrato.

## Execução
```powershell
cd src/fase-07-repository
# Se tiver csc no PATH:
csc InMemoryRepository.cs
InMemoryRepository.exe
```

## Observações
- Use `InMemoryRepository<T>` para testes unitários ou desenvolvimento rápido.
- Para produção, implemente `IRepository<T>` usando banco de dados/ORM.
- Prefira injetar `IRepository<T>` via DI para permitir substituição sem alterar serviços.
