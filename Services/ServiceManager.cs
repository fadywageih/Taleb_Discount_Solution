

namespace Services
{
    public class ServiceManager(IAuthenticationService authenticationService) : IServiceManager
    {
        public IAuthenticationService AuthenticationService { get; } = authenticationService;
    }
}
