namespace Services
{
    public class ServiceManager : IServiceManager
    {
        private readonly IVendorService _vendorService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IProductService _productService;
        private readonly IHomeService _homeService;
        private readonly IFeedBackService _feedBackService;
        private readonly ITransactionService _transactionService;

        public ServiceManager(
            IVendorService vendorService,
            IAuthenticationService authenticationService,
            IProductService productService,
            IHomeService homeService,
            IFeedBackService feedBackService,
            ITransactionService transactionService)
        {
            _vendorService = vendorService;
            _authenticationService = authenticationService;
            _productService = productService;
            _homeService = homeService;
            _feedBackService = feedBackService;
            _transactionService = transactionService;
        }

        public IAuthenticationService AuthenticationService => _authenticationService;
        public IVendorService VendorService => _vendorService;
        public IProductService ProductService => _productService;
        public IHomeService HomeService => _homeService;
        public IFeedBackService FeedBackService => _feedBackService;
        public ITransactionService TransactionService => _transactionService;
    }
}