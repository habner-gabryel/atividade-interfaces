# Fase 11 — Cheiros e Antídotos

Identificação de code smells em fases anteriores e refatorações que as eliminam.

## Arquivos principais

- `CodeSmellsAndAntidotes.cs` — código com cheiros e suas refatorações (antes/depois)
- `fase-11-mapa.md` — análise detalhada de 2 cheiros, motivação e antídotos

## Cheiros Identificados

### Cheiro 1: God Interface (Fase 4 → Fase 6)

**Problema:**
```csharp
// ❌ Uma interface gigante
public interface IBadFormatter
{
    string Format(string text);      // Formatação
    void ValidateInput(string input); // Validação
    void LogOperation(string message); // Logging
    string Describe();               // Descrição
}
```

**Sinais:**
- Interface com 4+ responsabilidades não relacionadas
- Cliente que quer só formatação é forçado a implementar logging
- Mudanças em logging quebram formatadores

**Antídoto: ISP (Interface Segregation Principle)**
```csharp
// ✅ Interfaces pequenas e focadas
public interface IGoodFormatter { string Format(string text); }
public interface IGoodValidator { bool IsValid(string input); }
public interface IGoodDescribable { string Describe(); }

// Cliente escolhe o que implementar
public class SimpleFormatter : IGoodFormatter { ... }
public class ValidatingFormatter : IGoodFormatter, IGoodValidator { ... }
```

**Ganhos:**
- ✅ Cliente implementa APENAS o necessário
- ✅ Mudanças isoladas por interface
- ✅ Testes mockar só o relevante
- ✅ Extensível sem quebrar existentes

---

### Cheiro 2: Composição Espalhada (Fase 3 → Fase 4)

**Problema:**
```csharp
// ❌ Switch duplicado em múltiplos clientes
public class BadClient1
{
    public string Format(string text, string mode)
    {
        var formatter = mode switch
        {
            "UPPER" => new UpperFormatter(),
            "LOWER" => new LowerFormatter(),
            // ... 5 linhas de decisão
        };
        return formatter.Format(text);
    }
}

public class BadClient2
{
    public string Transform(string text, string mode)
    {
        // Switch NOVAMENTE repetido...
        var formatter = mode switch { ... };
    }
}
```

**Sinais:**
- Switch similar em 2+ lugares
- Adicionar novo modo = editar vários clientes
- Alto risco de inconsistência
- Decisão não está centralizada

**Antídoto: Factory Pattern (Decisão Centralizada)**
```csharp
// ✅ Decisão em um único lugar
public class FormatterFactory
{
    public static IGoodFormatter CreateFormatter(string mode)
    {
        return mode switch
        {
            "UPPER" => new UpperFormatter(),
            "LOWER" => new LowerFormatter(),
            "REVERSE" => new ReverseFormatter(), // ← Novo modo aqui
            _ => new PassthroughFormatter()
        };
    }
}

// Clientes simples e consistentes
public class GoodClient1
{
    public string Format(string text, string mode)
    {
        var formatter = FormatterFactory.CreateFormatter(mode);
        return formatter.Format(text);
    }
}

public class GoodClient2
{
    public string Transform(string text, string mode)
    {
        var formatter = FormatterFactory.CreateFormatter(mode);
        return formatter.Format(text);
    }
}
```

**Ganhos:**
- ✅ Decisão centralizada em um único lugar
- ✅ Adicionar modo novo: edita Factory, clientes intactos
- ✅ Consistência garantida
- ✅ Fácil de testar isoladamente

---

## Padrões de Refatoração Usados

| Padrão | Problema | Solução |
|--------|----------|---------|
| **ISP** | God Interface | Dividir em interfaces pequenas e focadas |
| **Factory** | Decisão espalhada | Centralizar em um único ponto |
| **Composição** | Herança profunda | Compor interfaces simples |
| **Extract Method** | Duplicação | Método reutilizável |
| **Dependency Injection** | Acoplamento | Interface ao invés de concreto |

---

## Técnicas para Detectar Cheiros

| Cheiro | Sinal | Exemplo |
|--------|-------|---------|
| **God Class/Interface** | 4+ responsabilidades | `IFormatterValidatorLogger` |
| **Switch espalhado** | Mesmo switch em 2+ lugares | Decisão de composição em clientes |
| **Duplicação** | Mesmo código em 2+ locais | Mesmo switch em Client1 e Client2 |
| **Classe grande** | >500 linhas | Classe monolítica |
| **Acoplamento** | Cliente conhece concreto | `new CsvRepository()` direto |
| **Rigidez** | Adicionar feature quebra tudo | Herança frágil |

---

## Antes vs. Depois

### God Interface

| Aspecto | Antes (Cheiro) | Depois (ISP) |
|--------|---------|----------|
| **Métodos** | 4+ (tudo junto) | 1-2 (focado) |
| **Cliente usa** | 1 de 4 | Todos os implementados |
| **Mudança quebra** | Todos implementadores | Só quem usa |
| **Teste mocka** | 4 métodos | 1-2 métodos |
| **Acoplamento** | Alto | Baixo |

### Composição Espalhada

| Aspecto | Antes (Cheiro) | Depois (Factory) |
|--------|---------|----------|
| **Switch em** | Múltiplos clientes | Um único lugar |
| **Adicionar modo** | Edita N clientes | Edita 1 Factory |
| **Inconsistência** | Alto risco | Eliminado |
| **Testabilidade** | Difícil | Factory isolada |
| **Manutenção** | Alto custo | Baixo custo |

---

## Compile e rode

```powershell
# Compilar
csc CodeSmellsAndAntidotes.cs -out:CodeSmellsAndAntidotes.exe

# Executar
.\CodeSmellsAndAntidotes.exe

# Saída esperada:
# === Fase 11 — Cheiros e Antídotos ===
# 
# ## CHEIRO 1: God Interface
# ❌ ANTES: ...
# ✅ DEPOIS: ...
# ...
```

---

## Checklist para Refatoração

Ao identificar um cheiro:
- ✅ Escrever testes (baseline)
- ✅ Entender o problema
- ✅ Escolher antídoto apropriado
- ✅ Refatorar pequenas mudanças (1 de cada vez)
- ✅ Testes passam após cada mudança
- ✅ Validar comportamento observável intacto
- ✅ Code review com colega

---

## Próximas Evoluções

- Ferramentas de análise estática (Roslyn Analyzers)
- Métricas de complexidade (cyclomatic, cognitive)
- Padrões de design para problemas específicos
- Refatoração automática com Roslyn Code Fixes
