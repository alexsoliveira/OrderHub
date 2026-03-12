# OrderHub - Hexagonal Architecture Lab (.NET)

## 📋 Descrição

**OrderHub** é um projeto educacional que implementa os princípios de **Arquitetura Hexagonal** (Ports & Adapters) utilizando **.NET 8**.

O objetivo é demonstrar como aplicar a arquitetura hexagonal em um sistema de gerenciamento de pedidos, mantendo a lógica de negócio isolada do framework, banco de dados e interfaces externas.

## 🎯 Objetivos do Projeto

- ✅ Implementar **Arquitetura Hexagonal** (Ports & Adapters)
- ✅ Aplicar **Domain Driven Design (DDD)**
- ✅ Criar uma **API REST** robusta e desacoplada
- ✅ Implementar **persistência desacoplada**
- ✅ Desenvolver **testes unitários** de qualidade
- ✅ Criar **mock adapters** para simulação
- ✅ Demonstrar boas práticas em arquitetura de software

## 🏗️ Estrutura do Projeto

```
OrderHub/
├── src/
│   ├── OrderHub.Application/       # Camada de Aplicação (casos de uso)
│   ├── OrderHub.Domain/            # Camada de Domínio (entidades, value objects)
│   ├── OrderHub.Infrastructure/    # Camada de Infraestrutura (BD, APIs externas)
│   ├── OrderHub.Presentation/      # Camada de Apresentação (API REST)
│   └── OrderHub.Ports/             # Contracts/Interfaces (Ports)
├── tests/
│   ├── OrderHub.Application.Tests/
│   ├── OrderHub.Domain.Tests/
│   └── OrderHub.Infrastructure.Tests/
├── OrderHub.sln
├── .gitignore
└── README.md
```

## 🚀 Começando

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) ou [Visual Studio Code](https://code.visualstudio.com/)
- [Git](https://git-scm.com/)

### Instalação

1. Clone o repositório:
   ```bash
   git clone https://github.com/seu-usuario/OrderHub.git
   cd OrderHub
   ```

2. Restaure as dependências:
   ```bash
   dotnet restore
   ```

3. Compile o projeto:
   ```bash
   dotnet build
   ```

4. Execute os testes:
   ```bash
   dotnet test
   ```

## 🏛️ Arquitetura Hexagonal

A **Arquitetura Hexagonal** (proposta por Alistair Cockburn) organiza o código em camadas:

### Núcleo (Domain)
- Contém a lógica de negócio pura
- Independente de frameworks, banco de dados ou qualquer tecnologia externa
- Entidades, Value Objects e Domain Services

### Portas (Ports)
- Interfaces que definem contratos de comunicação
- Entrada (Driving Ports): Controllers, gRPC
- Saída (Driven Ports): Repositories, External APIs

### Adaptadores (Adapters)
- Implementações concretas das Portas
- Convertem entre o domínio e tecnologias externas
- Exemplos: EntityFramework, HttpClient, WebSockets

## 📚 Estrutura de Camadas

```
┌─────────────────────────────────────────┐
│     Camada de Apresentação (REST API)   │
└──────────┬──────────────────────────────┘
           │ Adapters (Controllers)
┌──────────┴──────────────────────────────┐
│     Camada de Aplicação (Use Cases)     │
└──────────┬──────────────────────────────┘
           │ Ports & Adapters
┌──────────┴──────────────────────────────┐
│     Camada de Domínio (Business Logic)  │
└──────────┬──────────────────────────────┘
           │ Ports
┌──────────┴──────────────────────────────┐
│   Camada de Infraestrutura (DB, APIs)   │
└─────────────────────────────────────────┘
```

## 🧪 Testes

O projeto segue boas práticas de testes:

- **Testes Unitários**: Validam a lógica de negócio
- **Testes de Integração**: Validam a comunicação entre camadas
- **Mocks**: Simulam dependências externas

Execute os testes:
```bash
dotnet test
```

## 🔧 Ferramentas e Dependências

- **.NET 8**: Runtime
- **Entity Framework Core**: ORM
- **xUnit**: Framework de testes
- **Moq**: Mock library
- **FluentAssertions**: Assertions fluentes

## 📖 Recursos Adicionais

- [Padrão Hexagonal - Alistair Cockburn](https://alistair.cockburn.us/hexagonal-architecture/)
- [Domain Driven Design - Eric Evans](https://www.domainlanguage.com/ddd/)
- [.NET Documentation](https://docs.microsoft.com/dotnet/)

## 🤝 Contribuindo

Este é um projeto educacional. Contribuições são bem-vindas!

1. Fork o repositório
2. Crie uma branch para sua feature (`git checkout -b feature/amazing-feature`)
3. Commit suas alterações (`git commit -m 'Add amazing feature'`)
4. Push para a branch (`git push origin feature/amazing-feature`)
5. Abra um Pull Request

## 📝 Licença

Este projeto está licenciado sob a MIT License - veja o arquivo [LICENSE](LICENSE) para detalhes.

## 👤 Autor

**Alex Oliveira**

## 📧 Contato

Para dúvidas ou sugestões, entre em contato através do email ou abra uma issue no repositório.

---

**Desenvolvido com ❤️ como laboratório prático de Arquitetura Hexagonal em .NET 8**
