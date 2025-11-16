namespace Shared.Dtos.Vendor
{
    public class BranchDto
    {
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
    }
}
