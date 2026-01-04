    using System.ComponentModel.DataAnnotations;

namespace Shared.Dtos.User
{
    public class VendorRegisterDto
    {
        [Required] public string BusinessName { get; set; }
        [Required, EmailAddress] public string Email { get; set; }
        [Required] public string Phone { get; set; }
        [Required] public string Description { get; set; }
        [Required] public string Address { get; set; }
        public string? Address2 { get; set; }
        [Required, DataType(DataType.Password)] public string Password { get; set; }
        [Required, DataType(DataType.Password), Compare("Password")] public string ConfirmPassword { get; set; }
        public string? Website { get; set; }
        public string? FacebookUrl { get; set; }
    }
}
