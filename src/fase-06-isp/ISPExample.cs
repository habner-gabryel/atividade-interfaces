using System;
using System.Collections.Generic;

namespace Fase06ISP
{
    // --- GOD INTERFACE (antes)
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

    // --- Segregated interfaces (depois)
    public interface ITextFormatter { string Format(string text); }
    public interface IValidator { bool Validate(string text); }
    public interface IPersistable { void Save(string path, string text); string Load(string path); }
    public interface IIdentifiable { string Id { get; } }
    public interface ILogger { void Log(string message); }
    public interface IConfigurable { void Configure(IDictionary<string,string> options); }

    // --- Implementação que antes poderia implementar IFormatterGod
    // Agora implementa apenas os contratos relevantes
    public class SimpleFormatter : ITextFormatter, IValidator, IIdentifiable
    {
        public string Id { get; } = Guid.NewGuid().ToString();

        public string Format(string text) => text?.Trim();
        public bool Validate(string text) => !string.IsNullOrWhiteSpace(text);
    }

    public class FilePersistence : IPersistable
    {
        public void Save(string path, string text)
        {
            // Demo: apenas simula
            Console.WriteLine($"[Persistence] Saving to {path}: '{text}'");
        }

        public string Load(string path)
        {
            Console.WriteLine($"[Persistence] Loading from {path}");
            return "LOADED";
        }
    }

    public class ConsoleLogger : ILogger
    {
        public void Log(string message) => Console.WriteLine($"[Logger] {message}");
    }

    // --- Consumidor ANTES (dependia de IFormatterGod)
    public class ReportGeneratorBefore
    {
        private readonly IFormatterGod _fg;
        public ReportGeneratorBefore(IFormatterGod fg) { _fg = fg; }

        public string Generate(string data)
        {
            if (!_fg.Validate(data)) throw new InvalidOperationException();
            var f = _fg.Format(data);
            _fg.Log("Generated");
            _fg.Save("/tmp/report.txt", f);
            return f;
        }
    }

    // --- Consumidor DEPOIS (dependências mínimas)
    public class ReportGeneratorAfter
    {
        private readonly ITextFormatter _formatter;
        private readonly IValidator _validator;
        private readonly IPersistable _persistence; // opcional
        private readonly ILogger _logger; // opcional

        public ReportGeneratorAfter(ITextFormatter formatter, IValidator validator,
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
            _logger?.Log("Generated");
            _persistence?.Save("/tmp/report.txt", formatted);
            return formatted;
        }
    }

    // --- Demo program
    public static class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("=== FASE 6: ISP na prática ===\n");

            // Implementação simples que combina formatter+validator+id
            var simple = new SimpleFormatter();

            // BEFORE: precisaríamos de um implementor de IFormatterGod (simulado abaixo)
            Console.WriteLine("-- Antes (IFormatterGod) - simulação --");
            var beforeMock = new FormatterGodMock(simple);
            var before = new ReportGeneratorBefore(beforeMock);
            Console.WriteLine(before.Generate(" report data "));

            // AFTER: fornecemos apenas o que o consumidor precisa
            Console.WriteLine("\n-- Depois (interfaces segregadas) --");
            var persistence = new FilePersistence();
            var logger = new ConsoleLogger();
            var after = new ReportGeneratorAfter(simple, simple, persistence, logger);
            Console.WriteLine(after.Generate(" report data "));

            Console.WriteLine("\nSinais que indicam necessidade de segregação:");
            Console.WriteLine("- Clientes com dependências infladas");
            Console.WriteLine("- Testes difíceis de escrever (muitos métodos obrigatórios)");
            Console.WriteLine("- Interface crescendo com métodos não relacionados\n");

            Console.WriteLine("Ganho: menor acoplamento, testes mais simples, responsabilidades claras.");
        }
    }

    // --- Mock que implementa a god interface usando componentes segregados
    public class FormatterGodMock : IFormatterGod
    {
        private readonly ITextFormatter _fmt;
        public FormatterGodMock(ITextFormatter fmt) { _fmt = fmt; }

        public string Format(string text) => _fmt.Format(text);
        public bool Validate(string text) => !string.IsNullOrWhiteSpace(text);
        public void Save(string path, string text) => Console.WriteLine($"[Mock Save] {path}");
        public string Load(string path) => "mocked";
        public string Id => "god-mock";
        public void Log(string message) => Console.WriteLine($"[Mock Log] {message}");
        public void Configure(IDictionary<string, string> options) { /* no-op */ }
    }
}
