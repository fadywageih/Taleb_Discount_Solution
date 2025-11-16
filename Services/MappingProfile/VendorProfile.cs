using Domain.Entities.Vendor;
using Shared.Dtos.Vendor;

namespace Services.MappingProfile
{
    public class VendorProfile: Profile
    {
        public VendorProfile()
        {
            CreateMap<Vendor, VendorDto>().ReverseMap();
            CreateMap<UpdateVendorDto, Vendor>();
            CreateMap<Branch, BranchDto>().ReverseMap();
        }
    }
}
