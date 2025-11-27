# Fase 6 — ISP na prática (Segregação por Capacidade)

## Enunciado

Dada uma interface "onipotente" ("god interface"), segregue-a em contratos coesos e ajuste o consumidor para depender apenas do que precisa.

---

## Antes — A "God Interface"

Imagine uma interface que tentou agrupar todas as responsabilidades relacionadas a um componente de formatação/gestão:

```csharp
public interface IFormatterGod
{
    string Format(string text);
    bool Validate(string text);
    void Save(string path, string text);
    string Load(string path);
    string Id { get; }
    void Log(string message);
    void Configure(IDictionary<string,string> options);
}
```

Problemas observados com essa abordagem:
- Cliente que só precisa formatar acaba dependendo de métodos de I/O, logging e configuração.
- Dificuldade para testar (é preciso prover implementações ou dublês para tudo).
- Viola o Interface Segregation Principle (ISP): clientes não devem ser forçados a depender de métodos que não usam.

---

## Depois — Segregando em contratos coesos

Extrair responsabilidades em interfaces pequenas e coesas:

```csharp
public interface ITextFormatter { string Format(string text); }
public interface IValidator { bool Validate(string text); }
public interface IPersistable { void Save(string path, string text); string Load(string path); }
public interface IIdentifiable { string Id { get; } }
public interface ILogger { void Log(string message); }
public interface IConfigurable { void Configure(IDictionary<string,string> options); }
```

Cada cliente agora pode declarar dependência apenas nas interfaces que efetivamente utiliza.

---

## Exemplo prático (antes / depois em código)

### Antes (consumidor dependente de IFormatterGod)
```csharp
public class ReportGenerator
{
    private readonly IFormatterGod _formatterGod;
    public ReportGenerator(IFormatterGod formatterGod) { _formatterGod = formatterGod; }

    public string Generate(string data)
    {
        if (!_formatterGod.Validate(data)) throw new InvalidOperationException();
        var formatted = _formatterGod.Format(data);
        _formatterGod.Log("Generated report");
        _formatterGod.Save("/tmp/report.txt", formatted);
        return formatted;
    }
}
```

Problemas: ReportGenerator depende de Log/Save/Validate mesmo que seu objetivo primário seja apenas obter o texto formatado para retorno.

### Depois (consumidor dependente de interfaces segregadas)
```csharp
public class ReportGenerator
{
    private readonly ITextFormatter _formatter;
    private readonly IValidator _validator;
    private readonly IPersistable _persistence; // opcional, se precisar salvar
    private readonly ILogger _logger; // opcional

    public ReportGenerator(ITextFormatter formatter, IValidator validator,
                           IPersistable persistence = null, ILogger logger = null)
    {
        _formatter = formatter;
        _validator = validator;
        _persistence = persistence;
        _logger = logger;
    }

    public string Generate(string data)
    {
        if (!_validator.Validate(data)) throw new InvalidOperationException();
        var formatted = _formatter.Format(data);
        _logger?.Log("Generated report");
        _persistence?.Save("/tmp/report.txt", formatted);
        return formatted;
    }
}
```

Benefício: o `ReportGenerator` só depende do que precisa. Em testes, basta prover um `ITextFormatter` e um `IValidator` dublês; não é necessário prover persistência nem logging.

---

## Sinais que indicam necessidade de segregação

- Clientes recebendo muitas dependências onde apenas 1–2 métodos são usados.
- Testes que precisam criar implementações muito grandes apenas para satisfazer o contrato.
- Presença de métodos `Save`, `Load`, `Configure`, `Log`, etc. na mesma interface de operações simples como `Format`.
- Adição frequente de métodos à mesma interface ao longo do tempo.

---

## Ganhos obtidos após segregação

- **Menor acoplamento:** clientes dependem de contratos mínimos.
- **Testabilidade melhorada:** dublês mais simples (stubs/mocks) necessários.
- **Coesão de interfaces:** cada contrato tem responsabilidade única e testável.
- **Evolução mais segura:** adicionar método a um contrato não força recompilação/reescrita de clientes que não usam esse método.

---

## Nota sobre composição e injeção

- Em aplicações reais, use DI container / factory para montar objetos compostos.
- Composição por interfaces permite trocar implementações (ex.: persistência em disco vs. nuvem) sem alterar clientes.

---

## Onde encontrar os exemplos
- Código: `src/fase-06-isp/ISPExample.cs`
- Guia: `src/fase-06-isp/README.md`
