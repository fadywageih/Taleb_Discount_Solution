namespace Domain.Exceptions
{
    public sealed class TransactionNotFoundException : NotFoundException
    {
        public TransactionNotFoundException(Guid id)
            : base($"Transaction with id: {id} not found") { }

        public TransactionNotFoundException(string transactionNumber)
            : base($"Transaction with number: {transactionNumber} not found") { }
    }
}
