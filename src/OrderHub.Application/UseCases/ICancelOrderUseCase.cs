namespace OrderHub.Application.UseCases;

/// <summary>
/// Use Case (Input Port) interface para cancelar um pedido
/// Define o contrato para implementações do caso de uso de cancelamento de pedidos
/// Representa um port na arquitetura hexagonal - interface de entrada da aplicação
/// Permite cancelar um pedido existente com motivo opcional e notificação
/// </summary>
public interface ICancelOrderUseCase
{
    /// <summary>
    /// Executa o caso de uso para cancelar um pedido existente
    /// Valida se o pedido está em um status que permite cancelamento
    /// Notifica cliente sobre o cancelamento
    /// </summary>
    /// <param name="orderId">Identificador único do pedido a ser cancelado</param>
    /// <param name="reason">Motivo do cancelamento (opcional)</param>
    /// <param name="cancellationToken">Token para cancelamento assíncrono da operação</param>
    /// <returns>Task completado quando o cancelamento foi processado</returns>
    /// <exception cref="ArgumentException">Se orderId estiver vazio ou nulo</exception>
    /// <exception cref="InvalidOperationException">Se pedido não existe ou está em status que não permite cancelamento</exception>
    Task ExecuteAsync(string orderId, string? reason = null, CancellationToken cancellationToken = default);
}
