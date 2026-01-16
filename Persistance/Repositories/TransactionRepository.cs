using Domain.Contracts;
using Domain.Entities.transcation;
using Microsoft.EntityFrameworkCore;
using Persistance.Data;

namespace Persistance.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly ApplicationDbContext _context;

        public TransactionRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Transaction?> GetByIdAsync(Guid id)
        {
            return await _context.Transactions
                .Include(t => t.Product)
                .Include(t => t.Vendor)
                .Include(t => t.Customer)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Transaction?> GetByTransactionNumberAsync(string transactionNumber)
        {
            return await _context.Transactions
                .Include(t => t.Product)
                .Include(t => t.Vendor)
                .Include(t => t.Customer)
                .FirstOrDefaultAsync(t => t.TransactionNumber == transactionNumber);
        }

        public async Task<IEnumerable<Transaction>> GetAllAsync(bool asNoTracking = false)
        {
            var query = _context.Transactions
                .Include(t => t.Product)
                .Include(t => t.Vendor)
                .Include(t => t.Customer)
                .AsQueryable();

            if (asNoTracking)
                query = query.AsNoTracking();

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetAllWithDetailsAsync()
        {
            return await _context.Transactions
                .Include(t => t.Product)
                .Include(t => t.Vendor)
                .Include(t => t.Customer)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }

        public async Task<Transaction> AddAsync(Transaction transaction)
        {
            await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task UpdateAsync(Transaction transaction)
        {
            _context.Transactions.Update(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Transaction>> GetVendorTransactionsAsync(Guid vendorId)
        {
            return await _context.Transactions
                .Include(t => t.Product)
                .Include(t => t.Customer)
                .Where(t => t.VendorId == vendorId)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetCustomerTransactionsAsync(Guid customerId)
        {
            return await _context.Transactions
                .Include(t => t.Product)
                .Include(t => t.Vendor)
                .Where(t => t.CustomerId == customerId)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }

        public async Task<bool> AcceptTransactionAsync(Guid transactionId, Guid vendorId)
        {
            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == transactionId && t.VendorId == vendorId && t.Status == TransactionStatus.Pending);

            if (transaction == null) return false;

            transaction.Status = TransactionStatus.Accepted;
            transaction.AcceptedDate = DateTime.UtcNow;
            transaction.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectTransactionAsync(Guid transactionId, Guid vendorId, string reason)
        {
            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == transactionId && t.VendorId == vendorId && t.Status == TransactionStatus.Pending);

            if (transaction == null) return false;

            transaction.Status = TransactionStatus.Rejected;
            transaction.RejectionReason = reason;
            transaction.RejectedDate = DateTime.UtcNow;
            transaction.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<string> GenerateUniqueTransactionNumberAsync()
        {
            var prefix = "TRX";
            var year = DateTime.UtcNow.Year;
            var month = DateTime.UtcNow.Month.ToString("D2");

            string transactionNumber;
            bool isUnique;

            do
            {
                var random = new Random();
                var sequence = random.Next(10000, 99999).ToString();
                transactionNumber = $"{prefix}-{year}{month}-{sequence}";

                isUnique = !await _context.Transactions.AnyAsync(t => t.TransactionNumber == transactionNumber);
            } while (!isUnique);

            return transactionNumber;
        }

        public async Task<(decimal TotalEarnings, int TotalOrders)> GetVendorStatsAsync(Guid vendorId)
        {
            var transactions = await _context.Transactions
                .Where(t => t.VendorId == vendorId && t.Status == TransactionStatus.Completed)
                .ToListAsync();

            var totalEarnings = transactions.Sum(t => t.VendorEarnings);
            var totalOrders = transactions.Count;

            return (totalEarnings, totalOrders);
        }

        public async Task<IEnumerable<Transaction>> GetVendorPendingTransactionsAsync(Guid vendorId)
        {
            return await _context.Transactions
                .Where(t => t.VendorId == vendorId && t.Status == TransactionStatus.Pending)
                .ToListAsync();
        }

        public async Task<bool> CompleteTransactionAsync(Guid transactionId)
        {
            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == transactionId && t.Status == TransactionStatus.Accepted);

            if (transaction == null) return false;

            transaction.Status = TransactionStatus.Completed;
            transaction.CompletedDate = DateTime.UtcNow;
            transaction.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Transaction>> GetCustomerActiveTransactionsAsync(Guid customerId)
        {
            var activeStatuses = new[] { TransactionStatus.Pending, TransactionStatus.Accepted };
            return await _context.Transactions
                .Where(t => t.CustomerId == customerId && activeStatuses.Contains(t.Status))
                .ToListAsync();
        }

        public async Task<int> GetTransactionCountByVendorAsync(Guid vendorId)
        {
            return await _context.Transactions
                .CountAsync(t => t.VendorId == vendorId);
        }

        // الـ CancelByCustomerAsync مش محتاجينه دلوقتي لأننا عملنا الـ Update مباشرة في الـ Service
    }
}