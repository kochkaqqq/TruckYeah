using AdminService.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AdminService.Infastructure
{
    public static class DependencyInjection
    {
        public static void InjectInfrastructure(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<ApplicationContext>(options =>
                options.UseNpgsql(connectionString));
            services.AddScoped<IDbContext, ApplicationContext>();
        }
    }
}
