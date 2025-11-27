using System;
using System.Globalization;

namespace Fase02Procedural
{
    /// <summary>
    /// FASE 2 — Procedural Mínimo (Formatação de Texto)
    /// 
    /// Demonstra a abordagem procedural pura com if/switch
    /// espalhados em um único método.
    /// 
    /// Objetivo: Formatar texto conforme modo escolhido (UPPER, LOWER, TITLE, DEFAULT).
    /// 
    /// Problema: Cada novo modo exige novo if; manutenção fica complexa com crescimento.
    /// </summary>
    public class TextFormatterProcedural
    {
        /// <summary>
        /// Formata texto conforme o modo especificado.
        /// Abordagem puramente procedural com if/else.
        /// </summary>
        /// <param name="text">Texto a ser formatado</param>
        /// <param name="mode">Modo de formatação: UPPER, LOWER, TITLE, ou DEFAULT</param>
        /// <returns>Texto formatado</returns>
        public string FormatarTexto(string text, string mode)
        {
            // Validação: entrada vazia
            if (string.IsNullOrWhiteSpace(text))
            {
                Console.WriteLine("⚠️  Texto vazio; retornando vazio");
                return "";
            }

            // Normaliza modo: uppercase e remove espaços
            string normalizedMode = mode?.ToUpper().Trim() ?? "DEFAULT";

            // ===== RAMIFICAÇÕES (if/switch) =====
            // Cada novo modo adiciona um novo else if aqui

            if (normalizedMode == "UPPER")
            {
                // Modo 1: Maiúsculas
                Console.WriteLine($"[UPPER] Aplicando maiúsculas: '{text}' → '{text.ToUpper()}'");
                return text.ToUpper();
            }
            else if (normalizedMode == "LOWER")
            {
                // Modo 2: Minúsculas
                Console.WriteLine($"[LOWER] Aplicando minúsculas: '{text}' → '{text.ToLower()}'");
                return text.ToLower();
            }
            else if (normalizedMode == "TITLE")
            {
                // Modo 3: Título (primeira letra de cada palavra maiúscula)
                string titleCased = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text);
                Console.WriteLine($"[TITLE] Aplicando título: '{text}' → '{titleCased}'");
                return titleCased;
            }
            else
            {
                // Modo padrão: Primeira letra maiúscula, resto como está
                string defaultFormatted = char.ToUpper(text[0]) + text.Substring(1);
                Console.WriteLine($"[DEFAULT] Aplicando padrão (modo '{mode}' não reconhecido): '{text}' → '{defaultFormatted}'");
                return defaultFormatted;
            }
        }

        /// <summary>
        /// Cenários de teste documentados no mapa (fase-02-mapa.md).
        /// Este método executa os 5 cenários de teste/fronteira.
        /// </summary>
        public void ExecutarCenariosTeste()
        {
            Console.WriteLine("\n=== FASE 2: Procedural (5 Cenários de Teste) ===\n");

            // 1️⃣ Entrada Mínima (string vazia)
            Console.WriteLine("1️⃣  Entrada Mínima (string vazia):");
            var resultado1 = FormatarTexto("", "UPPER");
            Console.WriteLine($"   Resultado: '{resultado1}' (esperado: '')\n");

            // 2️⃣ Entrada Máxima (texto grande)
            Console.WriteLine("2️⃣  Entrada Máxima (texto com 1000 caracteres):");
            string textoGrande = new string('a', 1000);
            var resultado2 = FormatarTexto(textoGrande, "TITLE");
            Console.WriteLine($"   Resultado: primeiros 50 caracteres = '{resultado2.Substring(0, 50)}...'");
            Console.WriteLine($"   Tamanho: {resultado2.Length} caracteres (esperado: 1000)\n");

            // 3️⃣ Modo Inválido (cai no padrão)
            Console.WriteLine("3️⃣  Modo Inválido (cai no padrão):");
            var resultado3 = FormatarTexto("hello", "GIRAR");
            Console.WriteLine($"   Resultado: '{resultado3}' (esperado: 'Hello' - DEFAULT)\n");

            // 4️⃣ Combinação Ambígua (texto com 1 letra)
            Console.WriteLine("4️⃣  Combinação Ambígua (1 letra: 'a'):");
            var resultado4a = FormatarTexto("a", "DEFAULT");
            var resultado4b = FormatarTexto("a", "UPPER");
            Console.WriteLine($"   DEFAULT: '{resultado4a}' (esperado: 'A')");
            Console.WriteLine($"   UPPER:   '{resultado4b}' (esperado: 'A')");
            Console.WriteLine($"   Nota: Ambas produzem 'A' para entrada única; para 2+ letras, diferem:\n");
            var resultado4c = FormatarTexto("ab", "DEFAULT");
            var resultado4d = FormatarTexto("ab", "UPPER");
            Console.WriteLine($"   DEFAULT (2 letras): '{resultado4c}' (esperado: 'Ab')");
            Console.WriteLine($"   UPPER   (2 letras): '{resultado4d}' (esperado: 'AB')\n");

            // 5️⃣ Caso Comum Representativo (nome completo com acentuação)
            Console.WriteLine("5️⃣  Caso Comum Representativo (nome com acentuação):");
            var resultado5 = FormatarTexto("joão da silva", "TITLE");
            Console.WriteLine($"   Resultado: '{resultado5}' (esperado: 'João Da Silva')\n");
        }
    }

    /// <summary>
    /// Problema de Escalabilidade: Adicionando novo modo (REVERSE)
    /// 
    /// Observe como adicionar um novo modo exige:
    /// 1. Entender a lógica procedural existente
    /// 2. Adicionar novo else if
    /// 3. Revisar toda a função
    /// 4. Aumentar testes exponencialmente
    /// </summary>
    public class TextFormatterWithReverse
    {
        public string FormatarTextoComReverse(string text, string mode)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "";

            string normalizedMode = mode?.ToUpper().Trim() ?? "DEFAULT";

            if (normalizedMode == "UPPER")
            {
                return text.ToUpper();
            }
            else if (normalizedMode == "LOWER")
            {
                return text.ToLower();
            }
            else if (normalizedMode == "TITLE")
            {
                return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text);
            }
            else if (normalizedMode == "REVERSE")
            {
                // ⚠️ NOVO MODO: exige modificação da função existente
                char[] chars = text.ToCharArray();
                Array.Reverse(chars);
                string reversed = new string(chars);
                Console.WriteLine($"[REVERSE] Invertendo: '{text}' → '{reversed}'");
                return reversed;
            }
            else
            {
                return char.ToUpper(text[0]) + text.Substring(1);
            }
        }
    }

    /// <summary>
    /// Demonstração de uso e problemas escalabilidade.
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            var formatter = new TextFormatterProcedural();

            // Executa os 5 cenários de teste
            formatter.ExecutarCenariosTeste();

            // Análise de escalabilidade
            Console.WriteLine("\n=== ⚠️  POR QUE NÃO ESCALA ===\n");
            Console.WriteLine("1. Multiplicação de if/switch: 4 modos → 10 modos = mais ramificações");
            Console.WriteLine("2. Duplicação de lógica: validação repetida em múltiplos métodos");
            Console.WriteLine("3. Testes combinatórios: 4 modos × 3 tamanhos = 12 cenários; com 10 modos = 30+");
            Console.WriteLine("4. Dificuldade de extensão: novo modo exige entender toda a função");
            Console.WriteLine("5. Acoplamento ao 'como': função conhece detalhes de CADA modo");
            Console.WriteLine("6. Manutenção: método fica muito grande (15+ linhas) e difícil de ler\n");

            // Demonstração do problema
            Console.WriteLine("=== Demonstração: Adicionando modo REVERSE ===\n");
            var formatterWithReverse = new TextFormatterWithReverse();
            formatterWithReverse.FormatarTextoComReverse("hello", "REVERSE");
            Console.WriteLine("⚠️  Observe: Precisou-se de uma NOVA CLASSE (ou renovar método inteiro)\n");

            Console.WriteLine("=== Próxima Fase (3): Polimorfismo/OO sem Interface ===");
            Console.WriteLine("Solução: Quebrar em estratégias (classes), permitir extensão SEM alterar código existente.");
        }
    }
}
