using AdminService.Application.Interfaces;
using AdminService.Application.Shared.Utils;
using AdminService.Domain.Entities;
using MediatR;

namespace AdminService.Application.Admins.Commands.CreateAdmin
{
    public class CreateAdminCommandHandler : IRequestHandler<CreateAdminCommand, Admin>
    {
        private readonly IDbContext _dbContext;

        public CreateAdminCommandHandler(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Admin> Handle(CreateAdminCommand request, CancellationToken cancellationToken)
        {
            var newAdmin = new Admin()
            {
                Name = request.Name,
                PasswordHash = PasswordHasher.Hash(request.Password),
            };

            await _dbContext.Admins.AddAsync(newAdmin, cancellationToken);
            await _dbContext.SaveChangesAsync();

            return newAdmin;
        }
    }
}
