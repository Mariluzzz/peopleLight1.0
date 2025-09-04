using PeopleLight.Application.DTOs.Auth;

namespace PeopleLight.Application.Interfaces
{
    public interface IAuthRepository
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
        Task RegisterAsync(RegisterUserDto request);
    }
}
