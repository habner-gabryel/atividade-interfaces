using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Fase04ComInterface
{
    // CONTRATO: Interface estável que define o contrato para formatadores de texto
    public interface ITextFormatter
    {
        string Format(string text);
        string GetName();
    }

    // Implementações concretas que agora implementam a interface
    public sealed class UpperCaseFormatter : ITextFormatter
    {
        public string Format(string text) => text?.ToUpperInvariant() ?? string.Empty;
        public string GetName() => "UPPER";
    }

    public sealed class LowerCaseFormatter : ITextFormatter
    {
        public string Format(string text) => text?.ToLowerInvariant() ?? string.Empty;
        public string GetName() => "LOWER";
    }

    public sealed class TitleCaseFormatter : ITextFormatter
    {
        public string Format(string text)
        {
            return Regex.Replace(text ?? string.Empty, "\\b(\\p{L})", m => m.Value.ToUpperInvariant());
        }
        public string GetName() => "TITLE";
    }

    public sealed class DefaultFormatter : ITextFormatter
    {
        public string Format(string text)
        {
            if (string.IsNullOrEmpty(text)) return text ?? string.Empty;
            return char.ToUpper(text[0]) + text.Substring(1);
        }
        public string GetName() => "DEFAULT";
    }

    public sealed class PassthroughFormatter : ITextFormatter
    {
        public string Format(string text) => text ?? string.Empty;
        public string GetName() => "PASSTHROUGH";
    }

    // Serviço que depende da abstração (ITextFormatter). Não conhece implementações concretas.
    public class FormatterService
    {
        private readonly ITextFormatter _formatter;

        public FormatterService(ITextFormatter formatter)
        {
            _formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
        }

        public string Render(string text)
        {
            Console.WriteLine($"[Service] Usando formatter: {_formatter.GetName()}");
            return _formatter.Format(text);
        }
    }

    // Factory simples para demonstração de composição; em projetos reais a composição viria de DI/Config
    public static class FormatterFactory
    {
        public static ITextFormatter Create(string mode) => mode?.ToUpperInvariant() switch
        {
            "UPPER" => new UpperCaseFormatter(),
            "LOWER" => new LowerCaseFormatter(),
            "TITLE" => new TitleCaseFormatter(),
            "DEFAULT" => new DefaultFormatter(),
            _ => new PassthroughFormatter()
        };
    }

    // Demo: mostra que o cliente não precisa mudar para alternar implementações
    public static class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("=== FASE 4: Interface Plugável e Testável ===\n");

            // Composição em tempo de execução (Factory) - cliente não precisa alterar
            var formatterA = FormatterFactory.Create("UPPER");
            var serviceA = new FormatterService(formatterA);
            Console.WriteLine(serviceA.Render("hello world"));

            var formatterB = FormatterFactory.Create("TITLE");
            var serviceB = new FormatterService(formatterB);
            Console.WriteLine(serviceB.Render("joão da silva"));

            // Injeção explícita (ex.: teste) - demonstrando dublê simples
            var mockFormatter = new MockFormatter(true);
            var testService = new FormatterService(mockFormatter);
            Console.WriteLine(testService.Render("ignored in mock"));

            Console.WriteLine("\n=== Como alternar implementações sem mudar o cliente ===");
            Console.WriteLine("- Cliente recebe uma ITextFormatter via construtor (injeção simples)");
            Console.WriteLine("- Para trocar implementação: passar outra ITextFormatter (Factory/DI/config)");

            Console.WriteLine("\n=== Como dobrar a dependência em testes ===");
            Console.WriteLine("- Em testes, injete um dublê (mock/fake) que implementa ITextFormatter");
            Console.WriteLine("- Exemplo abaixo usa MockFormatter que retorna um valor controlado\n");
        }

        // Um dublê simples para demonstração (poderia ser substituído por um mock framework)
        private sealed class MockFormatter : ITextFormatter
        {
            private readonly bool _resultToggle;
            public MockFormatter(bool resultToggle) { _resultToggle = resultToggle; }
            public string Format(string text) => _resultToggle ? "MOCK_OK" : "MOCK_FAIL";
            public string GetName() => "MOCK";
        }
    }
}
