using Microsoft.AspNetCore.Identity;

namespace Domain.Entities.User
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string? Name { get; set; } 
        public int? Age { get; set; }
        public string? NationalId { get; set; }
        public string UserType { get; set; }   // "School", "University", "Vendor"
    }
}
