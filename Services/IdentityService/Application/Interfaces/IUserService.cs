using Application.Shared.Dtos.Requests;
using Application.Shared.Dtos.Responses;

namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<LoginDtoResponse> RegistrationUserAsync(RegistrationDtoRequest regDto); 
        Task<LoginDtoResponse> LoginUserAsync(LoginDtoRequest logDto);
        Task<LoginDtoResponse> RefreshTokenAsync(string refreshToken);
    }
}
