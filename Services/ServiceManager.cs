
    namespace Services
    {
        public class ServiceManager : IServiceManager
        {
            public ServiceManager(
                IVendorService vendorService,
                IAuthenticationService authenticationService,
                IProductService productService,
                IHomeService homeService)
            {
                VendorService = vendorService;
                AuthenticationService = authenticationService;
                ProductService = productService;
                HomeService = homeService;
            }
            public IAuthenticationService AuthenticationService { get; }
            public IVendorService VendorService { get; }
            public IProductService ProductService { get; }
            public IHomeService  HomeService { get; }
        }
    }