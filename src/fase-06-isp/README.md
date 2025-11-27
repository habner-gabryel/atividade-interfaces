# Fase 6 — ISP na prática (Segregação por Capacidade)

## Visão geral

Esta fase demonstra o Interface Segregation Principle (ISP) aplicando segregação a uma "god interface" e ajustando o consumidor para depender apenas do que precisa.

## Arquivos

- `ISPExample.cs` — exemplo C# com: `IFormatterGod` (antes), interfaces segregadas (depois), implementações e consumidores antes/depois.
- `docs/arquitetura/fase-06-mapa.md` — documento conceitual com antes/depois, sinais e ganhos.

## Execução
```powershell
cd src/fase-06-isp
# Se tiver csc no PATH:
csc ISPExample.cs
ISPExample.exe
```

## O que procurar no código
- `FormatterGeneratorBefore` chama métodos do `IFormatterGod` e exemplifica o problema.
- `ReportGeneratorAfter` recebe `ITextFormatter` e `IValidator` (dependências mínimas), demonstrando consumo reduzido.

