
    namespace Services
    {
        public class ServiceManager : IServiceManager
        {
            public ServiceManager(
                IVendorService vendorService,
                IAuthenticationService authenticationService,
                IProductService productService)
            {
                VendorService = vendorService;
                AuthenticationService = authenticationService;
                ProductService = productService;
            }
            public IAuthenticationService AuthenticationService { get; }
            public IVendorService VendorService { get; }
            public IProductService ProductService { get; }
        }
    }