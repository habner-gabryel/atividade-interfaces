# Fase 2 — Procedural mínimo (ex.: formatar texto)

## Objetivo

Implementar uma função que formate texto escolhendo automaticamente o modo de formatação (maiúsculas, minúsculas, título ou padrão) conforme a entrada do usuário. A função deve demonstrar a evolução de um design procedural puro (com if/switch espalhados) e as dificuldades que surgem ao adicionar novos modos.

---

## Modos de Formatação

| Modo | Efeito Esperado | Exemplo |
|------|-----------------|---------|
| **UPPER** | Converte para maiúsculas | "hello" → "HELLO" |
| **LOWER** | Converte para minúsculas | "HELLO" → "hello" |
| **TITLE** | Capitaliza primeira letra de cada palavra | "hello world" → "Hello World" |
| **DEFAULT** | Retorna o texto com primeira letra maiúscula (padrão) | "hello world" → "Hello world" |

**Modo padrão:** `DEFAULT` (aplicado quando nenhum modo coincide ou entrada é inválida)

---

## Fluxo Procedural

```
1. Recebe entrada: (texto, modo)
   
2. Valida se texto não é vazio
   └─ Se vazio: retorna "" + aviso
   
3. Normaliza modo: converte para uppercase, remove espaços
   
4. Aplica if/switch na sequência:
   ├─ if (modo == "UPPER")     → return texto.ToUpper()
   ├─ else if (modo == "LOWER") → return texto.ToLower()
   ├─ else if (modo == "TITLE") → return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(texto)
   └─ else (DEFAULT)           → return Char.ToUpper(texto[0]) + texto.Substring(1)
   
5. Retorna texto formatado
```

**Onde estão as decisões:** Os `if/else` estão concentrados no passo 4, mas o conhecimento sobre cada modo (como funciona, quando aplicar, validações específicas) está distribuído no método único.

---

## 5 Cenários de Teste/Fronteira

### 1️⃣ Entrada Mínima (string vazia)
- **Entrada:** `FormatarTexto("", "UPPER")`
- **Esperado:** `""`
- **Motivo:** Testar comportamento com entrada nula; o sistema deve retornar gracefully sem exceção
- **Validação:** Não estoura índice; função retorna vazio reconhecível

### 2️⃣ Entrada Máxima/Limite (texto muito grande)
- **Entrada:** `FormatarTexto(string com 10.000 caracteres, "TITLE")`
- **Esperado:** Texto inteiro formatado em Title Case
- **Motivo:** Verificar desempenho e comportamento com entrada acima do esperado; pode revelar lentidão na iteração
- **Validação:** Função não trava; resultado formatado corretamente (sem truncamento)

### 3️⃣ Modo Inválido (cai no padrão)
- **Entrada:** `FormatarTexto("hello", "GIRAR")`
- **Esperado:** `"Hello"` (aplica DEFAULT)
- **Motivo:** Testar robustez: modo não reconhecido deve cair silenciosamente no padrão, não lançar exceção
- **Validação:** Retorna DEFAULT sem erro; comportamento inequívoco

### 4️⃣ Combinação Ambígua (modo padrão vs. UPPER para texto com 1 letra)
- **Entrada:** `FormatarTexto("a", "DEFAULT")` vs. `FormatarTexto("a", "UPPER")`
- **Esperado:** 
  - DEFAULT → `"A"` (primeira letra maiúscula)
  - UPPER → `"A"` (maiúsculas totais)
- **Motivo:** Ambas produzem o mesmo resultado para texto curto; mostra limite onde modos coincidem
- **Decisão:** Diferenciar porque UPPER tem contrato diferente (sempre maiúsculas, não apenas primeira)
- **Validação:** Para entrada com 2+ caracteres, modos se diferenciam claramente

### 5️⃣ Caso Comum Representativo
- **Entrada:** `FormatarTexto("joão da silva", "TITLE")`
- **Esperado:** `"João Da Silva"`
- **Motivo:** Caso realista (nome completo, acentuação); valida implementação de título com culturalmente correto
- **Validação:** Acentos preservados; cada palavra começa com maiúscula

---

## Por Que Essa Abordagem Não Escala

### Problemas Estruturais

1. **Multiplicação de `if/switch`**: Cada novo modo (ex.: REVERSE, CAMELCASE, SNAKE_CASE) exige um novo `else if` no método. Com 10 modos, há 10 ramificações; com 20, há 20. A lógica linear fica exponencialmente complexa.

2. **Duplicação de Lógica**: Validação de entrada (string vazia, null) é feita uma vez, mas se adicionarmos um método `FormatarTextoAvancado()` com mais modos, precisamos duplicar essas validações. Lógica comum (trim, normalização) não fica centralizada.

3. **Testes Combinatórios**: Para testar 4 modos × 3 tamanhos de entrada × 2 estados (válido/inválido), temos 24 cenários. Com 10 modos, explode para 60+. Sem abstração, cada cenário exige seu próprio teste unitário específico.

4. **Dificuldade de Extensão**: Adicionar um novo modo não é apenas "adicionar um `else if`"; é:
   - Entender a lógica procedural existente
   - Decidir onde inserir o novo `else if` (ordem importa?)
   - Testar interação com modos existentes
   - Revisar o método inteiro (alto risco de regressão)

5. **Acoplamento ao "Como"**: A função conhece **todos os detalhes** de cada modo. Se o comportamento de UPPER mudar (ex.: respeitar caracteres especiais), toda a função deve ser revisada, testada novamente e reimplantada.

6. **Manutenção e Legibilidade**: Um método procedural com 15+ linhas de if/else fica difícil de ler, debugar e explicar. Novos desenvolvedores gastam tempo entendendo fluxo em vez de estender.

---

## Onde Dói ao Adicionar Novos Modos

### Cenário: Adicionar modo `REVERSE` (inverte texto)

**Antes (code review):**
```
Método FormatarTexto tem 25 linhas
├─ Adicionar novo else if para REVERSE
├─ Revisar ordem dos ifs (importa?) → SIM, pode afetar precedência
├─ Atualizar testes: 24 cenários → 30+ novos cenários
├─ Verificar se REVERSE conflita com UPPER em cases especiais
└─ Reimplementar tudo se a estrutura não permitir
```

**Impacto:**
- ⚠️ **Mudança simples (1 feature) vira mudança complexa** (toda função)
- ⚠️ **Risco de regressão**: alterar um if pode quebrar comportamento de outro
- ⚠️ **Teste não isolado**: não há forma de testar só REVERSE sem toda a máquina

---

## Resumo: Sinal de Alerta

Este design procedural **demonstra claramente** por que interfaces e polimorfismo existem:
- ❌ Cliente muda ao adicionar novo modo
- ❌ Lógica espalhada (validação, decisão, aplicação)
- ❌ Testes frágeis e dependentes de toda a máquina
- ✅ **Próxima Fase (3)**: Quebrar em estratégias (classes), não ramificações

