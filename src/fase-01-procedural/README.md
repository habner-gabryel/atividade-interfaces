# Fase 1 — Exemplos de Código C#

Este diretório contém exemplos práticos dos três quadros da evolução de design apresentados no mapa mental.

## Arquivos

### 1. `Quadro1_Procedural.cs`
**Procedural: onde surgem if/switch**

- Fluxo linear com decisões embutidas
- Toda lógica concentrada em `PaymentServiceProcedural.ProcessarPagamento()`
- Tudo em um único método

**Problemas:**
- ❌ Difícil de estender (novos meios = novo if)
- ❌ Testes frágeis (sem forma de simular falhas isoladas)
- ❌ Lógica de Pix e Cartão misturada

**Execute:**
```bash
cd src/fase-01-procedural
csc Quadro1_Procedural.cs && Quadro1_Procedural.exe
```

---

### 2. `Quadro2_OOSemInterface.cs`
**OO sem Interface: encapsulamento parcial**

- Cada meio em classe separada (`PixProcessor`, `CardProcessor`)
- Um orquestrador (`PaymentService`) escolhe qual usar

**Melhoras:**
- ✅ Coesão por meio (cada classe tem responsabilidade clara)
- ✅ Menos duplicação

**Problemas remanescentes:**
- ⚠️ `PaymentService` **ainda contém o if** de seleção
- ⚠️ Cliente muda quando adicionamos novo meio (ex: Boleto)
- ⚠️ Difícil fazer testes com dublês

**Execute:**
```bash
cd src/fase-01-procedural
csc Quadro2_OOSemInterface.cs && Quadro2_OOSemInterface.exe
```

---

### 3. `Quadro3_ComInterface.cs`
**Com Interface: desacoplamento completo**

- Contrato: `IPaymentProcessor` (o que fazer)
- Implementações: `PixProcessor`, `CardProcessor`, `BoletoProcessor` (como fazer)
- Política: `PaymentPolicySelector` (escolha fora do cliente)
- Serviço: `PaymentService` (depende apenas da abstração)

**Benefícios:**
- ✅ Cliente **não muda** ao adicionar novos meios
- ✅ Testes simples com `MockPaymentProcessor`
- ✅ Política configurável
- ✅ Desacoplamento total

**Execute:**
```bash
cd src/fase-01-procedural
csc Quadro3_ComInterface.cs && Quadro3_ComInterface.exe
```

---

## Comparação Visual

```
Quadro 1 (Procedural)
├─ if (valor > 500)
│  ├─ Cartão (inline)
│  └─ Pix (inline)

Quadro 2 (OO sem Interface)
├─ PaymentService
│  ├─ if (valor > 500)
│  │  ├─ new CardProcessor()
│  │  └─ new PixProcessor()

Quadro 3 (Com Interface) ← OBJETIVO
├─ PaymentService
│  ├─ IPaymentProcessor (abstração)
│  ├─ PaymentPolicySelector (política)
│  └─ Implementações:
│     ├─ CardProcessor
│     ├─ PixProcessor
│     └─ BoletoProcessor (sem alterar cliente!)
```

---

## Próximas Fases

- **Fase 2**: Implementar testes unitários com mocks
- **Fase 3**: Padrões avançados (Factory, Strategy, Dependency Injection)
- **Fase 4**: Aplicação integrada em ASP.NET Core

---

## Referências

- [Mapa Mental Detalhado](../../docs/arquitetura/fase-01-mapa.md)
- [README Principal](../../README.md)
