# Fase 4 — Interface Plugável e Testável

## Visão Geral

Nesta fase introduzimos o contrato `ITextFormatter` e refatoramos o cliente para depender dessa abstração. O objetivo é permitir trocar implementações sem mudar o cliente e facilitar testes com dublês (mocks/fakes).

## Arquivos

- `TextFormatterInterface.cs` — implementação da interface, implementações concretas, `FormatterService`, `FormatterFactory` e demo `Program`.
- `docs/arquitetura/fase-04-mapa.md` — documentação conceitual explicando composição e estratégias de teste.

## Execução (exemplo)

```powershell
cd src/fase-04-oo-com-interface
# compilador C# (se instalado):
csc TextFormatterInterface.cs
TextFormatterInterface.exe
```

## Como usar em testes

- Crie um fake/mock que implemente `ITextFormatter` e injete no `FormatterService`.
- Isso permite testar o comportamento do cliente isoladamente.

## Próximos passos

- Fase 5: usar DI container e composição via configuração (runtime)
