# Política de Branches - OrderHub

## Estratégia de Ramificação

Este projeto segue uma estratégia de **Git Flow** simplificada com duas branches principais:

### 📌 Main Branch (Produção)
- **Nome**: `main`
- **Propósito**: Código em produção, pronto para release
- **Tipo de commit**: Apenas merges de `release` ou `hotfix`
- **Proteção**:
  - ✅ Requer Pull Request
  - ✅ Requer 2 reviewers
  - ✅ Status de build deve passar
  - ✅ Todos os comentários devem ser resolvidos
  - ❌ Não permite force push
  - ✅ Delete branch após merge

### 🔄 Develop Branch (Desenvolvimento)
- **Nome**: `develop`
- **Propósito**: Integração contínua de features
- **Tipo de commit**: Merges de `feature/*` e `bugfix/*`
- **Proteção**:
  - ✅ Requer Pull Request
  - ✅ Requer 1 reviewer
  - ✅ Status de build deve passar
  - ✅ Todos os comentários devem ser resolvidos
  - ❌ Não permite force push
  - ✅ Delete branch após merge

### 🌿 Branches de Feature
- **Padrão**: `feature/{número-da-tarefa}/{descrição}`
- **Exemplo**: `feature/FEAT-01/setup-projeto`
- **Origem**: Criar a partir de `develop`
- **Destino**: Merge de volta para `develop`
- **Exclusão**: Automática após merge (exceto em casos especiais)

### 🐛 Branches de Bug Fix
- **Padrão**: `bugfix/{número-da-tarefa}/{descrição}`
- **Exemplo**: `bugfix/BUG-05/corrigir-validacao`
- **Origem**: Criar a partir de `develop`
- **Destino**: Merge de volta para `develop`
- **Exclusão**: Automática após merge

### 🚀 Branches de Release
- **Padrão**: `release/{versão}`
- **Exemplo**: `release/1.0.0`
- **Origem**: Criar a partir de `develop`
- **Destino**: Merge para `main` e de volta para `develop`
- **Exclusão**: Manter para histórico

### 🔥 Branches de Hotfix
- **Padrão**: `hotfix/{número-da-tarefa}/{descrição}`
- **Exemplo**: `hotfix/HOTFIX-12/corrigir-critico`
- **Origem**: Criar a partir de `main`
- **Destino**: Merge para `main` e `develop`
- **Exclusão**: Manter para histórico

## Fluxo de Trabalho

```
┌─────────────────────────────────────────────────────────┐
│                    MAIN (Produção)                       │
│                   (v1.0.0, v1.0.1...)                   │
└──────────────────────┬───────────────────────────────────┘
                       │ Hotfix merge
┌──────────────────────▼───────────────────────────────────┐
│              DEVELOP (Integração)                        │
│              (próxima versão)                            │
└──────┬──────────┬──────────┬──────────┬──────────────────┘
       │          │          │          │
       ▼          ▼          ▼          ▼
   feature/   feature/   bugfix/    release/
   FEAT-01    FEAT-02    BUG-05     1.0.0
```

## Convenção de Commits

Utilize commits descritivos seguindo este padrão:

```
[TIPO]: Descrição breve (máximo 50 caracteres)

[Descrição detalhada, se necessário]

Closes #<número-da-issue>
```

### Tipos de Commit:
- **feat**: Nova feature
- **fix**: Correção de bug
- **docs**: Alterações em documentação
- **style**: Formatação, sem mudanças de lógica
- **refactor**: Refatoração de código
- **test**: Adição ou alteração de testes
- **chore**: Alterações em dependências, build scripts

### Exemplos:
```
feat: Implementar autenticação JWT

Adiciona autenticação via JWT tokens na API REST

Closes #42

---

fix: Corrigir validação de email

Closes #85

---

docs: Atualizar guia de instalação
```

## Pull Request (PR)

Cada feature/bugfix deve ser integrada via **Pull Request**:

1. **Título claro**: "feat(auth): Implementar JWT token"
2. **Descrição**: Explique WHAT e WHY, não HOW
3. **Checklist**:
   - ✅ Testes passando
   - ✅ Sem conflitos com `develop`
   - ✅ Código revisado por pelo menos 1 pessoa
   - ✅ Documentação atualizada

## Versionamento

Siga **Semantic Versioning** (MAJOR.MINOR.PATCH):

- **MAJOR** (1.0.0): Breaking changes
- **MINOR** (1.1.0): Novas features
- **PATCH** (1.0.1): Bug fixes

## Regras Importantes

1. ❌ **Nunca** fazer commit direto em `main` ou `develop`
2. ✅ **Sempre** criar uma feature branch
3. ✅ **Sempre** fazer PR para integração
4. ✅ **Sempre** executar testes antes de PR
5. ❌ **Nunca** usar force push em branches compartilhadas
6. ✅ **Sempre** deletar branch após merge
7. ✅ **Sempre** manter `main` estável e pronto para produção

## Configuração no Git

Configurar Git localmente:

```bash
# Clone o repositório
git clone https://github.com/seu-usuario/OrderHub.git
cd OrderHub

# Configure o seu nome e email
git config user.name "Seu Nome"
git config user.email "seu-email@exemplo.com"

# Defina `develop` como branch padrão
git checkout develop
git pull origin develop
```

## Criando uma Nova Feature

```bash
# Partindo de develop
git checkout develop
git pull origin develop

# Create feature branch
git checkout -b feature/FEAT-01/setup-projeto

# Fazer suas alterações
# Depois de pronto, fazer commit
git add .
git commit -m "feat(setup): Configurar estrutura inicial do projeto"

# Push para origin
git push origin feature/FEAT-01/setup-projeto

# Abrir Pull Request no GitHub/Azure DevOps
```

---

**Última atualização**: 12 de Março de 2026
