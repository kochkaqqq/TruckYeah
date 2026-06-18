using Application.Shared.Dtos.Requests;
using Application.Shared.Dtos.Responses;
namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<LoginDtoResponse> RegistrationUserAsync(RegistrationDtoRequest regDto); 
        Task<LoginDtoResponse> LoginUserAsync(LoginDtoRequest logDto);
        Task<LoginDtoResponse> RefreshTokenAsync(string refreshToken);
        Task<UserProfileResponse> GetCurrentUserAsync(Guid id);
        Task<UserProfileResponse> UpdateCurrentUserAsync(Guid id, UpdateUserProfileRequest request);
        Task<PublicUserResponse> GetPublicUserAsync(Guid id);
        Task LogoutAsync(string refreshToken);
        Task<IReadOnlyList<AdminUserResponse>> GetUsersAsync();
        Task BlockUserAsync(Guid id);
        Task UnblockUserAsync(Guid id);
    }
}
