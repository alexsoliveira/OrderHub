# 📚 Scripts de Automação - OrderHub

Conjunto de scripts PowerShell para automação e gerenciamento do ambiente de desenvolvimento do projeto **OrderHub**.

---

## 📋 Índice de Scripts

### 1. **setup-environment.ps1** - Setup Completo do Ambiente
**Propósito**: Configurar o ambiente de desenvolvimento do zero de forma completa e automatizada.

#### Características
- ✅ Validação de pré-requisitos (.NET SDK, SQL Server, Git)
- ✅ Restauração de dependências NuGet
- ✅ Limpeza de builds anteriores
- ✅ Compilação do projeto (Release)
- ✅ Aplicação de migrações de banco de dados
- ✅ Execução de testes unitários
- ✅ Coleta de métricas de cobertura
- ✅ Validação de análise estática de código
- ✅ Geração de relatórios detalhados

#### Uso Básico

```powershell
# Executar setup completo com todos os passos
.\setup-environment.ps1

# Executar sem testes (mais rápido)
.\setup-environment.ps1 -SkipTests

# Executar sem coletar cobertura
.\setup-environment.ps1 -SkipCoverage

# Executar sem análise estática
.\setup-environment.ps1 -SkipAnalysis

# Combinar múltiplas flags
.\setup-environment.ps1 -SkipTests -SkipCoverage
```

#### Parâmetros Avançados

```powershell
# Especificar servidor de banco de dados
.\setup-environment.ps1 -DbServer "seu-server\SQLEXPRESS"

# Especificar nome do banco
.\setup-environment.ps1 -DbName "OrderHubDb_Dev"

# Não criar/migrar banco de dados
.\setup-environment.ps1 -CreateDb $false

# Modo verbose (mais detalhes)
.\setup-environment.ps1 -Verbose
```

#### Pré-requisitos

- 🔹 **.NET 10.0 SDK** (ou superior)
- 🔹 **SQL Server** (qualquer edição) ou **SQL Server Express**
- 🔹 **LocalDB** (instalado COM Visual Studio ou SQL Express)
- 🔹 **PowerShell 5.1+** ou **PowerShell 7+**
- 🔹 **Git** (opcional, para verificações adicionais)

#### Saída Esperada

```
╔════════════════════════════════════════════════════════════╗
║ 🚀 SETUP DO AMBIENTE ORDERHUB                             ║
╚════════════════════════════════════════════════════════════╝

Configuração do Setup:
  • Executar Testes: SIM
  • Coletar Cobertura: SIM
  • Validar Análise Estática: SIM
  • Servidor BD: (localdb)\mssqllocaldb
  • Nome BD: OrderHubDb

[... etapas de validação, build, testes ...]

📈 RESUMO DO SETUP

Resultados:
  • DotNetValidation: ✅ SUCESSO
  • SqlServerValidation: ✅ SUCESSO
  • SolutionValidation: ✅ SUCESSO
  • Restore: ✅ SUCESSO
  • Build: ✅ SUCESSO
  • DatabaseMigration: ✅ SUCESSO
  • CodeAnalysis: ✅ SUCESSO
  • UnitTests: ✅ SUCESSO
  • Coverage: ✅ SUCESSO

Tempo Total: 00:05:32

✅ SETUP CONCLUÍDO COM SUCESSO!
```

#### O que o Script Faz

| Etapa | Comando | Objetivo |
|-------|---------|----------|
| **1. Validação** | `dotnet --version` / SQL Connection | Verifica pré-requisitos |
| **2. Limpeza** | `dotnet clean` | Remove artifacts anteriores |
| **3. Restore** | `dotnet restore` | Baixa dependências NuGet |
| **4. Build** | `dotnet build` | Compila solução em Release |
| **5. Migração** | `dotnet ef database update` | Cria/atualiza BD |
| **6. Análise** | `dotnet build /p:EnforceCodeStyleInBuild=true` | Valida style |
| **7. Testes** | `dotnet test` | Executa testes unitários |
| **8. Cobertura** | `dotnet test /p:CollectCoverage=true` | Coleta métricas |
| **9. Verificação** | `check-coverage-threshold.ps1` | Valida threshold 80% |

---

### 2. **run-tests-with-coverage.ps1** - Executar Testes com Cobertura
**Propósito**: Executar testes e gerar relatório detalhado de cobertura de código.

#### Uso

```powershell
# Executar com saída padrão
.\run-tests-with-coverage.ps1

# Gerar apenas relatório (sem executar testes novamente)
.\run-tests-with-coverage.ps1 -GenerateReportOnly
```

#### Saída

- 📊 Arquivo `coverage/coverage.opencover.xml` (formato OpenCover)
- 📊 Arquivo `coverage/coverage.json` (resumo em JSON)
- 📄 Arquivo `coverage/report/index.html` (relatório HTML interativo)
- 📝 Arquivo `coverage/COVERAGE_SUMMARY.md` (resumo Markdown)

#### Interpretar Resultados

```
Coverage Summary
================

Line Coverage:   85.3%  ✅ (Acima de 80%)
Branch Coverage: 82.1%  ✅ (Acima de 80%)
Method Coverage: 87.2%  ✅ (Acima de 80%)

Por Projeto:
  OrderHub.Domain:       88% (Core business logic - Bem testado)
  OrderHub.Application:  82% (Use cases - Acima do mínimo)
  API Adapters:          75% (Controllers - Em desenvolvimento)
  Persistence:           79% (Repositories - Em desenvolvimento)
```

---

### 3. **check-coverage-threshold.ps1** - Validar Threshold de Cobertura
**Propósito**: Verificar se cobertura de testes atende ao mínimo de 80%.

#### Uso

```powershell
# Validar com arquivo de cobertura padrão
.\check-coverage-threshold.ps1

# Validar com arquivo específico
.\check-coverage-threshold.ps1 -CoverageFilePath "seu-caminho/coverage.json"

# Especificar threshold diferente
.\check-coverage-threshold.ps1 -MinimumCoverage 0.75

# Combinar parâmetros
.\check-coverage-threshold.ps1 -CoverageFilePath "coverage.json" -MinimumCoverage 0.85
```

#### Retorno

```powershell
# Sucesso (exit code 0)
✅ SUCESSO: Cobertura 85.0% atende ao mínimo de 80%

# Falha (exit code 1)
❌ FALHA: Cobertura 65.0% está abaixo do mínimo de 80%
```

#### Integração no CI/CD

```yaml
# No azure-pipelines.yml
- task: PowerShell@2
  displayName: 'Validate Coverage Threshold'
  inputs:
    filePath: 'scripts/check-coverage-threshold.ps1'
    arguments: '-CoverageFilePath "coverage/coverage.json" -MinimumCoverage 0.80'
    failOnStderr: true
```

---

## 🚀 Cenários de Uso Comum

### Desenvolvedores Iniciantes (Primeira Vez)

```powershell
# Setup completo do ambiente
.\setup-environment.ps1
```

**O que acontece:**
1. Valida todos os pré-requisitos
2. Restaura dependências
3. Compila o projeto
4. Cria/atualiza banco de dados
5. Executa testes
6. Coleta cobertura
7. Valida análise estática

### Desenvolvedores Experientes (Mudança Rápida)

```powershell
# Setup rápido (sem testes)
.\setup-environment.ps1 -SkipTests -SkipCoverage

# Depois, rodar testes manualmente quando pronto
dotnet test
```

### Antes de Fazer Commit

```powershell
# Validar tudo (testes + cobertura)
.\run-tests-with-coverage.ps1
.\check-coverage-threshold.ps1
```

### No Pipeline CI/CD

```powershell
# Setup sem interação (para CI)
.\setup-environment.ps1 -Verbose

# Falha automaticamente se algo errar (exit code != 0)
```

---

## 🔧 Troubleshooting

### Erro: ".NET SDK não encontrado"

**Solução:**
```powershell
# Instale .NET 10.0 SDK de: https://dotnet.microsoft.com/download
dotnet --version  # Verificar instalação
```

### Erro: "SQL Server não acessível"

**Solução:**
```powershell
# Verificar se SQL Server/LocalDB está rodando
# Windows: Services > SQL Server (LOCALDB)
# Use -DbServer para especificar servidor alternativo
.\setup-environment.ps1 -DbServer "seu-servidor\SQLEXPRESS"
```

### Erro: "Não há permissão para executar scripts"

**Solução:**
```powershell
# Permitir execução (PowerShell como Admin)
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
# Ou executar com flag
PowerShell -ExecutionPolicy Bypass -File setup-environment.ps1
```

### Erro: "Migrações falharam"

**Solução:**
```powershell
# Pular criação de BD e criar manualmente depois
.\setup-environment.ps1 -CreateDb $false

# Depois, criar BD manualmente
cd src/OrderHub.Adapters.Inbound.Api
dotnet ef database update -p ../OrderHub.Adapters.Outbound.Persistence
```

---

## 📊 Estrutura de Diretórios

Após executar `setup-environment.ps1`, a seguinte estrutura é criada:

```
OrderHub/
├── bin/                           # Artifacts compilados
├── obj/                           # Objetos de build
├── coverage/                      # Relatórios de cobertura
│   ├── coverage.opencover.xml     # Formato OpenCover
│   ├── coverage.json              # Resumo JSON
│   ├── COVERAGE_SUMMARY.md        # Resumo Markdown
│   └── report/
│       └── index.html             # Relatório HTML interativo
├── logs/                          # Logs da aplicação (Serilog)
└── scripts/
    ├── setup-environment.ps1      # ← Você está aqui
    ├── run-tests-with-coverage.ps1
    └── check-coverage-threshold.ps1
```

---

## 📖 Referências

### Documentação Official

- **[.NET 10 Documentation](https://learn.microsoft.com/en-us/dotnet/core/)**
- **[Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)**
- **[PowerShell Docs](https://learn.microsoft.com/en-us/powershell/)**

### No Projeto

- **[README.md](../README.md)** - Descrição geral do projeto
- **[BRANCH_POLICY.md](../BRANCH_POLICY.md)** - Políticas de branch e Git
- **[DOC_IA/](../DOC_IA/)** - Documentação detalhada por feature

---

## 👥 Contribuições

Se encontrar problemas ou tiver sugestões sobre os scripts, abra uma issue ou PR.

---

**Última Atualização:** 18 de Março de 2026  
**Versão**: 1.0.0  
**Framework**: .NET 10.0  
**PowerShell**: 5.1+
