using Domain.Entities.transcation;

namespace Domain.Contracts
{
    public interface ITransactionRepository
    {
        Task<Transaction?> GetByIdAsync(Guid id);
        Task<Transaction?> GetByTransactionNumberAsync(string transactionNumber);
        Task<IEnumerable<Transaction>> GetAllAsync(bool asNoTracking = false);
        Task<IEnumerable<Transaction>> GetAllWithDetailsAsync();
        Task<Transaction> AddAsync(Transaction transaction);
        Task UpdateAsync(Transaction transaction);
        Task<IEnumerable<Transaction>> GetVendorTransactionsAsync(Guid vendorId);
        Task<IEnumerable<Transaction>> GetVendorPendingTransactionsAsync(Guid vendorId);
        Task<(decimal TotalEarnings, int TotalOrders)> GetVendorStatsAsync(Guid vendorId);
        Task<IEnumerable<Transaction>> GetCustomerTransactionsAsync(Guid customerId);
        Task<IEnumerable<Transaction>> GetCustomerActiveTransactionsAsync(Guid customerId);
        Task<bool> AcceptTransactionAsync(Guid transactionId, Guid vendorId);
        Task<bool> RejectTransactionAsync(Guid transactionId, Guid vendorId, string reason);
        Task<bool> CompleteTransactionAsync(Guid transactionId);
        Task<string> GenerateUniqueTransactionNumberAsync();
        Task<int> GetTransactionCountByVendorAsync(Guid vendorId);
    }
}
