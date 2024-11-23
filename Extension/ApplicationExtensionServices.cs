using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class ApplicationServiceExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, 
        IConfiguration config)
        {
            // Registering DbContext with SQLite connection string
            services.AddDbContext<DataContext>(options =>
                options.UseSqlite(config.GetConnectionString("DefaultConnection"))
            );

            // Registering CORS policy
            services.AddCors();

            // Registering the ITokenService and its implementation TokenService
            services.AddScoped<ITokenService, TokenService>();

            // Adding MVC controllers to the service container
            services.AddControllers();

            return services;
        }
    }
}
