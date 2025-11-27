using System;

namespace Fase01Procedural.Quadro3
{
    /// <summary>
    /// QUADRO 3 — Com Interface (contrato que permite alternar + ponto de composição)
    /// 
    /// Contrato: "processar pagamento"
    /// Implementações: PixProcessor, CardProcessor (e futuras: BoletoProcessor, WalletProcessor)
    /// Ponto de composição: política configurada FORA do cliente
    /// 
    /// Efeito:
    /// - Cliente NÃO muda quando alternamos implementações
    /// - Testes ficam simples (dublês/mocks)
    /// - Fácil adicionar novos meios
    /// </summary>
    
    /// <summary>
    /// CONTRATO: O que qualquer processador de pagamento deve fazer.
    /// </summary>
    public interface IPaymentProcessor
    {
        bool Processar(decimal valor);
        string GetMeio();
    }
    
    /// <summary>
    /// Implementação concreta: Pix
    /// </summary>
    public class PixProcessor : IPaymentProcessor
    {
        private readonly string _chave;
        
        public PixProcessor(string chave = "conta@banco.com")
        {
            _chave = chave;
        }
        
        public bool Processar(decimal valor)
        {
            Console.WriteLine($"[PIX] Processando pagamento de R$ {valor}");
            Console.WriteLine($"[PIX] Validando chave: {_chave}");
            
            if (!string.IsNullOrEmpty(_chave) && valor > 0)
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
        
        public string GetMeio() => "Pix";
    }
    
    /// <summary>
    /// Implementação concreta: Cartão
    /// </summary>
    public class CardProcessor : IPaymentProcessor
    {
        private readonly string _numero;
        private readonly string _cvv;
        
        public CardProcessor(string numero = "4111111111111111", string cvv = "123")
        {
            _numero = numero;
            _cvv = cvv;
        }
        
        public bool Processar(decimal valor)
        {
            Console.WriteLine($"[CARTÃO] Processando pagamento de R$ {valor}");
            Console.WriteLine($"[CARTÃO] Número: {_numero.Substring(0, 4)}...{_numero.Substring(_numero.Length - 4)}");
            
            if (!string.IsNullOrEmpty(_numero) && _numero.Length == 16 
                && !string.IsNullOrEmpty(_cvv) && valor > 0)
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
        
        public string GetMeio() => "Cartão";
    }
    
    /// <summary>
    /// (Futuro) Implementação concreta: Boleto
    /// Graças à interface, não precisa alterar PaymentService para adicionar isso!
    /// </summary>
    public class BoletoProcessor : IPaymentProcessor
    {
        private readonly string _cnpj;
        
        public BoletoProcessor(string cnpj = "12345678901234")
        {
            _cnpj = cnpj;
        }
        
        public bool Processar(decimal valor)
        {
            Console.WriteLine($"[BOLETO] Processando pagamento de R$ {valor}");
            Console.WriteLine($"[BOLETO] CNPJ: {_cnpj}");
            Console.WriteLine($"[BOLETO] ✓ Boleto gerado, prazo: 3 dias úteis");
            return true;
        }
        
        public string GetMeio() => "Boleto";
    }
    
    /// <summary>
    /// POLÍTICA DE SELEÇÃO: Define qual processador usar para cada valor.
    /// Essa política é facilmente alterável sem tocar no PaymentService.
    /// </summary>
    public class PaymentPolicySelector
    {
        public IPaymentProcessor SelecionarMeio(decimal valor)
        {
            // Política: valores > 500 usam Cartão; valores <= 500 usam Pix
            if (valor > 500)
            {
                return new CardProcessor();
            }
            else if (valor < 50)
            {
                return new PixProcessor();
            }
            else
            {
                // Exemplo: valores médios poderiam usar Boleto
                return new PixProcessor();
            }
        }
    }
    
    /// <summary>
    /// Serviço de pagamento que DEPENDE DA ABSTRAÇÃO (IPaymentProcessor).
    /// Não conhece detalhes de implementação.
    /// Não muda quando adicionamos novos processadores.
    /// </summary>
    public class PaymentService
    {
        private readonly PaymentPolicySelector _selector;
        
        public PaymentService(PaymentPolicySelector selector)
        {
            _selector = selector;
        }
        
        public bool ProcessarPagamento(decimal valor)
        {
            // Delega a seleção para a política
            var processor = _selector.SelecionarMeio(valor);
            
            Console.WriteLine($"[SERVIÇO] Selecionado: {processor.GetMeio()}");
            
            // Processa usando a abstração (não precisa saber qual é)
            return processor.Processar(valor);
        }
    }
    
    /// <summary>
    /// Exemplo com mock (dublê) para testes unitários.
    /// </summary>
    public class MockPaymentProcessor : IPaymentProcessor
    {
        private readonly bool _resultado;
        
        public MockPaymentProcessor(bool resultado = true)
        {
            _resultado = resultado;
        }
        
        public bool Processar(decimal valor)
        {
            Console.WriteLine($"[MOCK] Simulando pagamento de R$ {valor}");
            return _resultado;
        }
        
        public string GetMeio() => "Mock";
    }
    
    /// <summary>
    /// Exemplo de uso do serviço com interface.
    /// </summary>
    public class ProgramQuadro3
    {
        public static void Main(string[] args)
        {
            var selector = new PaymentPolicySelector();
            var service = new PaymentService(selector);
            
            Console.WriteLine("=== QUADRO 3: Com Interface ===\n");
            
            // Pagamento alto: usa Cartão (automaticamente selecionado pela política)
            Console.WriteLine("--- Pagamento Alto ---");
            service.ProcessarPagamento(700);
            Console.WriteLine();
            
            // Pagamento baixo: usa Pix (automaticamente selecionado pela política)
            Console.WriteLine("--- Pagamento Baixo ---");
            service.ProcessarPagamento(300);
            Console.WriteLine();
            
            // ===== BENEFÍCIOS =====
            Console.WriteLine("=== BENEFÍCIOS ===\n");
            
            Console.WriteLine("✅ Cliente (PaymentService) não mudou para adicionar BoletoProcessor");
            Console.WriteLine("✅ Testes podem usar MockPaymentProcessor sem tocar no banco real");
            Console.WriteLine("✅ Política é configurável (PaymentPolicySelector pode ser alterada)");
            Console.WriteLine("✅ Novos meios: basta criar nova classe que implemente IPaymentProcessor\n");
            
            // Demonstração: teste com mock
            Console.WriteLine("--- Teste com Mock ---");
            var mockProcessor = new MockPaymentProcessor(false);
            Console.WriteLine($"Mock retornou: {mockProcessor.Processar(100)}");
        }
    }
}
