using AdminService.Application.Shared.Dtos.Users;
using MediatR;

namespace AdminService.Application.Users.Queries.GetUsers
{
    public class GetUsersQuery : IRequest<ICollection<UserResponseDto>>
    {
    }
}
