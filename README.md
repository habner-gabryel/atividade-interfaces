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
4. ❌ **Acoplamento ao "como"** (função conhece detalhes de cada modo)
5. ❌ **Duplicação** (validação de entrada, normalização de modo)

### Artefatos
- Análise conceitual: [`docs/arquitetura/fase-02-mapa.md`](docs/arquitetura/fase-02-mapa.md)
- Código procedural (referência): `src/fase-02-procedural/TextFormatterProcedural.cs`
- Guia de navegação: [`src/fase-02-procedural/README.md`](src/fase-02-procedural/README.md)
