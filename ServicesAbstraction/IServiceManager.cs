namespace ServicesAbstraction
{
    public interface IServiceManager
    {
        public IAuthenticationService AuthenticationService { get; }
        public IVendorService VendorService { get; }
        public IProductService ProductService { get; }
        public IHomeService HomeService { get; }
        public IFeedBackService FeedBackService { get; }
        public ITransactionService TransactionService { get; }
    }
}
