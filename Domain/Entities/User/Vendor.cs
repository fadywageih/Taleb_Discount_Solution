using Domain.Entities.Vendor;

namespace Domain.Entities.User
{
    public class Vendor : BaseEntity<Guid>
    {
        public string BusinessName { get; set; }  
        public string Description { get; set; }    
        public string Address { get; set; }       
        public string? Address2 { get; set; }
        public string? Website { get; set; }
        public string? FacebookUrl { get; set; }
        public string? LogoUrl { get; set; }
        public List<string> BusinessImages { get; set; } = new List<string>();
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }
        public virtual ICollection<Branch> Branches { get; set; } = new List<Branch>();

    }
}
