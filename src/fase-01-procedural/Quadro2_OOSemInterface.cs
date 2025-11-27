using System;

namespace Fase01Procedural.Quadro2
{
    /// <summary>
    /// QUADRO 2 — OO sem Interface (quem encapsula o quê; o que ainda fica rígido)
    /// 
    /// Melhoras:
    /// - Lógica de cada meio em classes separadas
    /// - Coesão por meio
    /// - Menos duplicação
    /// 
    /// Rigidez remanescente:
    /// - Cliente/orquestrador AINDA contém o if de escolha
    /// - Troca de meio altera o cliente
    /// - Difícil estender
    /// </summary>
    
    /// <summary>
    /// Processa pagamento via Pix.
    /// </summary>
    public class PixProcessor
    {
        public bool Processar(string chave, decimal valor)
        {
            Console.WriteLine($"[PIX] Processando pagamento de R$ {valor}");
            Console.WriteLine($"[PIX] Validando chave: {chave}");
            
            if (ValidarChave(chave) && valor > 0)
            {
                Console.WriteLine($"[PIX] ✓ Pagamento de R$ {valor} confirmado");
                return true;
            }
            else
            {
                Console.WriteLine($"[PIX] ✗ Pagamento falhou");
                return false;
            }
        }
        
        private bool ValidarChave(string chave)
        {
            return !string.IsNullOrEmpty(chave);
        }
    }
    
    /// <summary>
    /// Processa pagamento via Cartão.
    /// </summary>
    public class CardProcessor
    {
        public bool Processar(string numero, string cvv, decimal valor)
        {
            Console.WriteLine($"[CARTÃO] Processando pagamento de R$ {valor}");
            Console.WriteLine($"[CARTÃO] Número: {numero.Substring(0, 4)}...{numero.Substring(numero.Length - 4)}");
            
            if (ValidarCartao(numero, cvv) && valor > 0)
            {
                Console.WriteLine($"[CARTÃO] ✓ Pagamento de R$ {valor} autorizado");
                return true;
            }
            else
            {
                Console.WriteLine($"[CARTÃO] ✗ Pagamento recusado");
                return false;
            }
        }
        
        private bool ValidarCartao(string numero, string cvv)
        {
            return !string.IsNullOrEmpty(numero) && numero.Length == 16 
                && !string.IsNullOrEmpty(cvv) && cvv.Length == 3;
        }
    }
    
    /// <summary>
    /// Serviço de pagamento que orquestra os processadores.
    /// ⚠️ PROBLEMA: Ainda contém a lógica de decisão (if por valor)
    /// </summary>
    public class PaymentService
    {
        public bool ProcessarPagamento(decimal valor)
        {
            if (valor > 500)
            {
                // Cliente AINDA conhece CardProcessor e decide usá-lo
                var processor = new CardProcessor();
                return processor.Processar("4111111111111111", "123", valor);
            }
            else
            {
                // Cliente AINDA conhece PixProcessor e decide usá-lo
                var processor = new PixProcessor();
                return processor.Processar("conta@banco.com", valor);
            }
        }
        
        // ⚠️ Se quisermos adicionar Boleto, temos que mexer aqui novamente
        // Cada novo meio exige alteração do cliente
    }
    
    /// <summary>
    /// Exemplo de uso do serviço OO (sem interface).
    /// </summary>
    public class ProgramQuadro2
    {
        public static void Main(string[] args)
        {
            var service = new PaymentService();
            
            Console.WriteLine("=== QUADRO 2: OO sem Interface ===\n");
            
            service.ProcessarPagamento(700);  // > 500: Cartão
            Console.WriteLine();
            
            service.ProcessarPagamento(300);  // < 500: Pix
            Console.WriteLine();
            
            Console.WriteLine("⚠️  Problema: O PaymentService ainda contém o if que decide qual classe usar.");
            Console.WriteLine("   Se trocarmos de estratégia ou adicionarmos novo meio, precisamos alterar PaymentService.");
        }
    }
}
