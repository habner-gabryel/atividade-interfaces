using System;

namespace Fase01Procedural.Quadro1
{
    /// <summary>
    /// QUADRO 1 — Procedural (onde surgem if/switch)
    /// 
    /// Fluxo: recebe valor → if (valor > 500) então "Cartão" senão "Pix" → processa → retorna sucesso/erro.
    /// 
    /// Problemas:
    /// - Decisões embutidas no fluxo
    /// - Cada novo meio (Boleto, Crediário) adiciona novos if/switch
    /// - Multiplicam casos de teste
    /// - Difícil testar em isolamento
    /// </summary>
    public class PaymentServiceProcedural
    {
        /// <summary>
        /// Processa pagamento de forma procedural.
        /// Todo o conhecimento (Pix, Cartão) está encapsulado aqui.
        /// </summary>
        public bool ProcessarPagamento(decimal valor)
        {
            if (valor > 500)
            {
                // Lógica específica do Cartão
                Console.WriteLine($"[CARTÃO] Processando pagamento de R$ {valor}");
                
                string numero = "4111111111111111";
                string cvv = "123";
                string validade = "12/25";
                
                bool autorizacao = ValidarCartao(numero, cvv, validade);
                if (autorizacao)
                {
                    Console.WriteLine($"[CARTÃO] ✓ Pagamento de R$ {valor} autorizado");
                    return true;
                }
                else
                {
                    Console.WriteLine($"[CARTÃO] ✗ Pagamento de R$ {valor} recusado");
                    return false;
                }
            }
            else
            {
                // Lógica específica do Pix
                Console.WriteLine($"[PIX] Processando pagamento de R$ {valor}");
                
                string chave = "conta@banco.com";
                
                bool confirmacao = ValidarPix(chave, valor);
                if (confirmacao)
                {
                    Console.WriteLine($"[PIX] ✓ Pagamento de R$ {valor} confirmado");
                    return true;
                }
                else
                {
                    Console.WriteLine($"[PIX] ✗ Pagamento de R$ {valor} falhou");
                    return false;
                }
            }
        }
        
        private bool ValidarCartao(string numero, string cvv, string validade)
        {
            // Validação simulada
            return !string.IsNullOrEmpty(numero) && numero.Length == 16;
        }
        
        private bool ValidarPix(string chave, decimal valor)
        {
            // Validação simulada
            return !string.IsNullOrEmpty(chave) && valor > 0;
        }
    }
    
    /// <summary>
    /// Exemplo de uso do serviço procedural.
    /// </summary>
    public class ProgramQuadro1
    {
        public static void Main(string[] args)
        {
            var service = new PaymentServiceProcedural();
            
            Console.WriteLine("=== QUADRO 1: Procedural ===\n");
            
            service.ProcessarPagamento(700);  // > 500: Cartão
            Console.WriteLine();
            
            service.ProcessarPagamento(300);  // < 500: Pix
            Console.WriteLine();
        }
    }
}
