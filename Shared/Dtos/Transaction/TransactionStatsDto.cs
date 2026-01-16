
namespace Shared.Dtos.Transaction
{
    public class TransactionStatsDto
    {
        public decimal TotalEarnings { get; set; }
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public int AcceptedOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int RejectedOrders { get; set; }
        public decimal ThisMonthEarnings { get; set; }
        public decimal LastMonthEarnings { get; set; }
        public decimal EarningsGrowth { get; set; }
    }
}
