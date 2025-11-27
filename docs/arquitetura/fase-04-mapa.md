# Fase 4 — Interface Plugável e Testável

## Objetivo

Definir um contrato claro (`ITextFormatter`) e refatorar o cliente para depender dele. Demonstrar como alternar implementações sem mudar o cliente e como injetar dublês em testes.

---

## Contrato (o que)

```csharp
public interface ITextFormatter
{
    string Format(string text);
    string GetName();
}
```

- `ITextFormatter` é pequeno, explícito e estável — permite trocar implementações sem tocar o cliente.

---

## Implementações (como)

- `UpperCaseFormatter` — implementa `ITextFormatter`, retorna `text.ToUpperInvariant()`
- `LowerCaseFormatter` — `text.ToLowerInvariant()`
- `TitleCaseFormatter` — capitaliza palavras via `Regex`/TextInfo
- `DefaultFormatter` — primeira letra maiúscula
- `PassthroughFormatter` — retorna original
- `MockFormatter` — dublê para testes (retorna valor controlado)

---

## Ponto de composição (onde injetar)

- Em runtime: via `FormatterFactory.Create(mode)` ou configuração (arquivo/env)
- Em composição de app: via DI Container (ex.: `services.AddTransient<ITextFormatter, UpperCaseFormatter>()`)
- Em testes: instanciar `FormatterService` passando um dublê que implementa `ITextFormatter`

---

## Código: Cliente dependente da abstração

```csharp
public class FormatterService
{
    private readonly ITextFormatter _formatter;
    public FormatterService(ITextFormatter formatter) { _formatter = formatter; }
    public string Render(string text) => _formatter.Format(text);
}
```

- Observação: o cliente não cria ou conhece a implementação; ele só usa o contrato.

---

## Como alternar implementações sem mudar o cliente

1. Configure a implementação desejada na composição da aplicação (Factory / DI).
2. O `FormatterService` recebe a implementação via injeção de dependência.
3. Para trocar comportamento em produção, altere a composição (configuração/DI), não o cliente.

---

## Como dobrar (mock) a dependência em testes

- Em testes unitários, crie um dublê que implemente `ITextFormatter` (fake, stub ou mock).
- Injete o dublê no `FormatterService` para controlar o retorno e verificar cenários (sucesso, falha, exceções).

Exemplo (pseudocódigo de teste):

```csharp
// arrange
var fake = new FakeFormatter() { /* controla retorno */ };
var service = new FormatterService(fake);

// act
var result = service.Render("qualquer");

// assert
Assert.Equal("FAKE_OK", result);
```

---

## Benefícios

- Cliente depende de abstração estável → baixo acoplamento
- Troca de implementação sem recompilar cliente
- Testes fáceis: injetar dublês simples ou usar frameworks de mock
- Composição centralizável (Factory/DI) com configuração

---

## Exemplo rápido de composição via Factory

```csharp
ITextFormatter fmt = FormatterFactory.Create(mode);
var service = new FormatterService(fmt);
var result = service.Render(text);
```

---

## Notas finais

- A Interface deve ser mínima e estável; evite expor detalhes desnecessários.
- Fase 4 prepara o terreno para injeção de dependência completa e composição por configuração em Fase 5.
