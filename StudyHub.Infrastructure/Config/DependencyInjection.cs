using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StudyHub.Core.Interfaces;
using StudyHub.Core.Services;
using StudyHub.Infrastructure.Data;
using StudyHub.Infrastructure.Repositories;

namespace StudyHub.Infrastructure.Config;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<ChatDbContext>(options =>
            options.UseNpgsql(config.GetConnectionString("DefaultConnection")));

        services.AddScoped<IChatRepository, ChatRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoomRepository, RoomRepository>();
        
        //app services
        services.AddScoped<ChatService>();
        services.AddScoped<AuthService>();
        services.AddScoped<RoomService>();
        
        return services;
    }
}