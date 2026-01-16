using Shared.Dtos.Transaction;
namespace ServicesAbstraction
{
    public interface ITransactionService
    {
        Task<TransactionDto> CreateTransactionAsync(TransactionCreateDto dto);
        Task<TransactionDto> GetTransactionByIdAsync(Guid id);
        Task<IEnumerable<TransactionDto>> GetVendorTransactionsAsync(Guid vendorId);
        Task<IEnumerable<TransactionDto>> GetCustomerTransactionsAsync(Guid customerId);
        Task<TransactionDto> AcceptTransactionAsync(Guid transactionId);
        Task<TransactionDto> RejectTransactionAsync(Guid transactionId, string reason);
        Task<TransactionStatsDto> GetVendorStatsAsync(Guid vendorId);
        Task<TransactionDto> CancelTransactionAsync(Guid transactionId);
    }
}
