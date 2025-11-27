# Fase 5 — Essenciais de Interfaces em C#

## Visão Geral

Propostas de duas interfaces do domínio (formatação de texto) e exemplos práticos:
- `ITextFormatter` — contrato de formatação
- `IIdentifiable` — contrato de identificação/metadata

Inclui exemplos de:
- Classe que implementa as duas interfaces (`FancyFormatter`)
- Implementação explícita de interfaces para desambiguar nomes (`DualDescriber`)
- Uso de generics com `where` constraints (`FormatterHost<T>`)
- Notas sobre `default members` em interfaces

## Arquivos
- `InterfacesEssentials.cs` — exemplos em C# (classe que implementa duas interfaces, explicit impl e generic host)
- `docs/arquitetura/fase-05-mapa.md` — documentação conceitual

## Execução
```powershell
cd src/fase-05-essenciais-interfaces
# Se tiver csc no PATH:
csc InterfacesEssentials.cs
InterfacesEssentials.exe
```
