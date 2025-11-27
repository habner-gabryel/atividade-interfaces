# Fase 3 — OO sem Interface (Herança + Polimorfismo)

## Transformação da Fase 2

Na **Fase 2**, usávamos uma função procedural única com múltiplos `if/switch` para escolher como formatar:

```csharp
// ❌ Fase 2: Procedural (decisões no fluxo)
public string FormatarTexto(string text, string mode)
{
    if (mode == "UPPER")
        return text.ToUpper();
    else if (mode == "LOWER")
        return text.ToLower();
    else if (mode == "TITLE")
        return ToTitleCase(text);
    else
        return DefaultFormat(text);  // ← múltiplas decisões aqui
}
```

**Problema:** A função conhece todos os detalhes; adicionar novo modo exige mexer aqui.

---

## Solução: Herança + Polimorfismo

Na **Fase 3**, substituímos ramificações explícitas por **delegação polimórfica**:

```csharp
// ✅ Fase 3: OO sem Interface (decisões delegadas)
public abstract class TextFormatterBase
{
    public string Format(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return "";
        return Apply(text);  // ← Delega o "como" à subclasse
    }

    protected abstract string Apply(string text);  // Template Method
}

public sealed class UpperCaseFormatter : TextFormatterBase
{
    protected override string Apply(string text) 
        => text?.ToUpperInvariant() ?? "";
}

public sealed class LowerCaseFormatter : TextFormatterBase
{
    protected override string Apply(string text) 
        => text?.ToLowerInvariant() ?? "";
}

// ... TitleCaseFormatter, DefaultFormatter, PassthroughFormatter
```

---

## Diagrama da Hierarquia

```
                  TextFormatterBase (abstrato)
                  ├─ Format(text) : string  [ritual comum]
                  └─ Apply(text) : string   [gancho variável - abstract]
                       ↑
        ┌──────────────┼──────────────┬──────────────┬─────────────┐
        │              │              │              │             │
    UpperCase      LowerCase       Title        Default      Passthrough
    Formatter      Formatter    Formatter     Formatter      Formatter
    (UPPER)        (LOWER)      (TITLE)       (DEFAULT)     (PASSTHROUGH)
    ────────────   ─────────    ─────────     ────────      ────────────
    ToUpperInv()   ToLowerInv()  Regex+Cap    Upper[0]      No-op
```

**Padrão:** Template Method + Polimorfismo

---

## Código: Classe Base

```csharp
public abstract class TextFormatterBase
{
    // Ritual comum (Template Method)
    public string Format(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return "";
        
        // Delegação polimórfica → subclasse define o "como"
        return Apply(text);
    }

    // Gancho abstrato: subclasses implementam o passo variável
    protected abstract string Apply(string text);

    // Identificação (útil para logs)
    public abstract string GetFormatterName();
}
```

**Por que funciona:**
- Método `Format()` é **comum a todas** as variações
- Método `Apply()` é **específico de cada variação**
- Polimorfismo resolve a chamada correta em runtime

---

## Código: Exemplos de Concretas

### Maiúsculas (UPPER)
```csharp
public sealed class UpperCaseFormatter : TextFormatterBase
{
    protected override string Apply(string text)
        => text?.ToUpperInvariant() ?? "";

    public override string GetFormatterName() => "UPPER";
}
```

### Minúsculas (LOWER)
```csharp
public sealed class LowerCaseFormatter : TextFormatterBase
{
    protected override string Apply(string text)
        => text?.ToLowerInvariant() ?? "";

    public override string GetFormatterName() => "LOWER";
}
```

### Título (TITLE)
```csharp
public sealed class TitleCaseFormatter : TextFormatterBase
{
    protected override string Apply(string text)
    {
        return Regex.Replace(
            text ?? string.Empty,
            @"\b(\p{L})",
            m => m.Value.ToUpperInvariant()
        );
    }

    public override string GetFormatterName() => "TITLE";
}
```

### Padrão (DEFAULT)
```csharp
public sealed class DefaultFormatter : TextFormatterBase
{
    protected override string Apply(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;
        return char.ToUpper(text[0]) + text.Substring(1);
    }

    public override string GetFormatterName() => "DEFAULT";
}
```

---

## Cliente: Remoção do if/switch do Fluxo

### Antes (Fase 2 — Procedural)
```csharp
public string FormatarTexto(string text, string mode)
{
    // ❌ Ramificações aqui DENTRO do fluxo
    if (mode == "UPPER")
        return ProcessarUpper(text);
    else if (mode == "LOWER")
        return ProcessarLower(text);
    else if (mode == "TITLE")
        return ProcessarTitle(text);
    else
        return ProcessarDefault(text);
}
```

### Depois (Fase 3 — Polimorfismo)
```csharp
public static string Render(string text, string mode)
{
    // ✅ Switch APENAS para escolher qual concreta usar
    TextFormatterBase formatter = mode?.ToUpper() switch
    {
        "UPPER" => new UpperCaseFormatter(),
        "LOWER" => new LowerCaseFormatter(),
        "TITLE" => new TitleCaseFormatter(),
        _ => new PassthroughFormatter()
    };

    // ✅ Fluxo é agora LINEAR: sem if/else, apenas delegação
    return formatter.Format(text);
}
```

**Diferença crítica:**
- **Antes:** `if/else` decidia O QUÊ fazer **dentro** de cada chamada
- **Depois:** `switch` escolhe QUAL CLASSE usar **antes** de iniciar; o fluxo é polimórfico

---

## Comparação: Fase 2 vs. Fase 3

| Aspecto | Fase 2 (Procedural) | Fase 3 (Polimorfismo) |
|--------|------|-----|
| **Fluxo de decisão** | if/else/if/else → vários caminhos | Método único (Format) com delegação |
| **Local das ramificações** | Dentro da função central | Apenas na composição inicial |
| **Coesão** | Lógica misturada (4+ modos no mesmo lugar) | Cada modo em sua classe |
| **Novo modo** | Adiciona novo else if; recompila tudo | Cria nova classe; client muda |
| **Testes** | Testar um modo afeta toda a função | Cada classe testada isoladamente |
| **Leitura** | Complexa (múltiplas branches) | Simples (cada classe é simples) |

---

## Análise: Melhorou vs. Ainda Rígido

### ✅ MELHOROU

1. **Remoção de if/switch do fluxo central**
   - Antes: método com 4+ ramificações
   - Agora: método único linear (Template Method)
   - Resultado: código mais legível

2. **Coesão por variação**
   - Antes: lógica de UPPER, LOWER, TITLE misturada
   - Agora: cada variação em sua classe
   - Resultado: fácil entender uma classe de cada vez

3. **Testes isolados e focados**
   - Antes: teste de UPPER afeta validação, normalização, etc.
   - Agora: teste de `UpperCaseFormatter` testa apenas `Apply()`
   - Resultado: testes rápidos e menos acoplados

4. **Novo modo sem alterar fluxo**
   - Antes: adicionar REVERSE exige novo else if
   - Agora: criar `ReverseFormatter : TextFormatterBase` e pronto
   - Resultado: Open/Closed Principle parcial (classe sim, cliente não)

### ⚠️ AINDA FICOU RÍGIDO

1. **Cliente AINDA conhece concretos**
   - Problema: `new UpperCaseFormatter()`, `new LowerCaseFormatter()`, ...
   - Impacto: Cliente deve ser recompilado ao trocar/adicionar variação
   - Exemplo: Adicionar `ReverseFormatter` exige mexer no switch do cliente

2. **Composição dispersa (sem contrato centralizado)**
   - Problema: Seleção de qual concreta usar está no switch do cliente
   - Impacto: Política de seleção não é centralizada nem configurável
   - Exemplo: Trocar critério "se UPPER então..." espalhado em múltiplos clientes

3. **Sem contrato estável (sem interface)**
   - Problema: Cliente conhece `TextFormatterBase` (classe abstrata)
   - Impacto: Difícil testar com dublês; acoplamento ainda existe
   - Exemplo: Não há contrato `ITextFormatter` → teste não consegue mockar claramente

4. **Ainda requer recompilação**
   - Problema: Novo modo exige recompilação do cliente
   - Impacto: Sem extensibilidade em runtime
   - Exemplo: Não é possível "plugar" novo formatter sem mexer no código

---

## Roadmap: De Fase 3 para Fase 4

```
Fase 3 (Atual): OO sem Interface
│
├─ ✅ Removeu if/switch do fluxo
├─ ✅ Delegou variabilidade para subclasses
└─ ❌ Cliente ainda conhece concretos

        ↓ Introduzir Interface

Fase 4 (Próxima): OO com Interface
│
├─ ✅ Contrato estável (ITextFormatter)
├─ ✅ Cliente depende de abstração
├─ ✅ Composição centralizada (Factory/DI)
└─ ✅ Novo modo sem mexer no cliente
```

---

## Exemplo: Preview da Fase 4

```csharp
// Contrato estável (será adicionado na Fase 4)
public interface ITextFormatter
{
    string Format(string text);
}

// Cliente depende da interface, não de concretos
public class FormatterService
{
    private readonly ITextFormatter _formatter;
    
    public FormatterService(ITextFormatter formatter)
    {
        _formatter = formatter;  // Injeção de dependência
    }
    
    public string Render(string text) 
        => _formatter.Format(text);  // Sem saber qual concreta é
}

// Composição centralizada (Factory)
public class FormatterFactory
{
    public static ITextFormatter Create(string mode) => mode switch
    {
        "UPPER" => new UpperCaseFormatter(),
        "LOWER" => new LowerCaseFormatter(),
        // ...
        _ => new PassthroughFormatter()
    };
}
```

---

## Resumo: Ganho Evolutivo

| Métrica | Fase 2 | Fase 3 | Ganho |
|---------|--------|--------|-------|
| **Decisões no fluxo** | 4+ if/else | 1 switch | ↓ Reduz 75% |
| **Acoplamento** | Alto | Médio | ↓ Melhor |
| **Coesão** | Baixa | Alta | ↑ Melhor |
| **Testes** | Difíceis | Fáceis | ↑ Simples |
| **Extensibilidade** | Recompila tudo | Recompila cliente | ↑ Melhor |

**Conclusão:** Fase 3 consolida a virada de "decidir dentro da função" → "delegar para subclasses" (polimorfismo).
