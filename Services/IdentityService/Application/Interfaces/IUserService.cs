using Application.Shared.Dtos.Requests;
using Application.Shared.Dtos.Responses;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<LoginDtoResponse> RegistrationUserAsync(RegistrationDtoRequest regDto); 
        Task<LoginDtoResponse> LoginUserAsync(LoginDtoRequest logDto);
        Task<LoginDtoResponse> RefreshTokenAsync(string refreshToken);
        Task<User> GetUserAsync(Guid id);
    }
}
