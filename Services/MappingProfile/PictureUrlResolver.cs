using Domain.Entities.Product;
using Microsoft.Extensions.Configuration;
using Shared.Dtos.Product;

namespace Services.MappingProfile
{
    public class PictureUrlResolver : IValueResolver<Product, ProductResultDto, string> // تصحيح الإسم
    {
        private readonly IConfiguration _configuration;

        public PictureUrlResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Resolve(Product source, ProductResultDto destination, string destMember, ResolutionContext context)
        {
            if (string.IsNullOrWhiteSpace(source.PictureUrl))
                return string.Empty;

            var baseUrl = _configuration["BaseUrl"] ?? throw new InvalidOperationException("BaseUrl is not configured.");
            if (!baseUrl.EndsWith("/"))
                baseUrl += "/";

            var picturePath = source.PictureUrl.TrimStart('/');
            return $"{baseUrl}{picturePath}";
        }
    }
}