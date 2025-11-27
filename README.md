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
2. **Ramificações espalhadas** (`if/switch` por tipo/valor em múltiplos pontos)
3. **Testes lentos e frágeis** (sem dublês, integrações reais, difícil simular falhas)

### Artefatos
- Mapa mental: [`docs/arquitetura/fase-01-mapa.md`](docs/arquitetura/fase-01-mapa.md)
- Exemplos de código: `src/fase-01-procedural/`
