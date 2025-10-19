using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Shared.Dtos.User
{
    public class SchoolRegisterDto
    {
        [Required] public string? Name { get; set; }
        [Required, EmailAddress] public string? Email { get; set; }
        [Required, DataType(DataType.Password)] public string Password { get; set; }
        [Required, DataType(DataType.Password), Compare("Password")] public string ConfirmPassword { get; set; }
        [Required] public string Address { get; set; }
        [Required] public string? NationalId { get; set; }
        [Required] public int Age { get; set; }
        [Required] public int Level { get; set; }
        [Required] public string SchoolName { get; set; }
        [Required] public string Phone { get; set; }
        [Required] public IFormFile? BirthCertificateFile { get; set; }
    }
}
