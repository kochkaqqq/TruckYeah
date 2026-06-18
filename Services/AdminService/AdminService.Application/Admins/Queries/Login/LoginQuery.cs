using AdminService.Application.Shared.Dtos;
using MediatR;

namespace AdminService.Application.Admins.Queries.Login
{
    public class LoginQuery : IRequest<LoginResponseDto>
    {
        public string Name { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
