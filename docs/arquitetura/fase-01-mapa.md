# Fase 1 — Heurística antes do código (Mapa Mental)

## Problema escolhido

Queremos permitir que o cliente pague a compra escolhendo automaticamente o meio adequado (Pix ou Cartão) conforme regras simples de valor/risco.

---

## Quadro 1 — Procedural (onde surgem if/switch)

### Fluxo
```
recebe valor → if (valor > 500) 
              ├─ SIM  → "Cartão" 
              └─ NÃO  → "Pix" 
            → processa → retorna sucesso/erro
```

### Características
- **Decisões embutidas** no fluxo (ramificações por valor e, eventualmente, disponibilidade do banco)
- **Lógica linear** mas rígida: o fluxo principal conhece todos os detalhes de cada meio
- **Sinais de dor**: cada novo meio (Boleto, Crediário) adiciona novos `if/switch`, multiplicando casos e testes
- **Difícil de testar**: sem forma de simular falhas isoladas ou trocar estratégias sem alterar o código

### Exemplo de código
```csharp
public bool ProcessarPagamento(decimal valor)
{
    if (valor > 500)
    {
        // Lógica específica do Cartão
        var numero = "4111111111111111";
        var cvv = "123";
        var resultado = ProcessarCartao(numero, cvv, valor);
        return resultado;
    }
    else
    {
        // Lógica específica do Pix
        var chave = "conta@banco.com";
        var resultado = ProcessarPix(chave, valor);
        return resultado;
    }
}
```

---

## Quadro 2 — OO sem interface (quem encapsula o quê; o que ainda fica rígido)

### Encapsulamento
- Lógica de cada meio em classes concretas: `PixProcessor`, `CardProcessor`
- Um "serviço" de pagamento orquestra o fluxo

### Melhoras
- ✅ Coesão por meio (cada classe cuida de uma responsabilidade)
- ✅ Menos duplicação (lógica de Pix centralizada em `PixProcessor`)

### Rigidez remanescente
- **Cliente/orquestrador continua decidindo** "qual classe usar"
- **Ainda contém o `if` de escolha** (seleção por valor)
- ⚠️ Troca de meio altera o cliente
- ⚠️ Difícil estender sem mexer na orquestração

### Exemplo de código
```csharp
public class PaymentService
{
    public bool ProcessarPagamento(decimal valor)
    {
        if (valor > 500)
        {
            var processor = new CardProcessor();
            return processor.Processar("4111111111111111", "123", valor);
        }
        else
        {
            var processor = new PixProcessor();
            return processor.Processar("conta@banco.com", valor);
        }
    }
}

public class PixProcessor
{
    public bool Processar(string chave, decimal valor)
    {
        // Lógica específica do Pix
        return true;
    }
}

public class CardProcessor
{
    public bool Processar(string numero, string cvv, decimal valor)
    {
        // Lógica específica do Cartão
        return true;
    }
}
```

**Problema**: O `if` que decide qual usar **ainda está no cliente**.

---

## Quadro 3 — Com interface (contrato que permite alternar + ponto de composição)

### Contrato
```csharp
public interface IPaymentProcessor
{
    bool Processar(decimal valor);
    string GetMeio();
}
```

### Implementações
```csharp
public class PixProcessor : IPaymentProcessor
{
    public bool Processar(decimal valor)
    {
        var chave = "conta@banco.com";
        // Lógica específica do Pix
        return true;
    }
    
    public string GetMeio() => "Pix";
}

public class CardProcessor : IPaymentProcessor
{
    public bool Processar(decimal valor)
    {
        var numero = "4111111111111111";
        var cvv = "123";
        // Lógica específica do Cartão
        return true;
    }
    
    public string GetMeio() => "Cartão";
}

// Futuras implementações (sem alterar clientes):
// - BoletoProcessor
// - WalletProcessor
// - CreditLineProcessor
```

### Ponto de Composição
A **política é configurável fora do cliente**:

```csharp
public class PaymentPolicySelector
{
    public IPaymentProcessor SelecionarMeio(decimal valor)
    {
        if (valor > 500)
            return new CardProcessor();
        else
            return new PixProcessor();
    }
}

public class PaymentService
{
    private readonly PaymentPolicySelector _selector;
    
    public PaymentService(PaymentPolicySelector selector)
    {
        _selector = selector;
    }
    
    public bool ProcessarPagamento(decimal valor)
    {
        var processor = _selector.SelecionarMeio(valor);
        return processor.Processar(valor);
    }
}
```

### Efeito
- ✅ **Cliente não muda** quando alternamos implementações ou política
- ✅ **Testes simples**: usar dublês (mocks) para `IPaymentProcessor`
- ✅ **Fácil estender**: novas estratégias de seleção sem tocar no `PaymentService`
- ✅ **Desacoplamento**: cliente depende da abstração, não do concreto

---

## 3 Sinais de Alerta Previstos

| Sinal | Descrição | Exemplo |
|-------|-----------|---------|
| **1. Cliente muda ao trocar implementação** | Cheiro de acoplamento ao "como"; cliente não deveria conhecer detalhes de cada meio | Se trocar `CardProcessor` por `ApplePayProcessor`, o `if/switch` precisa mudar |
| **2. Ramificações espalhadas** | `if/switch` por tipo/valor aparece em vários pontos do código | Lógica de seleção duplicada em `PaymentService`, `CheckoutService`, `RefundService` |
| **3. Testes lentos e frágeis** | Sem dublês, batendo em integrações reais; difícil simular falhas ou controlar cenários | Teste de `ProcessarPagamento` precisa de cartão real e gateway de pagamento rodando |

---

## Resumo: Evolução Esperada

```
Procedural (Quadro 1)
    ↓ Adiciona Classes
    
OO sem Interface (Quadro 2)
    ↓ Adiciona Contrato
    
Com Interface (Quadro 3) ← OBJETIVO
```

**Benefício progressivo**: Cada nível reduz acoplamento e aumenta flexibilidade.

---

## Onde testar

- Código de exemplo em `src/fase-01-procedural/`
- Este mapa em `docs/arquitetura/fase-01-mapa.md`
- Ver também `README.md` para contexto da atividade
