using Microsoft.EntityFrameworkCore;
using StudyHub.Core.Entities;

namespace StudyHub.Infrastructure.Data;

public class ChatDbContext:DbContext
{
    public ChatDbContext(DbContextOptions<ChatDbContext> options):base(options){}
    
    public DbSet<ChatMessage> Messages => Set<ChatMessage>();

    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<RoomMember> RoomMembers => Set<RoomMember>();

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChatMessage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RoomId).IsRequired().HasMaxLength(100);
            entity.Property(e => e.UserId).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Content).IsRequired().HasMaxLength(2000);
            entity.Property(e => e.TimeStamp).IsRequired();
            entity.HasIndex(e => new { e.RoomId, e.TimeStamp });
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.UserName).IsRequired().HasMaxLength(100);
            entity.Property(x => x.PasswordHash).IsRequired();
            entity.Property(x => x.Role).IsRequired().HasMaxLength(32);
            entity.HasIndex(e => e.UserName).IsUnique();
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Slug).IsRequired().HasMaxLength(100);
            entity.Property(x => x.Title).IsRequired().HasMaxLength(200);
            entity.HasIndex(x => x.Slug).IsUnique();
        });

        modelBuilder.Entity<RoomMember>(entity =>
        {
            entity.HasKey(x => new { x.RoomId, x.UserId });
            entity.HasOne(x => x.Room).WithMany(r => r.Members).HasForeignKey(x => x.RoomId);
            entity.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
        });
        
        

    }
}