using Moq;
using OrderHub.Application.DTOs;
using OrderHub.Application.Ports;
using OrderHub.Domain.Aggregates.Order;

namespace OrderHub.UnitTests.Fixtures
{
    /// <summary>
    /// Fixture responsável por fornecer mocks pré-configurados de repositórios (Output Ports).
    /// Segue o padrão de Hexagonal Architecture onde repositórios são interfaces (portas)
    /// implementadas em outras camadas (adapters).
    /// Mocka Application.Ports.IOrderRepository (com métodos que usam string)
    /// </summary>
    public class RepositoryMockFixture
    {
        /// <summary>
        /// Mock pré-configurado de Application.Ports.IOrderRepository.
        /// Pode ser usado diretamente ou customizado nos testes.
        /// </summary>
    public Mock<IOrderRepository> OrderRepositoryMock { get; private set; }

    /// <summary>
    /// Cria e configura os mocks de repositório com comportamentos padrão.
    /// </summary>
    public RepositoryMockFixture()
    {
        OrderRepositoryMock = CreateOrderRepositoryMock();
    }

    /// <summary>
    /// Cria um mock de Application.Ports.IOrderRepository com setup básico.
    /// Por padrão, retorna null ao buscar uma ordem inexistente.
    /// </summary>
    private Mock<IOrderRepository> CreateOrderRepositoryMock()
    {
        var mock = new Mock<IOrderRepository>();

        //Setup padrão: GetByIdAsync retorna null se não configurado (Application layer version)
        mock.Setup(r => r.GetByIdAsync(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync((OrderResponse?)null);

            // Setup padrão: GetByCustomerIdAsync retorna lista vazia
            mock.Setup(r => r.GetByCustomerIdAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<OrderResponse>());

            // Setup padrão: SaveAsync não faz nada (void)
            mock.Setup(r => r.SaveAsync(
                It.IsAny<Order>(),
                It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Nota: DeleteAsync e ExistsAsync são definidas em Domain.Ports.IOrderRepository 
            // com parâmetros OrderId, portanto não são mockadas aqui

            return mock;
        }

        /// <summary>
        /// Configura o mock de repositório para retornar uma ordem específica quando buscada.
        /// </summary>
        /// <param name="orderId">ID da ordem a ser retornada (string)</param>
        /// <param name="orderResponse">Resposta OrderResponse a ser retornada</param>
        public void SetupOrderRepositoryGetById(string orderId, OrderResponse orderResponse)
        {
            OrderRepositoryMock
                .Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(orderResponse);
        }

        /// <summary>
        /// Configura o mock de repositório para retornar múltiplas ordens de um cliente.
        /// </summary>
        /// <param name="customerId">ID do cliente</param>
        /// <param name="orders">Lista de OrderResponse a ser retornada</param>
        public void SetupOrderRepositoryGetByCustomerId(string customerId, List<OrderResponse> orders)
        {
            OrderRepositoryMock
                .Setup(r => r.GetByCustomerIdAsync(customerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(orders);
        }

        /// <summary>
        /// Configura o mock de repositório para indicar que uma ordem existe.
        /// NOTE: Método comentado - ExistsAsync agora usa OrderId ValueObject, use Domain.Ports.IOrderRepository diretamente
        /// </summary>
        /// <param name="orderId">ID da ordem</param>
        /// <param name="exists">Se existe ou não</param>
        //public void SetupOrderRepositoryExists(string orderId, bool exists)
        //{
        //    OrderRepositoryMock
        //        .Setup(r => r.ExistsAsync(orderId, It.IsAny<CancellationToken>()))
        //        .ReturnsAsync(exists);
        //}

        /// <summary>
        /// Configura o mock de repositório para lançar uma exceção ao tentar buscar uma ordem.
        /// </summary>
        /// <param name="orderId">ID da ordem que causará exceção</param>
        /// <param name="exception">Exceção a ser lançada</param>
        public void SetupOrderRepositoryGetByIdThrows(string orderId, Exception exception)
        {
            OrderRepositoryMock
                .Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);
        }

        /// <summary>
        /// Configura o mock de repositório para lançar uma exceção ao tentar salvar uma ordem.
        /// </summary>
        /// <param name="exception">Exceção a ser lançada</param>
        public void SetupOrderRepositorySaveThrows(Exception exception)
        {
            OrderRepositoryMock
                .Setup(r => r.SaveAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);
        }

        /// <summary>
        /// Configura o mock de repositório para lançar uma exceção ao tentar deletar uma ordem.
        /// NOTE: Método comentado - DeleteAsync agora usa OrderId ValueObject, use Domain.Ports.IOrderRepository diretamente
        /// </summary>
        /// <param name="exception">Exceção a ser lançada</param>
        //public void SetupOrderRepositoryDeleteThrows(Exception exception)
        //{
        //    OrderRepositoryMock
        //        .Setup(r => r.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
        //        .ThrowsAsync(exception);
        //}

        /// <summary>
        /// Verifica se o repositório foi chamado para salvar uma ordem.
        /// </summary>
        /// <param name="times">Número de vezes esperado</param>
        public void VerifyOrderRepositorySaveWasCalled(Times times)
        {
            OrderRepositoryMock.Verify(
                r => r.SaveAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()),
                times);
        }

        /// <summary>
        /// Verifica se o repositório foi chamado para buscar uma ordem específica.
        /// </summary>
        /// <param name="orderId">ID da ordem</param>
        /// <param name="times">Número de vezes esperado</param>
        public void VerifyOrderRepositoryGetByIdWasCalled(string orderId, Times times)
        {
            OrderRepositoryMock.Verify(
                r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()),
                times);
        }

        /// <summary>
        /// Verifica se o repositório foi chamado para deletar uma ordem.
        /// NOTE: Método comentado - DeleteAsync agora usa OrderId ValueObject, use Domain.Ports.IOrderRepository diretamente
        /// </summary>
        /// <param name="times">Número de vezes esperado</param>
        //public void VerifyOrderRepositoryDeleteWasCalled(Times times)
        //{
        //    OrderRepositoryMock.Verify(
        //        r => r.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
        //        times);
        //}

        /// <summary>
        /// Verifica se o repositório nunca foi chamado para nenhuma operação.
        /// </summary>
        public void VerifyNoRepositoryCalls()
        {
            OrderRepositoryMock.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Reseta o mock de repositório para estado inicial.
        /// </summary>
        public void ResetOrderRepository()
        {
            OrderRepositoryMock.Reset();
            OrderRepositoryMock = CreateOrderRepositoryMock();
        }

        /// <summary>
        /// Retorna o objeto mock para customizações avançadas nos testes.
        /// </summary>
        public IOrderRepository GetOrderRepositoryInstance()
        {
            return OrderRepositoryMock.Object;
        }
    }
}
