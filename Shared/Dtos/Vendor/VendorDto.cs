namespace Shared.Dtos.Vendor
{
    public class VendorDto
    {
        public Guid Id { get; set; }
        public string BusinessName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? Address2 { get; set; }
        public string? Website { get; set; }
        public string? FacebookUrl { get; set; }
        public string? LogoUrl { get; set; }
        public List<string> BusinessImages { get; set; } = new List<string>();
        public Guid UserId { get; set; }
        public List<BranchDto> Branches { get; set; } = new List<BranchDto>();
    }
}
