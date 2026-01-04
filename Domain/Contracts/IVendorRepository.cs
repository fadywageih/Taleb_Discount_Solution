using Domain.Entities.User;

namespace Domain.Contracts
{
    public interface IVendorRepository
    {
        Task<Vendor?> GetVendorByIdAsync(Guid id);
        Task<Vendor?> GetVendorByUserIdAsync(Guid userId);
        Task<Vendor?> GetVendorByEmailAsync(string email);
        Task<Vendor?> UpdateVendorAsync(Guid id, Vendor vendor);
    }
}
