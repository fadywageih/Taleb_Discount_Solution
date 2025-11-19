
using RealState.BLL.Common.Services.AttachmentService;
using Services;
using ServicesAbstraction;
using Shared;

namespace Taleb_Discount.Extentions
{
    public static class CoreServicesExstentions
    {
        //bulider.Services
        public static IServiceCollection AddCoreServices(this IServiceCollection Services, IConfiguration configuration)
        {
            Services.AddScoped<IAttachmentService, AttachmentService>();
            Services.AddScoped<IServiceManager, ServiceManager>();
            Services.AddScoped<IEmailService, EmailService>();
            Services.AddScoped<IVendorService, VendorService>();
            Services.AddScoped<IProductService, ProductService>();
            Services.AddScoped<IAuthenticationService, AuthenticationService>();
            Services.AddScoped<IExtendedImageService, ImageService>();
            Services.AddScoped<IImageService>(provider => provider.GetRequiredService<IExtendedImageService>());

            Services.AddAutoMapper(typeof(Services.AssemblyReference).Assembly);
            Services.Configure<JwtOptions>(configuration.GetSection("JwtOptions"));
            return Services;
        }
    }
}
