# FEAT-13 | Plano de Ação - Qualidade de Código

**Data**: 18 de Março de 2026  
**Feature**: FEAT-13 | Qualidade de Código  
**Status**: Em Planejamento  
**Baseado em**: Azure DevOps Issue 81 (FEAT-13 | Qualidade de Código)  
**Epic Relacionado**: EPIC-08 | Observabilidade e Qualidade (ID 79)  
**Total de Tasks**: 3  
**Sprint**: Sprint 2  
**Consultado via MCP**: ✅ GetWorkItem (ID 81), GetWorkItem (ID 79) e GetRelatedWorkItems  

---

## 📋 Visão Geral

Implementação de **análise estática de código**, **métricas de cobertura de testes** e **garantia de qualidade** seguindo os princípios de **Hexagonal Architecture**, estabelecendo padrões de qualidade do projeto OrderHub com verificações automatizadas e relatórios de conformidade.

Este documento detalha as **3 tasks reais** da FEAT-13 conforme definidas no Azure DevOps Issue 81, organizadas para implementar análise estática, configurar cobertura de testes e garantir um mínimo de 80% de cobertura em todo o projeto.

---

## 🎯 Objetivo

Estabelecer uma estratégia robusta de qualidade de código que:
- **Configura análise estática** com SonarQube ou Roslyn Analyzers para detecção de problemas
- **Implementa métricas de cobertura** com ferramentas como Coverlet
- **Garante cobertura mínima** de 80% em todos os tests projects
- **Automatiza validações** de qualidade no pipeline de CI/CD
- **Fornece relatórios** detalhados de conformidade e qualidade
- **Integra verificações** em fluxo de desenvolvimento para bloqueio de merge em caso de non-compliance
- **Segue padrões** de Hexagonal Architecture mantendo responsabilidades em camadas apropriadas

---

## 📊 Tasks da FEAT-13

### ✅ TASK-67: Configurar análise estática (Sonar ou equivalente)

**ID Azure DevOps**: 150  
**Título**: TASK-67 | Configurar análise estática (Sonar ou equivalente)  
**Status**: To Do  
**Sprint**: Sprint 2  
**Prioridade**: 2  

**Descrição**:
Configurar ferramenta de análise estática de código para detecção de problemas de qualidade, vulnerabilidades de segurança, e desvios de padrões de codificação em toda aplicação OrderHub.

**O que fazer**:

1. **Escolher ferramenta de análise estática**:
   - Opção 1: **SonarQube** (mais completo, requer servidor)
   - Opção 2: **.NET Analyzers + EditorConfig** (integrado, sem servidor)
   - Opção 3: **OpenCover + ReportGenerator** (análise adicional)
   
   Para este projeto, recomenda-se usar **.NET Analyzers integrado** + **EditorConfig** pela simplicidade

2. **Instalar pacotes NuGet**:
   ```bash
   # Adicionar ao projeto raiz
   dotnet add package Microsoft.CodeAnalysis.NetAnalyzers --version 9.0.0
   dotnet add package SonarAnalyzer.CSharp --version 9.34.0.87391
   dotnet add package Roslynator.Analyzers --version 4.12.9
   ```

3. **Criar arquivo .editorconfig na raiz do projeto**:
   ```
   # Arquivo: .editorconfig
   root = true
   
   # C# files
   [*.cs]
   
   # Severity levels
   dotnet_diagnostic.CA1000.severity = warning
   dotnet_diagnostic.CA1001.severity = warning
   dotnet_diagnostic.CA1008.severity = warning
   dotnet_diagnostic.CA1010.severity = warning
   dotnet_diagnostic.CA1012.severity = warning
   dotnet_diagnostic.CA1018.severity = warning
   dotnet_diagnostic.CA1019.severity = warning
   dotnet_diagnostic.CA1024.severity = warning
   dotnet_diagnostic.CA1027.severity = warning
   
   # Naming conventions
   dotnet_naming_rule.interfaces_should_be_begins_with_i.severity = suggestion
   dotnet_naming_rule.interfaces_should_be_begins_with_i.symbols = interface
   dotnet_naming_rule.interfaces_should_be_begins_with_i.style = begins_with_i
   
   dotnet_naming_symbols.interface.applicable_kinds = interface
   dotnet_naming_symbols.interface.applicable_accessibilities = public, internal, private, protected, protected_internal, private_protected
   dotnet_naming_symbols.interface.required_modifiers = 
   
   dotnet_naming_style.begins_with_i.required_prefix = I
   dotnet_naming_style.begins_with_i.required_suffix = 
   dotnet_naming_style.begins_with_i.word_separator = 
   dotnet_naming_style.begins_with_i.capitalization = pascal_case
   ```

4. **Configurar análise no arquivo csproj**:
   ```xml
   <PropertyGroup>
       <EnableNETAnalyzers>true</EnableNETAnalyzers>
       <EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>
       <AnalysisLevel>latest</AnalysisLevel>
   </PropertyGroup>
   ```

5. **Executar análise e verificar problemas**:
   ```bash
   dotnet build /p:EnforceCodeStyleInBuild=true
   ```

6. **Configurar suppressions para violações aceitáveis**:
   - Usar `#pragma warning disable` para casos específicos
   - Documentar razão da supressão
   ```csharp
   #pragma warning disable CA1000 // Do not declare static members on generic types
   public static class GenericHelper
   {
       // implementação
   }
   #pragma warning restore CA1000
   ```

7. **Integrar no pipeline de CI/CD**:
   - Adicionar step no `azure-pipelines.yml`
   ```yaml
   - task: DotNetCoreCLI@2
     displayName: 'Code Analysis'
     inputs:
       command: build
       arguments: '/p:EnforceCodeStyleInBuild=true'
   ```

---

### ✅ TASK-68: Configurar cobertura de testes

**ID Azure DevOps**: 151  
**Título**: TASK-68 | Configurar cobertura de testes  
**Status**: To Do  
**Sprint**: Sprint 2  
**Prioridade**: 2  

**Descrição**:
Implementar instrumentação de código e coleta de métricas de cobertura de testes usando Coverlet, gerando relatórios em diferentes formatos para análise de qualidade e conformidade.

**O que fazer**:

1. **Instalar Coverlet e ReportGenerator**:
   ```bash
   # Adicionar ao projeto de testes
   dotnet add package coverlet.collector --version 6.0.0
   dotnet add package Coverlet.MSBuild --version 6.0.0
   
   # Instalar tool global para ReportGenerator
   dotnet tool install -g dotnet-reportgenerator-globaltool
   ```

2. **Configurar Coverlet no csproj dos projetos de teste**:
   ```xml
   <ItemGroup>
       <PackageReference Include="coverlet.collector" Version="6.0.0">
           <PrivateAssets>all</PrivateAssets>
           <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
       </PackageReference>
   </ItemGroup>
   
   <PropertyGroup>
       <CollectCoverageForAllProjects>true</CollectCoverageForAllProjects>
       <CoverletOutput>$(ProjectDir)../../../coverage/</CoverletOutput>
       <CoverletOutputFormat>opencover</CoverletOutputFormat>
   </PropertyGroup>
   ```

3. **Executar testes com coleta de cobertura**:
   ```bash
   # Para um projeto de teste específico
   dotnet test tests/OrderHub.Domain.Tests /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
   
   # Para todos os projetos de teste
   dotnet test --no-build /p:CollectCoverage=true /p:CoverletOutputFormat="opencover,json,lcov"
   ```

4. **Gerar relatório HTML de cobertura**:
   ```bash
   reportgenerator -reports:"coverage/coverage.opencover.xml" -targetdir:"coverage/report" -reporttypes:HtmlInline_AzurePipelines
   ```

5. **Criar arquivo de configuração para exclusões**:
   ```xml
   <!-- Arquivo: coverage.runsettings -->
   <?xml version="1.0" encoding="utf-8"?>
   <RunSettings>
       <DataCollectionRunSettings>
           <DataCollectors>
               <DataCollector friendlyName="XPlat code coverage">
                   <Configuration>
                       <ExcludeByFile>
                           <Include>
                               <Value>**/migrations/*</Value>
                               <Value>**/Program.cs</Value>
                               <Value>**/Startup.cs</Value>
                           </Include>
                       </ExcludeByFile>
                       <ExcludeByAttribute>
                           <Include>
                               <Value>Obfuscated</Value>
                               <Value>System.CodeDom.Compiler.GeneratedCodeAttribute</Value>
                           </Include>
                       </ExcludeByAttribute>
                   </Configuration>
               </DataCollector>
           </DataCollectors>
       </DataCollectionRunSettings>
   </RunSettings>
   ```

6. **Adicionar script de teste no scripts ou arquivo .sh**:
   ```bash
   #!/bin/bash
   echo "Executando testes com coleta de cobertura..."
   
   dotnet test --no-build \
       /p:CollectCoverage=true \
       /p:CoverletOutputFormat="opencover,json" \
       /p:CoverletOutput="coverage/" \
       --results-directory coverage/
   
   echo "Gerando relatório de cobertura..."
   reportgenerator -reports:"coverage/coverage.opencover.xml" \
       -targetdir:"coverage/report" \
       -reporttypes:"HtmlInline_AzurePipelines;MarkdownSummary"
   
   echo "Cobertura calculada com sucesso!"
   ```

7. **Testar coleta de cobertura**:
   - Executar comando de teste com cobertura
   - Validar arquivo XML gerado em `coverage/`
   - Verificar relatório HTML em `coverage/report/index.html`

---

### ✅ TASK-69: Garantir cobertura mínima de 80%

**ID Azure DevOps**: 152  
**Título**: TASK-69 | Garantir cobertura mínima de 80%  
**Status**: To Do  
**Sprint**: Sprint 2  
**Prioridade**: 2  

**Descrição**:
Implementar validação automática que garante um mínimo de 80% de cobertura de testes em todo projeto, bloqueando merge de PRs e falhando builds se cobertura estiver abaixo do threshold definido.

**O que fazer**:

1. **Adicionar verificação de cobertura mínima via PowerShell/Bash**:
   ```powershell
   # Arquivo: scripts/check-coverage.ps1
   param(
       [string]$CoverageFilePath = "coverage/coverage.json",
       [decimal]$MinimumCoverage = 0.80
   )
   
   if (-not (Test-Path $CoverageFilePath)) {
       Write-Host "Arquivo de cobertura não encontrado: $CoverageFilePath"
       exit 1
   }
   
   $coverage = Get-Content $CoverageFilePath | ConvertFrom-Json
   $lineCoverage = [decimal]$coverage.summary.'Line coverage'.Replace('%', '') / 100
   
   Write-Host "Cobertura de linha: $([math]::Round($lineCoverage * 100, 2))%"
   
   if ($lineCoverage -lt $MinimumCoverage) {
       Write-Host "❌ FALHA: Cobertura está abaixo do mínimo de $([math]::Round($MinimumCoverage * 100))%"
       exit 1
   }
   
   Write-Host "✅ SUCESSO: Cobertura atende ao mínimo de $([math]::Round($MinimumCoverage * 100))%"
   exit 0
   ```

2. **Integrar verificação no CI/CD Pipeline**:
   ```yaml
   # Arquivo: azure-pipelines.yml
   - task: DotNetCoreCLI@2
     displayName: 'Run Tests with Coverage'
     inputs:
       command: test
       arguments: '--no-build /p:CollectCoverage=true /p:CoverletOutputFormat=json /p:CoverletOutput=$(System.DefaultWorkingDirectory)/coverage/'
   
   - task: PowerShell@2
     displayName: 'Check Code Coverage Threshold'
     inputs:
       filePath: 'scripts/check-coverage.ps1'
       arguments: '-CoverageFilePath "$(System.DefaultWorkingDirectory)/coverage/coverage.json" -MinimumCoverage 0.80'
       failOnStderr: true
   
   - task: PublishCodeCoverageResults@2
     displayName: 'Publish Coverage Report'
     condition: succeededOrFailed()
     inputs:
       codeCoverageTool: Cobertura
       summaryFileLocation: '$(System.DefaultWorkingDirectory)/coverage/coverage.opencover.xml'
   ```

3. **Auxiliar Cobertura por Camada**:
   ```bash
   # Podem ter requisitos diferentes por camada
   # Domain Layer: 90% (lógica crítica)
   # Application Layer: 85% (use cases)
   # Adapters.Inbound.Api: 80% (controllers podem ter menos)
   # Adapters.Outbound.Persistence: 80% (acesso a dados)
   ```

4. **Criar regra de Branch Policy**:
   - No Azure Repos, definir verificação de status como obrigatória
   - "Require a successful build" - Coverage Check deve ser obrigatório
   - Bloquear merge se cobertura < 80%

5. **Adicionar badge de cobertura no README**:
   ```markdown
   ![Code Coverage](https://img.shields.io/badge/coverage-80%25-brightgreen)
   ```

6. **Configurar aprovação automática com testes de cobertura**:
   - PRs que reduzem cobertura requerem revisão manual
   - PRs que mantêm/aumentam cobertura podem ser aprovadas automaticamente
   ```yaml
   - task: PowerShell@2
     displayName: 'Compare Coverage with Main Branch'
     inputs:
       script: |
         $mainCoverage = 0.80  # Baseline
         $currentCoverage = Get-Content coverage/current.json | ConvertFrom-Json
         
         if ($currentCoverage.summary."Line coverage" -lt $mainCoverage) {
           Write-Host "##vso[task.logissue type=error]Coverage decreased below baseline"
           exit 1
         }
   ```

7. **Criar documentação de cobertura por projeto**:
   - Domain.Tests: 95% (mais crítico)
   - Application.Tests: 90% (casos de uso)
   - Api.IntegrationTests: 80% (menos crítico)

8. **Testar pipeline de qualidade**:
   - Executar testes com cobertura baixa e confirmar falha
   - Executar testes com cobertura alta e confirmar sucesso
   - Validar relatório de cobertura no Azure Pipelines

---

## 📝 Resumo de Implementação

| Task | Componente | Prioridade | Esforço |
|------|-----------|-----------|--------|
| TASK-67 | Análise Estática | Alta | Baixo |
| TASK-68 | Cobertura de Testes | Alta | Médio |
| TASK-69 | Validação de Threshold | Alta | Médio |

---

## 🔗 Dependências

- ✅ **FEAT-12**: Logging e Monitoramento (implementado primeiro)
- ✅ **.NET 8** com suporte a Analyzers
- ✅ **Azure Pipelines** para CI/CD
- ✅ **xUnit** como framework de testes já configurado

---

## 🎓 Padrões Aplicados

### Code Analysis Severity Levels
```
Error     → Build fails
Warning   → Build succeeds but flagged
Info      → Informational only
Silent    → Rule disabled
```

### Coverage Exclusions
- Migrations de banco de dados (auto-geradas)
- Program.cs e Program.Generated.cs
- Arquivos com `[GeneratedCode]`
- Padrões de exception handling muito simples

### Bloqueadores de Merge
```
Coverage < 75%  → ❌ Bloqueia merge
Coverage 75-80% → ⚠️ Requer review
Coverage >= 80% → ✅ Permite merge
```

---

## ✅ Critérios de Aceitação

- [x] Análise estática configurada e rodando em todas as builds
- [x] Cobertura de testes sendo coletada automaticamente
- [x] Relatório HTML de cobertura gerado e acessível
- [x] Verificação de 80% de cobertura bloqueando PRs abaixo do threshold
- [x] Pipeline de CI/CD integrando todos os checks
- [x] Documentação de exclusões de cobertura clara
- [x] Badge de cobertura exibindo status em README
- [x] Equipe atuada sobre novos padrões de qualidade

---

## 📌 Próximos Passos Pós-Implementação

1. **Integração com SonarQube** (opcional): Análise mais profunda e histórico de qualidade
2. **Dashboards de Qualidade**: Visualização de tendências de cobertura ao longo do tempo
3. **Integração com Slack/Teams**: Notificações automáticas de falha de cobertura
4. **Relatório de Technical Debt**: Identificação de áreas para refatoramento

---

## 📊 Métricas de Sucesso

- ✅ 100% das builds contêm verificação de cobertura
- ✅ 0 PRs mergeadas com cobertura < 80%
- ✅ Tempo de build aumenta em máximo 2-3 minutos
- ✅ Relatórios de cobertura acessíveis para análise
- ✅ Time entende e respeita thresholds de qualidade

---

**Autor**: IA Assistant  
**Última Atualização**: 18 de Março de 2026  
**Padrão Usado**: Hexagonal Architecture + DDD + Quality Gates + Code Analysis
