# Atividade de Identificação e criação de Interfaces em C#

## Equipe:

| Aluno | RA  |
| ---   | --- |
| Habner Gabryel Correa | 2455102 |

## Fase 0 — Objetivo e Peças Alternáveis

Definir um objetivo fixo e peças alternáveis que realizem o objetivo por 2 caminhos diferentes. Nomear o contrato (o que fazer) e duas implementações (como fazer), além de propor uma política simples para escolher entre as peças.

### **Objetivo:** 
Armas em um Jogo

### **Contrato:**  
Disparar balas

### **Implementações:** 
1. ShotGun
2. Pistola

### **Política:**
- Quando o personagem selecionar em seu inventário o item "ShotGun", os disparos deverão seguir a implementação 1
- Quando o personagem selecionar em seu inventário o item "Pistola", os disparos deverão seguir a implementação 2

### **Observações:**
- Os disparos feitos na implementação 1 terão um alcance de 6 metros e uma dispersão de balas, acertando alvos em um raio de 35º a frente do personagem
- Disparos feitos na implementação 2, o alcance será de 30 metros

---

## Fase 1 — Heurística antes do código (Mapa Mental)

**Tema:** Processamento de Pagamentos

### **Problema escolhido**
Queremos permitir que o cliente pague a compra escolhendo automaticamente o meio adequado (Pix ou Cartão) conforme regras simples de valor/risco.

### Mapa Mental
Acesse o **mapa detalhado** em: [`docs/arquitetura/fase-01-mapa.md`](docs/arquitetura/fase-01-mapa.md)

O mapa explora:
- **Quadro 1 — Procedural**: onde surgem `if/switch` (e os problemas)
- **Quadro 2 — OO sem Interface**: encapsulamento, mas ainda rigidez
- **Quadro 3 — Com Interface**: contrato, composição e desacoplamento

### Sinais de alerta previstos
1. **Cliente muda ao trocar implementação** (acoplamento ao "como")
3. **Testes lentos e frágeis** (sem dublês, integrações reais, difícil simular falhas)

### Artefatos
- Mapa mental: [`docs/arquitetura/fase-01-mapa.md`](docs/arquitetura/fase-01-mapa.md)
- Exemplos de código: `src/fase-01-procedural/`

---

## Fase 2 — Procedural Mínimo (Formatação de Texto)

**Tema:** Formatação de Texto com Múltiplos Modos

### **Problema escolhido**
Implementar uma função que formate texto escolhendo automaticamente o modo (UPPER, LOWER, TITLE, DEFAULT) conforme entrada, demonstrando os problemas do design procedural puro com `if/switch`.

### Modos de Formatação
- **UPPER** (maiúsculas): "hello" → "HELLO"
- **LOWER** (minúsculas): "HELLO" → "hello"
- **TITLE** (título): "hello world" → "Hello World"
- **DEFAULT** (padrão - 1ª letra maiúscula): "hello" → "Hello"

### Artefato Conceitual
Acesse o **documento de análise** em: [`docs/arquitetura/fase-02-mapa.md`](docs/arquitetura/fase-02-mapa.md)

Contém:
- Descrição do objetivo e 4 modos (3+ conforme requisito)
- Fluxo procedural detalhando onde surgem `if/switch`
- **5 cenários de teste/fronteira**: entrada mínima, máxima, modo inválido, combinação ambígua, caso comum
- Análise de escalabilidade: por que essa abordagem não escala ao adicionar novos modos

### Código de Referência
Demonstração procedural (puramente educativo):
- Arquivo: `src/fase-02-procedural/TextFormatterProcedural.cs`
- Implementação com `if/switch` para cada modo
- Execução dos 5 cenários de teste
- Análise de problemas ao extensão (ex.: adicionar modo REVERSE)

### Sinais de Alerta Verificados
1. ❌ **Cliente muda ao adicionar novo modo** (recompilação necessária)
2. ❌ **Ramificações espalhadas** (`else if` sequenciais em método único)
3. ❌ **Testes combinatórios** (4 modos × N tamanhos = N×4 cenários)
5. ❌ **Duplicação** (validação de entrada, normalização de modo)

### Artefatos
- Análise conceitual: [`docs/arquitetura/fase-02-mapa.md`](docs/arquitetura/fase-02-mapa.md)
- Código procedural (referência): `src/fase-02-procedural/TextFormatterProcedural.cs`
- Guia de navegação: [`src/fase-02-procedural/README.md`](src/fase-02-procedural/README.md)

---

## Fase 3 — OO sem Interface (Herança + Polimorfismo)

**Tema:** Hierarquia Polimórfica de Formatadores de Texto

### **Transformação da Fase 2**
Substituímos a função procedural com `if/switch` por uma hierarquia orientada a objetos:
- **Classe base abstrata:** `TextFormatterBase` (ritual comum)
- **Subclasses concretas:** `UpperCaseFormatter`, `LowerCaseFormatter`, `TitleCaseFormatter`, `DefaultFormatter`, `PassthroughFormatter`
- **Padrão:** Template Method + Polimorfismo

### Hierarquia de Classes
```
TextFormatterBase (abstrato)
├─ Format(text)      ← Ritual comum
└─ Apply(text)       ← Gancho abstrato
    ↓
    ├─ UpperCaseFormatter
    ├─ LowerCaseFormatter
    ├─ TitleCaseFormatter
    ├─ DefaultFormatter
    └─ PassthroughFormatter
```

### Como o Polimorfismo Substitui Decisões

**Antes (Fase 2 — Procedural):**
```csharp
// ❌ if/else DENTRO do fluxo
if (mode == "UPPER") return text.ToUpper();
else if (mode == "LOWER") return text.ToLower();
else if (mode == "TITLE") return ToTitleCase(text);
else return DefaultFormat(text);
```

**Depois (Fase 3 — Polimorfismo):**
```csharp
// ✅ Switch APENAS para escolher qual classe usar
TextFormatterBase formatter = mode switch
{
    "UPPER" => new UpperCaseFormatter(),
    "LOWER" => new LowerCaseFormatter(),
    _ => new PassthroughFormatter()
};

// ✅ Fluxo é LINEAR: sem if/else, apenas delegação polimórfica
return formatter.Format(text);
```

### Exemplo: Classe Base
```csharp
public abstract class TextFormatterBase
{
    public string Format(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return "";
        
        return Apply(text);  // ← Delegação polimórfica
    }

    protected abstract string Apply(string text);  // Gancho variável
}

public sealed class UpperCaseFormatter : TextFormatterBase
{
    protected override string Apply(string text) 
        => text?.ToUpperInvariant() ?? "";
}
```

### Responsabilidades

| Classe | Responsabilidade |
|--------|------------------|
| `TextFormatterBase` | Orquestar o ritual (`Format`), definir gancho (`Apply`) |
| `UpperCaseFormatter`, `LowerCaseFormatter`, etc. | Implementar apenas o passo variável (`Apply`) |
| `FormatterClient` | Escolher qual concreta usar (ainda via switch) |

### Análise: Melhorou vs. Ainda Rígido

#### ✅ MELHOROU
1. **Remoção de if/switch do fluxo central** → leitura clara
2. **Coesão por variação** → cada classe é simples e focada
3. **Testes isolados** → testar `UpperCaseFormatter` independente
4. **Novo modo sem alterar fluxo** → criar nova classe, não mexer em método existente

#### ⚠️ AINDA FICOU RÍGIDO
1. **Cliente AINDA conhece concretos** → `new UpperCaseFormatter()` explícito
2. **Composição dispersa** → switch está no cliente, não centralizado
3. **Sem contrato estável** → sem interface, difícil testar com dublês
4. **Extensibilidade limitada** → novo modo exige recompilação do cliente

### Artefatos
- Mapa com diagrama, código e análise: [`docs/arquitetura/fase-03-mapa.md`](docs/arquitetura/fase-03-mapa.md)
- Implementação completa: `src/fase-03-oo-sem-interface/TextFormatterPolymorphic.cs`
- Guia de navegação: [`src/fase-03-oo-sem-interface/README.md`](src/fase-03-oo-sem-interface/README.md)


## Fase 5 — Essenciais de Interfaces em C#

**Tema:** Duas interfaces do domínio e uma classe que implementa ambas

### Objetivo
Propor duas interfaces do domínio de formatação e demonstrar uma classe que implementa ambas, além de explicar implementação explícita, generics com constraints e default members.

### Onde encontrar
- Documento de análise: [`docs/arquitetura/fase-05-mapa.md`](docs/arquitetura/fase-05-mapa.md)
- Código de referência: `src/fase-05-essenciais-interfaces/InterfacesEssentials.cs`
- Guia de navegação: `src/fase-05-essenciais-interfaces/README.md`

---

## Fase 6 — ISP na prática (Segregação por Capacidade)

**Tema:** Segregar uma "god interface" em contratos coesos e ajustar consumidores

### Objetivo
Dado um contrato multifuncional, extrair interfaces coesas e mostrar o impacto no consumidor (antes/depois), sinais de ocorrência e ganhos.

### Onde encontrar
- Documento: [`docs/arquitetura/fase-06-mapa.md`](docs/arquitetura/fase-06-mapa.md)
- Código: `src/fase-06-isp/ISPExample.cs`
- Guia: `src/fase-06-isp/README.md`



## Fase 4 — Interface Plugável e Testável

**Tema:** Contrato estável (`ITextFormatter`) + Injeção de dependência

### Objetivo
Definir um contrato claro (`ITextFormatter`) e refatorar o cliente para depender dele. Demonstrar como alternar implementações sem mudar o cliente e como injetar dublês em testes.

### Onde encontrar
- Documento explicativo: [`docs/arquitetura/fase-04-mapa.md`](docs/arquitetura/fase-04-mapa.md)
- Código de referência: `src/fase-04-oo-com-interface/TextFormatterInterface.cs`
- Guia de navegação: `src/fase-04-oo-com-interface/README.md`

---

## Fase 7 — Repository progressivo (InMemory)

**Tema:** Repositório genérico em memória + serviço desacoplado

### Objetivo
Modelar um repositório em memória (`InMemoryRepository<T>`) com contrato `IRepository<T>` e demonstrar como um serviço (`MovieService`) consome esse contrato sem conhecer detalhes da implementação.

### Onde encontrar
- Código: `src/fase-07-repository/InMemoryRepository.cs`
- Documento: `docs/arquitetura/fase-07-mapa.md`
- Guia: `src/fase-07-repository/README.md`

---

## Fase 8 — Repository progressivo (CSV)

**Tema:** Persistência simples em CSV mantendo contrato `IRepository<T>`

### Objetivo
Evoluir a implementação de repositório em memória para uma versão que persista em arquivo CSV, sem alterar o contrato usado pelo cliente. Demonstrar leitura/escrita, tratamento de arquivo inexistente e por que o cliente permanece inalterado ao trocar a implementação.

### Onde encontrar
- Código: `src/fase-08-repository/CsvRepository.cs`
- Documento: `docs/arquitetura/fase-08-mapa.md`
- Guia: `src/fase-08-repository/README.md`

---

## Fase 9 — Repository progressivo (JSON)

**Tema:** Persistência com JSON mantendo contrato `IRepository<T>`

### Objetivo
Evoluir de CSV para JSON como formato de persistência, mantendo o contrato `IRepository<T>` inalterado. Demonstrar ida/volta (serialização/desserialização), mutações seguras e o ponto único de composição que permite trocar CSV→JSON sem alterar o cliente.

### Onde encontrar
- Código: `src/fase-09-repository/JsonRepository.cs`
- Documento: `docs/arquitetura/fase-09-mapa.md`
- Guia: `src/fase-09-repository/README.md`

---

## Fase 10 — Testabilidade: Dublês e Costuras

**Tema:** Planejar testes sem I/O usando dublês adequados e costuras (Seams)

### Objetivo
Demonstrar como testar componentes que dependem de repositórios sem executar operações reais de I/O. Usar dublês (Dummy, Stub, Mock, Spy, Fake) injetados via construtor e explorar "costuras" (Seams) que permitem trocar implementações sem alterar código.

### Onde encontrar
- Código: `src/fase-10-testability/TestabilityExamples.cs`
- Documento: `docs/arquitetura/fase-10-mapa.md`
- Guia: `src/fase-10-testability/README.md`

---

## Fase 11 — Cheiros e Antídotos

**Tema:** Identificar code smells em fases anteriores e descrever refatorações

### Objetivo
Analisar padrões problemáticos encontrados em fases anteriores e mostrar como eliminar acoplamento, duplicação e rigidez através de refatoração cuidadosa. Cada cheiro é ilustrado com antes/depois e a motivação por trás do antídoto.

### Dois cheiros principais

1. **God Interface (Fase 4 → Fase 6):** interface com múltiplas responsabilidades → ISP (Interface Segregation)
2. **Composição Espalhada (Fase 3 → Fase 4):** switch duplicado em clientes → Factory Pattern (decisão centralizada)

### Onde encontrar
- Código: `src/fase-11-code-smells/CodeSmellsAndAntidotes.cs`
- Documento: `docs/arquitetura/fase-11-mapa.md`
- Guia: `src/fase-11-code-smells/README.md`
