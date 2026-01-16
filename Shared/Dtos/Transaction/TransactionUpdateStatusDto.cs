namespace Shared.Dtos.Transaction
{
    public class TransactionUpdateStatusDto
    {
        public Guid TransactionId { get; set; }
        public string? RejectionReason { get; set; }
    }
}
