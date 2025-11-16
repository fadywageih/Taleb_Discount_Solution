
namespace Services
{
    public class ServiceManager : IServiceManager
    {
        public ServiceManager(
            IVendorService vendorService,
            IAuthenticationService authenticationService)
        {
            VendorService = vendorService;
            AuthenticationService = authenticationService;
        }
        public IAuthenticationService AuthenticationService { get; }
        public IVendorService VendorService { get; }
    }
}