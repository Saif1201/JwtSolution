using JwtExampleProject.ApiResponses;
using JwtExampleProject.DTOs;

namespace JwtExampleProject.Repositories.Interfaces
{
    public interface IUserRepo
    {
        Task<RegisterUserResponse> RegisterUserAsync(RegisterUserDto registerUserDto);

        Task<LoginUserResponse> LoginUserAsync(LoginUserDto loginUserDto);
    }
}
