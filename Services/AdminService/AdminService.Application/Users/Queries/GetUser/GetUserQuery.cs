using AdminService.Application.Shared.Dtos.Users;
using MediatR;

namespace AdminService.Application.Users.Queries.GetUser
{
    public class GetUserQuery : IRequest<UserResponseDto>
    {
        public Guid UserId { get; set; }
    }
}
