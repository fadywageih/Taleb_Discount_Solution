using Domain.Contracts;
using Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using Persistance.Data;

namespace Persistance.Repositories
{
    public class VendorRepository:IVendorRepository
    {
        
            private readonly ApplicationDbContext _context;

            public VendorRepository(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<Vendor?> GetVendorByIdAsync(Guid id)
            {
                return await _context.Vendors
                    .Include(v => v.User)
                    .Include(v => v.Branches)
                    .FirstOrDefaultAsync(v => v.Id == id);
            }

            public async Task<Vendor?> GetVendorByUserIdAsync(Guid userId)
            {
                return await _context.Vendors
                    .Include(v => v.User)
                    .Include(v => v.Branches)
                    .FirstOrDefaultAsync(v => v.UserId == userId);
            }

            public async Task<Vendor?> GetVendorByEmailAsync(string email)
            {
                return await _context.Vendors
                    .Include(v => v.User)
                    .Include(v => v.Branches)
                    .FirstOrDefaultAsync(v => v.User.Email == email);
            }

            public async Task<Vendor?> UpdateVendorAsync(Guid id, Vendor vendor)
            {
                var existingVendor = await _context.Vendors
                    .Include(v => v.Branches)
                    .FirstOrDefaultAsync(v => v.Id == id);

                if (existingVendor == null) return null;

                // Update properties
                existingVendor.BusinessName = vendor.BusinessName;
                existingVendor.Description = vendor.Description;
                existingVendor.Address = vendor.Address;
                existingVendor.Address2 = vendor.Address2;
                existingVendor.Website = vendor.Website;
                existingVendor.FacebookUrl = vendor.FacebookUrl;
                existingVendor.LogoUrl = vendor.LogoUrl;
                existingVendor.BusinessImages = vendor.BusinessImages;

                // Update branches
                _context.Branches.RemoveRange(existingVendor.Branches);
                foreach (var branch in vendor.Branches)
                {
                    branch.VendorId = existingVendor.Id;
                    await _context.Branches.AddAsync(branch);
                }

                await _context.SaveChangesAsync();
                return existingVendor;
            }
        }
}
