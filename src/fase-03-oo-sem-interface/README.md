# Fase 3 — OO sem Interface (Herança + Polimorfismo)

## 📋 Visão Geral

Transformação da **Fase 2** (procedural com if/switch) para uma hierarquia orientada a objetos com **herança** e **polimorfismo**. Substituímos decisões explícitas por delegação: cada variação implementa seu próprio "como" via `override`.

**Objetivo:** Demonstrar que polimorfismo remove ramificações do fluxo central, mas o cliente ainda conhece concretos (será resolvido na Fase 4 com interfaces).

---

## 📁 Artefatos

### `fase-03-mapa.md`
Documento de design (com snippets de código) contendo:
- ✅ Transformação de Fase 2 → Fase 3
- ✅ Diagrama da hierarquia (base abstrata + 5 concretas)
- ✅ Código: classe base e exemplos de subclasses
- ✅ Cliente: remoção do if/switch do fluxo
- ✅ Comparação: Fase 2 vs. Fase 3
- ✅ Análise "melhorou vs. ainda rígido"
- ✅ Preview: como Fase 4 resolverá a rigidez

**Acesse:** [`docs/arquitetura/fase-03-mapa.md`](../../docs/arquitetura/fase-03-mapa.md)

### `TextFormatterPolymorphic.cs`
Implementação completa demonstrando:
- Classe base abstrata `TextFormatterBase`
- 5 subclasses concretas (Upper, Lower, Title, Default, Passthrough)
- Cliente `FormatterClient` com remoção de if/switch do fluxo
- Testes cobrindo: casos reais, entrada vazia, instância explícita, polimorfismo em array
- Análise final (melhorou vs. ainda rígido)

**Execução:**
```bash
cd src/fase-03-oo-sem-interface
csc TextFormatterPolymorphic.cs
TextFormatterPolymorphic.exe
```

---

## 🏗️ Hierarquia de Classes

```
TextFormatterBase (abstrato)
├─ Format(text)      ← Ritual comum (Template Method)
└─ Apply(text)       ← Gancho abstrato (implementado por cada concreta)
    ↑
    ├─ UpperCaseFormatter     → "HELLO"
    ├─ LowerCaseFormatter     → "hello"
    ├─ TitleCaseFormatter     → "Hello World"
    ├─ DefaultFormatter       → "Hello world"
    └─ PassthroughFormatter   → "hello" (sem mudança)
```

---

## 🔑 Padrão: Template Method

```csharp
public abstract class TextFormatterBase
{
    // Template Method: define estrutura comum
    public string Format(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return "";
        
        // Delegação: passo variável definido por subclasse
        return Apply(text);
    }

    // Gancho: subclasses implementam o "como"
    protected abstract string Apply(string text);
}

// Subclass: implementa apenas o passo variável
public sealed class UpperCaseFormatter : TextFormatterBase
{
    protected override string Apply(string text) 
        => text?.ToUpperInvariant() ?? "";
}
```

**Benefício:** Lógica comum (validação, normalização) em um lugar; variabilidade isolada.

---

## 📊 Comparação: Fase 2 vs. Fase 3

| Aspecto | Fase 2 (Procedural) | Fase 3 (Polimorfismo) |
|--------|---|---|
| **Fluxo** | if/else/if/else | Método único linear |
| **Ramificações** | Dentro da função | Apenas composição inicial |
| **Coesão** | Baixa (4+ modos misturados) | Alta (cada classe isolada) |
| **Novo modo** | Novo else if; recompila tudo | Nova classe; cliente muda |
| **Testes** | Acoplados (testar 1 modo afeta tudo) | Isolados (testar `UpperCaseFormatter` só) |

---

## ✅ Melhorou

1. **Remoção de if/switch do fluxo central**
   - Antes: método com 4+ ramificações
   - Agora: método único, linear (Template Method)
   - Efeito: código mais legível e fácil de seguir

2. **Coesão por variação**
   - Antes: UPPER, LOWER, TITLE misturados em uma função
   - Agora: cada modo em sua própria classe
   - Efeito: classes pequenas, específicas, fáceis de entender

3. **Testes isolados e focados**
   - Antes: teste de UPPER precisa testar validação, normalização, etc.
   - Agora: `UpperCaseFormatterTest` testa apenas `Apply()`
   - Efeito: testes rápidos, independentes, menos frágeis

4. **Novo modo sem alterar fluxo**
   - Antes: adicionar REVERSE exige novo else if na função
   - Agora: criar `ReverseFormatter : TextFormatterBase`
   - Efeito: Open/Closed Principle parcial (classe aberta, cliente não)

---

## ⚠️ Ainda Ficou Rígido

1. **Cliente AINDA conhece concretos**
   - Problema: `new UpperCaseFormatter()`, `new LowerCaseFormatter()`
   - Impacto: cliente recompilado ao adicionar nova variação
   - Exemplo: Novo `ReverseFormatter` exige mexer no switch do cliente

2. **Composição dispersa**
   - Problema: switch de seleção está dentro do cliente
   - Impacto: política de seleção não é centralizada nem configurável
   - Exemplo: Trocar critério "se UPPER então..." exige mexer em múltiplos lugares

3. **Sem contrato estável**
   - Problema: cliente conhece `TextFormatterBase` (classe abstrata)
   - Impacto: difícil testar com dublês (mocks)
   - Exemplo: não há interface → não há contrato bem definido

4. **Extensibilidade limitada**
   - Problema: novo modo exige recompilação do cliente
   - Impacto: sem extensibilidade em runtime ou via plugins
   - Exemplo: não é possível "plugar" formatter via arquivo de configuração

---

## 🎯 Próxima Fase (4)

**Solução:** Introduzir **interface** como contrato estável:

```csharp
// ✅ Contrato (será adicionado em Fase 4)
public interface ITextFormatter
{
    string Format(string text);
}

// ✅ Cliente depende da abstração, não de concretos
public class FormatterService
{
    private readonly ITextFormatter _formatter;
    
    public FormatterService(ITextFormatter formatter)
    {
        _formatter = formatter;
    }
    
    public string Render(string text) 
        => _formatter.Format(text);
}

// ✅ Composição centralizada (Factory/DI)
public class FormatterFactory
{
    public static ITextFormatter Create(string mode) => mode switch
    {
        "UPPER" => new UpperCaseFormatter(),
        "LOWER" => new LowerCaseFormatter(),
        _ => new PassthroughFormatter()
    };
}
```

**Resultado:**
- Cliente depende de interface, não de concretos
- Novo modo não afeta cliente (contrato estável)
- Testes podem mockar interface facilmente
- Composição centralizada e configurável

---

## 📚 Referências

- [Mapa Mental Detalhado (Fase 3)](../../docs/arquitetura/fase-03-mapa.md)
- [Fase 2 — Procedural](../fase-02-procedural/README.md)
- [Fase 1 — Heurística](../fase-01-procedural/README.md)
- [README Principal](../../README.md)

---

## 🔄 Evolução Visual

```
Fase 1 (Heurística/Análise)
  ↓
Fase 2 (Procedural: if/switch no fluxo)
  ↓
Fase 3 (OO sem Interface: polimorfismo, cliente ainda conhece concretos)
  ↓
Fase 4 (OO com Interface: cliente desacoplado, contrato estável)
  ↓
Fase 5+ (Patterns avançados: Factory, Strategy, DI Container)
```
