using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StudyHub.Core.Constants;
using StudyHub.Core.Entities;
using StudyHub.Core.Security;

namespace StudyHub.Infrastructure.Data;

public static class SeedData
{
    public static async Task EnsureSeededAsync(IServiceProvider sp, IConfiguration config)
    {
        using var scope = sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ChatDbContext>();
        await db.Database.MigrateAsync();

        var adminUser = config["Admin:Username"] ?? "admin";
        var adminPass = config["Admin:Password"] ?? "admin123!";

        var admin = await db.Users.FirstOrDefaultAsync(u => u.UserName == adminUser);
        if (admin is null)
        {
            admin = new User
            {
                Id = Guid.NewGuid(),
                UserName = adminUser,
                PasswordHash = PasswordHasher.Hash(adminPass),
                Role = Roles.Admin
            };
            db.Users.Add(admin);
            await db.SaveChangesAsync();
        }

        if (!await db.Rooms.AnyAsync())
        {
            var room = new Room { Slug = "math101", Title = "Math 101", OwnerId = admin.Id };
            db.Rooms.Add(room);
            db.RoomMembers.Add(new RoomMember { RoomId = room.Id, UserId = admin.Id });
            await db.SaveChangesAsync();
        }
    }
}