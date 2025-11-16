using Shared.Dtos.Vendor;

namespace ServicesAbstraction
{
    public interface IVendorService
    {
        Task<VendorDto?> GetVendorByIdAsync(Guid id);
        Task<VendorDto?> GetVendorByUserIdAsync(Guid userId);
        Task<VendorDto?> GetVendorByEmailAsync(string email);
        Task<VendorDto?> UpdateVendorAsync(Guid id, UpdateVendorDto vendorDto);
    }
}
