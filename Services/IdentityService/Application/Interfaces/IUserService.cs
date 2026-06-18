using Application.Shared.Dtos.Requests;
using Application.Shared.Dtos.Responses;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<LoginDtoResponse> RegistrationUserAsync(UserDtoRequest regDto); 
        Task<LoginDtoResponse> LoginUserAsync(LoginDtoRequest logDto);
        Task<LoginDtoResponse> RefreshTokenAsync(string refreshToken);
        Task<User> GetUserAsync(Guid id);
        Task<UserDtoResponse> UpdateUserAsync(UpdateUserRequestDto request);
        Task DeleteUserAsync(Guid userId);
        Task<ICollection<UserDtoResponse>> GetUsersAsync();
    }
}
