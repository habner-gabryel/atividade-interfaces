using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Fase03OOSemInterface
{
    /// <summary>
    /// FASE 3 — OO sem Interface (Herança + Polimorfismo)
    /// 
    /// Transformação da Fase 2:
    /// - Removemos os if/switch do fluxo central
    /// - Delegamos decisões às subclasses via polimorfismo (override)
    /// - Cliente ainda conhece concretos (será endereçado na Fase 4)
    /// </summary>
    /// 
    /// <summary>
    /// CLASSE BASE ABSTRATA
    /// Define o ritual comum "Format" e delega o passo variável "Apply" para subclasses.
    /// Template Method Pattern.
    /// </summary>
    public abstract class TextFormatterBase
    {
        /// <summary>
        /// Ritual comum: prepara e aplica formatação.
        /// (No futuro: poderia incluir normalização, validação comum)
        /// </summary>
        public string Format(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                Console.WriteLine($"[{GetType().Name}] Texto vazio; retornando vazio");
                return "";
            }

            // Passo variável delegado à subclasse
            return Apply(text);
        }

        /// <summary>
        /// Gancho variável: cada subclasse define como formatar.
        /// Este é o passo que muda conforme a variação.
        /// </summary>
        protected abstract string Apply(string text);

        /// <summary>
        /// Identificação do formatter (útil para logs/debug)
        /// </summary>
        public abstract string GetFormatterName();
    }

    /// <summary>
    /// SUBCLASSE CONCRETA 1: Maiúsculas
    /// Responsabilidade única: converter para UPPER
    /// </summary>
    public sealed class UpperCaseFormatter : TextFormatterBase
    {
        protected override string Apply(string text)
        {
            string result = text?.ToUpperInvariant() ?? "";
            Console.WriteLine($"[UPPER] Aplicando maiúsculas: '{text}' → '{result}'");
            return result;
        }

        public override string GetFormatterName() => "UPPER";
    }

    /// <summary>
    /// SUBCLASSE CONCRETA 2: Minúsculas
    /// Responsabilidade única: converter para LOWER
    /// </summary>
    public sealed class LowerCaseFormatter : TextFormatterBase
    {
        protected override string Apply(string text)
        {
            string result = text?.ToLowerInvariant() ?? "";
            Console.WriteLine($"[LOWER] Aplicando minúsculas: '{text}' → '{result}'");
            return result;
        }

        public override string GetFormatterName() => "LOWER";
    }

    /// <summary>
    /// SUBCLASSE CONCRETA 3: Título (PascalCase/Title Case)
    /// Responsabilidade única: capitalizar primeira letra de cada palavra
    /// </summary>
    public sealed class TitleCaseFormatter : TextFormatterBase
    {
        protected override string Apply(string text)
        {
            // Usa Regex para Title Case (mais robusto que TextInfo)
            string result = Regex.Replace(
                text ?? string.Empty,
                @"\b(\p{L})",
                m => m.Value.ToUpperInvariant()
            );
            Console.WriteLine($"[TITLE] Aplicando título: '{text}' → '{result}'");
            return result;
        }

        public override string GetFormatterName() => "TITLE";
    }

    /// <summary>
    /// SUBCLASSE CONCRETA 4: Padrão (primeira letra maiúscula, resto mantém)
    /// Responsabilidade única: aplicar padrão DEFAULT
    /// </summary>
    public sealed class DefaultFormatter : TextFormatterBase
    {
        protected override string Apply(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            string result = char.ToUpper(text[0]) + text.Substring(1);
            Console.WriteLine($"[DEFAULT] Aplicando padrão: '{text}' → '{result}'");
            return result;
        }

        public override string GetFormatterName() => "DEFAULT";
    }

    /// <summary>
    /// SUBCLASSE CONCRETA 5: Passthrough (mantém original)
    /// Responsabilidade única: não fazer nada (padrão para modo desconhecido)
    /// </summary>
    public sealed class PassthroughFormatter : TextFormatterBase
    {
        protected override string Apply(string text)
        {
            Console.WriteLine($"[PASSTHROUGH] Mantendo original: '{text}'");
            return text;
        }

        public override string GetFormatterName() => "PASSTHROUGH";
    }

    /// <summary>
    /// CLIENTE: Ainda conhece concretos (será melhorado na Fase 4)
    /// ⚠️ O switch aqui é APENAS para compor a concreta inicial
    /// O if/switch foi REMOVIDO do fluxo de formatação
    /// </summary>
    public static class FormatterClient
    {
        /// <summary>
        /// Renderiza texto conforme modo.
        /// 
        /// ✅ Melhor: switch está fora do fluxo central; cada concreta tem sua lógica
        /// ⚠️ Ainda rígido: cliente conhece todas as concretas
        /// </summary>
        public static string Render(string text, string mode)
        {
            Console.WriteLine($"\n[CLIENT] Renderizando em modo '{mode}'\n");

            // ⚠️ Switch aqui é APENAS para escolher qual concreta usar
            // (Não há mais lógica de formatação dentro do switch)
            TextFormatterBase formatter = mode?.ToUpper() switch
            {
                "UPPER" => new UpperCaseFormatter(),
                "LOWER" => new LowerCaseFormatter(),
                "TITLE" => new TitleCaseFormatter(),
                "DEFAULT" => new DefaultFormatter(),
                _ => new PassthroughFormatter()
            };

            // ✅ Fluxo é agora simples e polimórfico
            // Não há if/else aqui; o "como" é delegado à concreta
            string result = formatter.Format(text);
            Console.WriteLine($"[CLIENT] Resultado: '{result}'\n");
            return result;
        }

        /// <summary>
        /// Versão melhorada: demonstra como Fase 4 resolveria a composição
        /// (usando Factory ou DI - endereçado na próxima fase)
        /// </summary>
        public static string RenderWithExplicitFormatter(string text, TextFormatterBase formatter)
        {
            Console.WriteLine($"\n[CLIENT] Usando formatter: {formatter.GetFormatterName()}\n");
            string result = formatter.Format(text);
            Console.WriteLine($"[CLIENT] Resultado: '{result}'\n");
            return result;
        }
    }

    /// <summary>
    /// Execução de testes e demonstração da evolução
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("=== FASE 3: OO sem Interface (Herança + Polimorfismo) ===\n");

            // Teste 1: Casos reais
            Console.WriteLine("--- Teste 1: Casos Reais ---");
            FormatterClient.Render("joão da silva", "TITLE");
            FormatterClient.Render("HELLO WORLD", "LOWER");
            FormatterClient.Render("hello world", "UPPER");
            FormatterClient.Render("hello", "DEFAULT");
            FormatterClient.Render("test", "UNKNOWN");

            // Teste 2: Entrada vazia
            Console.WriteLine("\n--- Teste 2: Entrada Vazia ---");
            FormatterClient.Render("", "UPPER");

            // Teste 3: Demonstração com instância explícita (prepara Fase 4)
            Console.WriteLine("\n--- Teste 3: Instância Explícita (Fase 4 Preview) ---");
            TextFormatterBase formatter1 = new UpperCaseFormatter();
            FormatterClient.RenderWithExplicitFormatter("hello world", formatter1);

            TextFormatterBase formatter2 = new TitleCaseFormatter();
            FormatterClient.RenderWithExplicitFormatter("hello world", formatter2);

            // Teste 4: Composição em array (polimorfismo em ação)
            Console.WriteLine("\n--- Teste 4: Polimorfismo em Array ---");
            var formatters = new TextFormatterBase[]
            {
                new UpperCaseFormatter(),
                new LowerCaseFormatter(),
                new TitleCaseFormatter(),
                new DefaultFormatter()
            };

            string textToFormat = "hello world";
            Console.WriteLine($"Aplicando todos os formatadores a '{textToFormat}':\n");
            foreach (var fmt in formatters)
            {
                string result = fmt.Format(textToFormat);
                Console.WriteLine($"→ {fmt.GetFormatterName()}: '{result}'\n");
            }

            // Análise
            Console.WriteLine("\n=== Análise: Melhorou vs. Ainda Rígido ===\n");
            PrintAnalysis();
        }

        private static void PrintAnalysis()
        {
            Console.WriteLine("✅ MELHOROU:");
            Console.WriteLine("  • Remoção de if/switch no fluxo central → leitura mais clara");
            Console.WriteLine("  • Coesão por variação: cada concreta é simples e focada");
            Console.WriteLine("  • Testes de cada variação ficam pequenos e isolados");
            Console.WriteLine("  • Novo modo pode ser criado sem alterar fluxo existente\n");

            Console.WriteLine("⚠️  AINDA FICOU RÍGIDO:");
            Console.WriteLine("  • Cliente AINDA conhece todas as concretas (trocar = mexer no código)");
            Console.WriteLine("  • Composição dispersa: switch de seleção ainda está no cliente");
            Console.WriteLine("  • Sem contrato formal (interface) → difícil fazer testes com dublês");
            Console.WriteLine("  • Adicionar novo modo exige mudança no switch do cliente\n");

            Console.WriteLine("🎯 PRÓXIMA FASE (4):");
            Console.WriteLine("  • Introduzir interface (contrato estável)");
            Console.WriteLine("  • Extrair composição para Factory ou Dependency Injection");
            Console.WriteLine("  • Cliente dependerá da interface, não dos concretos");
        }
    }
}
