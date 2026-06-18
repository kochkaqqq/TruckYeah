using MediatR;

namespace AdminService.Application.Admins.Queries.Refresh
{
    public class RefreshQuery : IRequest<string>
    {
        public string RefreshToken { get; set; } = null!;
    }
}
