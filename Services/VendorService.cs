namespace Services
{
    public class VendorService : IVendorService
    {
        private readonly IVendorRepository _vendorRepository;
        private readonly IImageService _imageService;
        private readonly IMapper _mapper;
        public VendorService(IVendorRepository vendorRepository, IImageService imageService, IMapper mapper)
        {
            _vendorRepository = vendorRepository;
            _imageService = imageService;
            _mapper = mapper;
        }
        public async Task<VendorDto?> GetVendorByIdAsync(Guid id)
        {
            var vendor = await _vendorRepository.GetVendorByIdAsync(id);
            return _mapper.Map<VendorDto>(vendor);
        }
        public async Task<VendorDto?> GetVendorByUserIdAsync(Guid userId)
        {
            var vendor = await _vendorRepository.GetVendorByUserIdAsync(userId);
            return _mapper.Map<VendorDto>(vendor);
        }
        public async Task<VendorDto?> GetVendorByEmailAsync(string email)
        {
            var vendor = await _vendorRepository.GetVendorByEmailAsync(email);
            return _mapper.Map<VendorDto>(vendor);
        }
        public async Task<VendorDto?> UpdateVendorAsync(Guid id, UpdateVendorDto vendorDto)
        {
            var vendor = await _vendorRepository.GetVendorByIdAsync(id);
            if (vendor == null)
                return null;

            UpdateVendorData(vendor, vendorDto);
            await UpdateBusinessImages(vendor, vendorDto);
            UpdateBranches(vendor, vendorDto);

            var updatedVendor = await _vendorRepository.UpdateVendorAsync(id, vendor);
            return _mapper.Map<VendorDto>(updatedVendor);
        }
        private void UpdateVendorData(Vendor vendor, UpdateVendorDto vendorDto)
        {
            vendor.BusinessName = vendorDto.BusinessName;
            vendor.Description = vendorDto.Description;
            vendor.Address = vendorDto.Address;
            vendor.Address2 = vendorDto.Address2;
            vendor.Website = vendorDto.Website;
            vendor.FacebookUrl = vendorDto.FacebookUrl;
            vendor.LogoUrl = vendorDto.LogoUrl;
        }
        private async Task UpdateBusinessImages(Vendor vendor, UpdateVendorDto vendorDto)
        {
            if (vendorDto.BusinessImages == null || !vendorDto.BusinessImages.Any())
                return;

            var savedImageUrls = new List<string>();

            foreach (var imageData in vendorDto.BusinessImages.Take(5))
            {
                if (string.IsNullOrEmpty(imageData))
                    continue;

                var imageUrl = imageData.StartsWith("data:image")
                    ? await _imageService.SaveBase64ImageAsync(imageData)
                    : imageData;

                if (!string.IsNullOrEmpty(imageUrl))
                    savedImageUrls.Add(imageUrl);
            }
            vendor.BusinessImages = savedImageUrls;
        }
        private void UpdateBranches(Vendor vendor, UpdateVendorDto vendorDto)
        {
            if (vendorDto.Branches == null)
                return;

            vendor.Branches.Clear();
            foreach (var branchDto in vendorDto.Branches)
            {
                vendor.Branches.Add(new Branch
                {
                    Address = branchDto.Address,
                    City = branchDto.City,
                    State = branchDto.State,
                    PhoneNumber = branchDto.PhoneNumber
                });
            }
        }
    }
}