using Shared.Dtos.User.Shared.Dtos.Home;

namespace ServicesAbstraction
{
    public interface IHomeService
    {
        Task<HomePageDto> GetHomePageDataAsync();
        Task<HomePageDto> GetHomePageDataForStudentAsync(string userType);
    }
}
