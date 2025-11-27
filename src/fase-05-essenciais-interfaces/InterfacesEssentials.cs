using System;

namespace Fase05Essenciais
{
    // Interface primária de formatação
    public interface ITextFormatter
    {
        string Format(string text);
    }

    // Interface de identificação / metadata
    public interface IIdentifiable
    {
        string Id { get; }
        string GetName();
    }

    // Classe que implementa as duas interfaces
    public class FancyFormatter : ITextFormatter, IIdentifiable
    {
        public string Id { get; } = Guid.NewGuid().ToString();

        public string GetName() => "FancyFormatter";

        // Método público que implementa ITextFormatter
        public string Format(string text)
        {
            // comportamento simples: trim + UPPER
            return text == null ? string.Empty : text.Trim().ToUpperInvariant();
        }

        // Um caso de colisão de nomes: suponha que outra interface declare
        // um membro com o mesmo nome "Describe". Usamos implementação explícita.
    }

    // Interfaces com método com mesmo nome para demonstrar implementação explícita
    public interface IAlpha
    {
        void Describe();
    }

    public interface IBeta
    {
        void Describe();
    }

    // Classe que implementa ambas e usa implementação explícita
    public class DualDescriber : IAlpha, IBeta
    {
        // Implementação explícita: os métodos ficam acessíveis apenas via interface
        void IAlpha.Describe() => Console.WriteLine("Alpha description");
        void IBeta.Describe()  => Console.WriteLine("Beta description");

        // Se quiser oferecer acesso público, exponha um wrapper
        public void DescribeBoth()
        {
            ((IAlpha)this).Describe();
            ((IBeta)this).Describe();
        }
    }

    // Generic host mostrando constraints
    public class FormatterHost<T>
        where T : ITextFormatter, new() // exige que T implemente ITextFormatter e tenha construtor sem parâmetros
    {
        public string Run(string text)
        {
            // Criamos T com 'new()' apenas porque impusemos a constraint
            var formatter = new T();
            return formatter.Format(text);
        }
    }

    // Demonstração de uso
    public static class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("=== Fase 5: Essenciais de Interfaces em C# ===\n");

            var fancy = new FancyFormatter();
            Console.WriteLine($"Formatter {fancy.GetName()} id={fancy.Id} -> {fancy.Format(" hello ")}");

            var dual = new DualDescriber();
            Console.WriteLine("Chamar métodos via wrapper:");
            dual.DescribeBoth();

            Console.WriteLine("Chamar métodos via interface:");
            ((IAlpha)dual).Describe();
            ((IBeta)dual).Describe();

            Console.WriteLine("\nExemplo de FormatterHost<T> (generic with constraints):");
            var host = new FormatterHost<FancyFormatter>();
            Console.WriteLine(host.Run("  generic run  "));

            Console.WriteLine("\nObservações sobre default members:");
            Console.WriteLine("- Preferir evitar em bibliotecas públicas; usar com parcimônia");
        }
    }
}
