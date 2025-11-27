using System;
using System.Collections.Generic;

// ============================================================================
// CHEIRO 1: GOD INTERFACE (Antes - Fase 4 estilo)
// ============================================================================

// ❌ CHEIRO: Uma interface grande fazendo tudo
public interface IBadFormatter
{
    string Format(string text);           // Formatação
    void ValidateInput(string input);     // Validação
    void LogOperation(string message);    // Logging
    string Describe();                    // Descrição
}

// Cliente forçado a implementar TUDO mesmo que não use
public class BadUpperFormatter : IBadFormatter
{
    public string Format(string text) => text.ToUpper();
    
    // Implementações vazias de métodos que não usa
    public void ValidateInput(string input) { } // Não faz nada
    public void LogOperation(string message) { } // Não faz nada
    public string Describe() => "Não me descrevo"; // Não faz nada
}

// ✅ ANTÍDOTO: ISP - Segregar em interfaces pequenas
public interface IGoodFormatter
{
    string Format(string text);
}

public interface IGoodValidator
{
    bool IsValid(string input);
    string GetErrorMessage();
}

public interface IGoodDescribable
{
    string Describe();
}

// Cliente implementa APENAS o que precisa
public class GoodUpperFormatter : IGoodFormatter
{
    public string Format(string text) => text.ToUpper();
    // Pronto! Sem overhead.
}

// Cliente que QUER validar, implementa ambas
public class ValidatingFormatter : IGoodFormatter, IGoodValidator
{
    public string Format(string text)
    {
        if (!IsValid(text))
            throw new ArgumentException(GetErrorMessage());
        return text.ToUpper();
    }

    public bool IsValid(string input) => !string.IsNullOrEmpty(input);
    public string GetErrorMessage() => "Input não pode ser vazio";
}

// ============================================================================
// CHEIRO 2: COMPOSIÇÃO ESPALHADA (Antes - Fase 3 estilo)
// ============================================================================

// ❌ CHEIRO: Switch em múltiplos clientes
public class BadFormatterClient1
{
    public string Format(string text, string mode)
    {
        // Switch duplicado...
        var formatter = mode switch
        {
            "UPPER" => (IGoodFormatter)new GoodUpperFormatter(),
            "LOWER" => new LowerFormatter(),
            "TITLE" => new TitleFormatter(),
            _ => new PassthroughFormatter()
        };
        return formatter.Format(text);
    }
}

public class BadFormatterClient2
{
    public string Transform(string text, string mode)
    {
        // ...Switch novamente duplicado!
        var formatter = mode switch
        {
            "UPPER" => (IGoodFormatter)new GoodUpperFormatter(),
            "LOWER" => new LowerFormatter(),
            "TITLE" => new TitleFormatter(),
            _ => new PassthroughFormatter()
        };
        return formatter.Format(text);
    }
}

// Problema: Adicionar novo modo = editar TODOS os clientes

// ✅ ANTÍDOTO: Factory Pattern - Decisão centralizada
public class FormatterFactory
{
    public static IGoodFormatter CreateFormatter(string mode)
    {
        return mode.ToUpperInvariant() switch
        {
            "UPPER" => new GoodUpperFormatter(),
            "LOWER" => new LowerFormatter(),
            "TITLE" => new TitleFormatter(),
            "REVERSE" => new ReverseFormatter(),    // ← Novo modo adicionado AQUI
            _ => new PassthroughFormatter()
        };
    }
}

// Clientes agora simples e consistentes
public class GoodFormatterClient1
{
    public string Format(string text, string mode)
    {
        var formatter = FormatterFactory.CreateFormatter(mode);
        return formatter.Format(text);
    }
}

public class GoodFormatterClient2
{
    public string Transform(string text, string mode)
    {
        var formatter = FormatterFactory.CreateFormatter(mode);
        return formatter.Format(text);
    }
}

// Novo cliente não precisa conhecer detalhes
public class GoodFormatterClient3
{
    public string Process(string text, string mode)
    {
        var formatter = FormatterFactory.CreateFormatter(mode);
        return formatter.Format(text);
    }
}

// ============================================================================
// IMPLEMENTAÇÕES CONCRETAS (usadas nos exemplos)
// ============================================================================

public class LowerFormatter : IGoodFormatter
{
    public string Format(string text) => text.ToLower();
}

public class TitleFormatter : IGoodFormatter
{
    public string Format(string text)
    {
        if (string.IsNullOrEmpty(text)) return text;
        var words = text.Split(' ');
        for (int i = 0; i < words.Length; i++)
        {
            if (words[i].Length > 0)
                words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower();
        }
        return string.Join(" ", words);
    }
}

public class ReverseFormatter : IGoodFormatter
{
    public string Format(string text)
    {
        var chars = text.ToCharArray();
        System.Array.Reverse(chars);
        return new string(chars);
    }
}

public class PassthroughFormatter : IGoodFormatter
{
    public string Format(string text) => text;
}

// ============================================================================
// DEMONSTRAÇÃO: Mostrando o impacto das refatorações
// ============================================================================

public class RefactoringDemo
{
    public static void Main()
    {
        Console.WriteLine("=== Fase 11 — Cheiros e Antídotos ===\n");

        // Demonstração do Cheiro 1: God Interface
        Console.WriteLine("## CHEIRO 1: God Interface\n");
        
        Console.WriteLine("❌ ANTES (IBadFormatter):");
        var badFormatter = new BadUpperFormatter();
        Console.WriteLine($"   Format('hello'): {badFormatter.Format("hello")}");
        Console.WriteLine("   ⚠️  BadUpperFormatter implementa 4 métodos, usa 1\n");

        Console.WriteLine("✅ DEPOIS (IGoodFormatter segregada):");
        var goodFormatter = new GoodUpperFormatter();
        Console.WriteLine($"   Format('hello'): {goodFormatter.Format("hello")}");
        Console.WriteLine("   ✓  GoodUpperFormatter implementa 1 método, bem focado\n");

        Console.WriteLine("✅ COM VALIDAÇÃO (composição de interfaces):");
        var validatingFormatter = new ValidatingFormatter();
        Console.WriteLine($"   Format('hello'): {validatingFormatter.Format("hello")}");
        Console.WriteLine("   IsValid(''): {0}", validatingFormatter.IsValid(""));
        Console.WriteLine("   ✓  ValidatingFormatter escolhe implementar IValidator também\n");

        // Demonstração do Cheiro 2: Composição Espalhada
        Console.WriteLine("## CHEIRO 2: Composição Espalhada\n");

        Console.WriteLine("❌ ANTES (Switch em múltiplos clientes):");
        var badClient1 = new BadFormatterClient1();
        var badClient2 = new BadFormatterClient2();
        Console.WriteLine($"   Client1.Format('hello', 'UPPER'): {badClient1.Format("hello", "UPPER")}");
        Console.WriteLine($"   Client2.Transform('hello', 'UPPER'): {badClient2.Transform("hello", "UPPER")}");
        Console.WriteLine("   ⚠️  Switch duplicado! Adicionar modo quebra os 2 clientes\n");

        Console.WriteLine("✅ DEPOIS (Factory centralizada):");
        var goodClient1 = new GoodFormatterClient1();
        var goodClient2 = new GoodFormatterClient2();
        var goodClient3 = new GoodFormatterClient3();
        Console.WriteLine($"   Client1.Format('hello', 'UPPER'): {goodClient1.Format("hello", "UPPER")}");
        Console.WriteLine($"   Client2.Transform('hello', 'LOWER'): {goodClient2.Transform("hello", "LOWER")}");
        Console.WriteLine($"   Client3.Process('hello', 'REVERSE'): {goodClient3.Process("hello", "REVERSE")}");
        Console.WriteLine("   ✓  Factory centralizada! Adicionar modo: edita Factory, clientes intactos\n");

        Console.WriteLine("✓ Demonstração de refatoração bem-sucedida");
        Console.WriteLine("  - ISP: God Interface → Interfaces segregadas");
        Console.WriteLine("  - Factory: Decisão espalhada → Centralizada");
    }
}
