## Fase 11 — Cheiros e Antídotos

**Tema:** Identificar code smells em fases anteriores e descrever refatorações

### Objetivo
Analisar padrões problemáticos encontrados em fases anteriores e mostrar como eliminar acoplamento, duplicação e rigidez através de refatoração cuidadosa. Cada cheiro é ilustrado com antes/depois e a motivação por trás do antídoto.

---

## Cheiro 1: "God Interface" (Fase 4 → Fase 6)

### Identificação

**Fase 4 — TextFormatterInterface.cs:**
```csharp
public interface ITextFormatter
{
    string Format(string text);
    void ValidateInput(string input);
    void LogOperation(string message);
    string Describe();
}
```

**Sinais:**
1. ❌ Interface com múltiplas responsabilidades (formatação + validação + logging + descrição)
2. ❌ Cliente que só quer formatação é forçado a implementar logging e descrição
3. ❌ Mudanças em logging quebram formatadores que não usam logging
4. ❌ Teste unitário precisa mockar 4 métodos para testar 1

### Motivação

A interface assume que TUDO que formata também valida, loga e se descreve. Realidade:
- `UpperCaseFormatter` só quer `Format()`
- `FileLogger` só quer `LogOperation()`
- `Inspector` só quer `Describe()`

Forçar implementar todos quebra o **Interface Segregation Principle (ISP)**.

### Antídoto: ISP (Interface Segregation Principle)

**Depois (Fase 6 — ISPExample.cs):**
```csharp
// Interfaces coesas e focadas
public interface ITextFormatter
{
    string Format(string text);
}

public interface IValidator
{
    bool IsValid(string input);
    string GetErrorMessage();
}

public interface IDescribable
{
    string Describe();
}

// Agora cliente escolhe quais implementar
public class SimpleFormatter : ITextFormatter
{
    public string Format(string text) => text.ToUpper();
    // Pronto! Sem métodos inúteis.
}

public class ValidatingFormatter : ITextFormatter, IValidator
{
    public string Format(string text) => text.ToUpper();
    public bool IsValid(string input) => !string.IsNullOrEmpty(input);
    public string GetErrorMessage() => "Input não pode ser vazio";
}
```

**Ganhos:**
- ✅ Cliente implementa APENAS métodos que usa
- ✅ Mudanças em `IValidator` não quebram `SimpleFormatter`
- ✅ Testes unitários mockar só o necessário
- ✅ Novas interfaces podem ser criadas sem alterar existentes

---

## Cheiro 2: "Composição Espalhada" (Fase 3 → Fase 4)

### Identificação

**Fase 3 — TextFormatterPolymorphic.cs:**
```csharp
public class FormatterClient
{
    public string FormatText(string text, string mode)
    {
        TextFormatterBase formatter = mode switch
        {
            "UPPER" => new UpperCaseFormatter(),
            "LOWER" => new LowerCaseFormatter(),
            "TITLE" => new TitleCaseFormatter(),
            "DEFAULT" => new DefaultFormatter(),
            _ => new PassthroughFormatter()
        };
        
        return formatter.Format(text);
    }
}

public class AnotherClient
{
    public string TransformText(string text, string mode)
    {
        TextFormatterBase formatter = mode switch
        {
            "UPPER" => new UpperCaseFormatter(),
            "LOWER" => new LowerCaseFormatter(),
            // ... duplicado!
        };
        
        return formatter.Format(text);
    }
}
```

**Sinais:**
1. ❌ Switch (`mode`) aparece em múltiplos clientes
2. ❌ Adicionar novo modo exige editar TODOS os clientes
3. ❌ Decisão de composição espalhada, não centralizada
4. ❌ Risco de inconsistência (um cliente esquece um modo)

### Motivação

Cada cliente conhece COMO escolher qual formatter usar. Se surgir novo modo ou remover existente, todos quebram.

A decisão "qual implementação usar?" deve estar **em um único lugar**.

### Antídoto: Factory Pattern (Decisão Centralizada)

**Depois (Fase 4 — TextFormatterInterface.cs):**
```csharp
// Decisão CENTRALIZADA em uma Factory
public class FormatterFactory
{
    public static ITextFormatter CreateFormatter(string mode)
    {
        return mode.ToUpperInvariant() switch
        {
            "UPPER" => new UpperCaseFormatter(),
            "LOWER" => new LowerCaseFormatter(),
            "TITLE" => new TitleCaseFormatter(),
            "DEFAULT" => new DefaultFormatter(),
            _ => new PassthroughFormatter()
        };
    }
}

// Cliente 1: simples, sem switch
public class FormatterClient
{
    public string FormatText(string text, string mode)
    {
        var formatter = FormatterFactory.CreateFormatter(mode);
        return formatter.Format(text);
    }
}

// Cliente 2: idêntico, sem repetição
public class AnotherClient
{
    public string TransformText(string text, string mode)
    {
        var formatter = FormatterFactory.CreateFormatter(mode);
        return formatter.Format(text);
    }
}

// Adicionar novo modo? Edita APENAS a Factory:
public class FormatterFactory
{
    public static ITextFormatter CreateFormatter(string mode)
    {
        return mode.ToUpperInvariant() switch
        {
            "UPPER" => new UpperCaseFormatter(),
            "LOWER" => new LowerCaseFormatter(),
            "TITLE" => new TitleCaseFormatter(),
            "DEFAULT" => new DefaultFormatter(),
            "REVERSE" => new ReverseFormatter(),  // ← NOVO, sem quebrar clientes
            _ => new PassthroughFormatter()
        };
    }
}
```

**Ganhos:**
- ✅ Decisão centralizada em um único lugar
- ✅ Clientes desacoplados da escolha
- ✅ Adicionar modo novo: edita só a Factory
- ✅ Consistência garantida
- ✅ Teste da Factory isolado

---

## Comparação: Antes vs. Depois

| Aspecto | Fase 3/4 (Cheiro) | Fase 4 (Antídoto 1) | Fase 6 (Antídoto 2) |
|--------|------------------|-------------------|-------------------|
| **Interface God** | Sim, tudo junto | Um pouco melhor | ✅ Segregado (ISP) |
| **Switch espalhado** | Sim, múltiplos clientes | ✅ Centralizado (Factory) | ✅ Centralizado |
| **Fácil estender** | ❌ Quebra clientes | ✅ Só Factory muda | ✅ Factory + nova interface |
| **Testes** | ❌ Mockar tudo | ✅ Mockar só `ITextFormatter` | ✅ Mockar interface específica |
| **Manutenção** | ❌ Alto risco | ✅ Médio | ✅ Baixo risco |

---

## Padrões de Refatoração Aplicados

### 1. ISP (Interface Segregation Principle)
**Problema:** Interface faz tudo
**Solução:** Dividir em interfaces pequenas, focadas

```csharp
// ❌ Antes
public interface IBigThing { void A(); void B(); void C(); }

// ✅ Depois
public interface IDoA { void A(); }
public interface IDoB { void B(); }
public interface IDoC { void C(); }
```

### 2. Factory Pattern (Decisão Centralizada)
**Problema:** Switch espalhado em múltiplos clientes
**Solução:** Centralizar em uma Factory

```csharp
// ❌ Antes: em cada cliente
var obj = mode switch { ... };

// ✅ Depois: em uma Factory
var obj = Factory.Create(mode);
```

### 3. Composição sobre Herança
**Problema:** Herança profunda com muita variedade
**Solução:** Compor interfaces simples

```csharp
// ❌ Antes: herança rígida
public class FormatterWithLogging : FormatterWithValidation { ... }

// ✅ Depois: composição flexível
public class Logger : ILogger { ... }
public class Formatter : IFormatter { ... }
var combo = new Formatter { Logger = new Logger() };
```

---

## Técnicas para Detectar Code Smells

1. **God Interface:** Interface com 4+ métodos não relacionados
2. **Switch espalhado:** Switch similar em 2+ lugares → centralizar
3. **Classe grande:** >500 linhas → dividir responsabilidades
4. **Duplicação:** Código copiado → extrair em método/classe
5. **Acoplamento:** Cliente conhece concretude → usar interface
6. **Rigidez:** Adicionar feature quebra tudo → refatorar composição

---

## Checklist de Refatoração

Antes de refatorar, verifique:
- ✅ Testes escritos e passando (baseline)
- ✅ Entender por que código é problemático
- ✅ Planejar o antídoto (ISP, Factory, etc.)
- ✅ Refatoração pequena (um change por vez)
- ✅ Testes passam após cada mudança
- ✅ Validar que comportamento observável não mudou

