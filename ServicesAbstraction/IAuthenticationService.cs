using Shared.Dtos;
using Shared.Dtos.User;

namespace ServicesAbstraction
{
    public interface IAuthenticationService
    {
        //public Task<UserResultDto> Login(LoginDto loginDto);
        Task<UserResultDto> RegisterSchool(SchoolRegisterDto dto);
        Task<UserResultDto> RegisterUniversity(UniversityRegisterDto dto);
        Task<UserResultDto> RegisterVendor(VendorRegisterDto dto);
        //Get current user
        public Task<UserResultDto> GetUserByEmail(string email);
        public Task<bool> CheckIfEmailExist(string email);
    }
}
