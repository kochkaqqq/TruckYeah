using AdminService.Domain.Entities;
using MediatR;

namespace AdminService.Application.Admins.Commands.CreateAdmin
{
    public class CreateAdminCommand : IRequest<Admin>
    {
        public string Name { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
