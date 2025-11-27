# Fase 5 — Essenciais de Interfaces em C#

## Objetivo

Propor duas interfaces relevantes ao domínio de formatação de texto e demonstrar uma classe que implementa ambas. Explicar quando usar implementação explícita, quando generics com constraints ajudam e por que default members em interfaces devem ser evitados em muitos casos.

---

## Interfaces propostas (domínio: formatação de texto)

1) `ITextFormatter` — contrato primário de formatação
```csharp
public interface ITextFormatter
{
    string Format(string text);
}
```
- Responsabilidade: definir o comportamento "formatar". Implementações: `UpperCaseFormatter`, `LowerCaseFormatter`, `TitleCaseFormatter`, etc.

2) `IIdentifiable` — contrato simples de metadados/identificação
```csharp
public interface IIdentifiable
{
    string Id { get; }
    string GetName();
}
```
- Responsabilidade: fornecer identificação/metadata para componentes (útil em logs, discovery, registro e testes).

---

## Classe que implementa duas interfaces

Uma classe pode implementar ambos os contratos quando faz sentido no domínio. Exemplo:

```csharp
public class FancyFormatter : ITextFormatter, IIdentifiable
{
    public string Id { get; } = Guid.NewGuid().ToString();
    public string GetName() => "FancyFormatter";

    public string Format(string text)
    {
        // Lógica de formatação customizada
        return text?.Trim()?.ToUpperInvariant();
    }
}
```

Vantagens: a classe fornece tanto comportamento (formatar) quanto metadados (identificação), podendo ser usada em coleções heterogêneas e em pontos de composição.

---

## Implementação explícita de interface (quando usar)

Cenário: duas interfaces têm membros com o mesmo nome ou você quer esconder a operação por padrão, expondo-a apenas quando o objeto for tratado como aquela interface.

Exemplo:
```csharp
public interface IAlpha { void Show(); }
public interface IBeta  { void Show(); }

public class Dual : IAlpha, IBeta
{
    // Implementação explícita para desambiguar e controlar visibilidade
    void IAlpha.Show() => Console.WriteLine("Alpha");
    void IBeta.Show()  => Console.WriteLine("Beta");

    // Optional: fornecer método público com outro nome
    public void ShowBoth() { ((IAlpha)this).Show(); ((IBeta)this).Show(); }
}
```

Quando usar:
- Quando há colisão de nomes entre interfaces.
- Quando deseja-se que a implementação seja acessível somente via a interface (encapsulamento).
- Para evitar poluição do API público da classe com membros que não devem ser usados diretamente.

Evite quando:
- Você quer que o método seja parte da API pública da classe (prefira implementá-lo publicamente).

---

## Generics com constraints: quando ajudam

Exemplo de fábrica/serviço genérico que aceita apenas formatadores:

```csharp
public class FormatterHost<T>
    where T : ITextFormatter, new()
{
    public string Run(string text)
    {
        var formatter = new T(); // precisa do constraint new()
        return formatter.Format(text);
    }
}
```

Quando usar generics com constraints:
- Se você precisa criar instâncias do tipo genérico (`new()` constraint).
- Se quer garantir em tempo de compilação que `T` tem comportamento mínimo (`ITextFormatter`).
- Para evitar casts/boxed calls e obter performance/segurança de tipo.

Cuidado/alternativas:
- Evite `new()` quando o tipo precisa de dependências (preferir factory/DI).
- Prefira aceitar `ITextFormatter` via parâmetros/DI em vez de usar `new()` quando houver dependências.

---

## Default members em interfaces (caveats)

A partir do C# 8, interfaces podem fornecer implementações default para membros.
Exemplo:
```csharp
public interface IExample
{
    void DoWork() => Console.WriteLine("Default work");
}
```

Por que evitar (geralmente):
- **Compatibilidade e surpreendência**: adicionar um default member altera o comportamento das implementações existentes de forma silenciosa.
- **Ambiguidade de versão**: bibliotecas diferentes podem introduzir membros semelhantes, causando conflitos difíceis de depurar.
- **Testabilidade**: dublês/mocks que dependem apenas do contrato podem não capturar alterações sutis na lógica default.
- **Design coeso**: responsabilidades vazam para a interface (violando a intenção original de interface como contrato mínimo).

Use default members com parcimônia, apenas quando:
- For realmente um comportamento trivial que todas as implementações podem compartilhar sem risco, e
- Houver controle total sobre versões/consumo (ex.: app internal).

---

## Recomendações práticas

- Prefira **interfaces pequenas e estáveis** (SRP/Single Responsibility).
- Use **implementação explícita** quando quiser controlar visibilidade ou desambiguar membros.
- Use **generics com constraints** quando precisar de garantias de tipo em tempo de compilação; prefira factories/DI se houver dependências complexas.
- Evite **default members** em bibliotecas públicas; se usar, documente bem e mantenha versão controlada.

---

## Exemplo completo em C# (arquivo de referência)
Ver `src/fase-05-essenciais-interfaces/InterfacesEssentials.cs` para um exemplo prático com classe que implementa duas interfaces, exemplo de implementação explícita, uso de generic host e nota sobre default members.
