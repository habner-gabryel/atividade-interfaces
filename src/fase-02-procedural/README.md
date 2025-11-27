# Fase 2 — Procedural Mínimo (Formatação de Texto)

## 📋 Visão Geral

Esta fase implementa uma função simples de **formatação de texto** em abordagem **puramente procedural**, demonstrando claramente os problemas de escalabilidade, manutenção e extensão que surgem com `if/switch` embutidos.

**Objetivo:** Formatar texto conforme modo escolhido (UPPER, LOWER, TITLE, DEFAULT), evidenciando a dor do design procedural.

---

## 📁 Artefatos

### `fase-02-mapa.md`
Documento conceitual (sem código) contendo:
- ✅ Objetivo e modos escolhidos (3 + padrão)
- ✅ Fluxo procedural com indicação de if/switch
- ✅ 5 cenários de teste/fronteira descritos em texto
- ✅ Análise de limitações ("por que não escala")

**Acesse:** [`docs/arquitetura/fase-02-mapa.md`](../../docs/arquitetura/fase-02-mapa.md)

### `TextFormatterProcedural.cs`
Implementação em C# demonstrando:
- Abordagem procedural pura (4 `else if` para 4 modos)
- Execução dos 5 cenários de teste
- Demonstração do problema ao adicionar novo modo (REVERSE)
- Análise de escalabilidade

**Execução:**
```bash
cd src/fase-02-procedural
csc TextFormatterProcedural.cs
TextFormatterProcedural.exe
```

---

## 🎯 Modos de Formatação

| Modo | Efeito | Exemplo |
|------|--------|---------|
| **UPPER** | Maiúsculas | "hello" → "HELLO" |
| **LOWER** | Minúsculas | "HELLO" → "hello" |
| **TITLE** | Título (1ª letra maiúscula por palavra) | "hello world" → "Hello World" |
| **DEFAULT** | Primeira letra maiúscula (padrão) | "hello" → "Hello" |

---

## 🧪 5 Cenários de Teste

1. **Entrada Mínima**: String vazia → retorna vazio sem erro
2. **Entrada Máxima**: Texto com 1000+ caracteres → processa sem trava
3. **Modo Inválido**: Modo não reconhecido → cai para DEFAULT
4. **Combinação Ambígua**: Texto com 1 letra em DEFAULT e UPPER → comportamento coincide
5. **Caso Comum**: Nome completo com acentuação em TITLE → formatação correta

---

## ⚠️ Por Que Não Escala

### Problemas Estruturais

**1. Multiplicação de `if/switch`**
- 4 modos → 4 `else if`
- 10 modos → 10 `else if` (mantém sequência linear, mas cresce)
- Lógica fica cada vez mais difícil de seguir

**2. Duplicação de Lógica**
- Validação (string vazia) feita uma vez
- Se criar `FormatarTextoAvancado()`, duplica validação
- Mudança em regra comum afeta múltiplos lugares

**3. Testes Combinatórios**
- 4 modos × 3 tamanhos de entrada = 12 cenários
- Com 10 modos → 30+ cenários
- Cada cenário exige teste isolado; sem abstração, não há "teste de modo genérico"

**4. Dificuldade de Extensão**
- Adicionar novo modo (ex: REVERSE) exige:
  - Entender lógica procedural existente
  - Adicionar novo `else if`
  - Testar interação com modos existentes
  - Risco de regressão em toda a função

**5. Acoplamento ao "Como"**
- Função conhece **todos os detalhes** de cada modo
- Mudança em UPPER (ex: respeitar caracteres especiais) afeta toda a função
- Cliente não pode "compor" modo (ex: UPPER + TRIM em isolamento)

**6. Manutenção e Legibilidade**
- Método com 15+ linhas de if/else fica difícil de ler
- Novos desenvolvedores gastam tempo entendendo ramificações
- Documentação fica complexa (onde entra? onde sai?)

---

## 📊 Comparação: Escalabilidade

```
Modo    | Linhas  | if/else | Testes  | Extensibilidade
--------|---------|---------|---------|------------------
4       | ~20     | 4       | 12      | ⚠️  Difícil
10      | ~45     | 10      | 30+     | ❌ Muito difícil
20      | ~90     | 20      | 60+     | ❌ Quase impossível
```

---

## 🔑 Sinais de Alerta

- ❌ **Cliente muda ao adicionar novo modo** (recompilação/redistribuição)
- ❌ **Ramificações espalhadas** (if/switch em método único)
- ❌ **Testes frágeis** (sem forma de simular modo em isolamento)
- ❌ **Acoplamento ao "como"** (função conhece detalhes internos)

---

## ✅ Próxima Fase (3)

**Solução:** Quebrar em estratégias (polimorfismo)
- Cada modo → classe que implementa contrato comum (`ITextFormatter`)
- Cliente depende da abstração, não do concreto
- Novo modo → nova classe (não altera código existente)
- Testes → mocka interface, testa em isolamento

---

## 📚 Referências

- [Mapa Mental Fase 2](../../docs/arquitetura/fase-02-mapa.md)
- [README Principal](../../README.md)
- [Fase 1 — Heurística (Pagamentos)](../fase-01-procedural/README.md)
