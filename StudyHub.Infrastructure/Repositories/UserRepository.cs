using Microsoft.EntityFrameworkCore;
using StudyHub.Core.Entities;
using StudyHub.Core.Interfaces;
using StudyHub.Infrastructure.Data;

namespace StudyHub.Infrastructure.Repositories;

public class UserRepository:IUserRepository
{
    private readonly ChatDbContext _context;

    public UserRepository(ChatDbContext context)
    {
        _context = context;
    }


    public Task<User?> GetByUsernameAsync(string userName)
        => _context.Users.SingleOrDefaultAsync(u => u.UserName == userName);

    public Task<User?> GetByIdAsync(Guid id)
        => _context.Users.SingleOrDefaultAsync(u => u.Id == id);

    public async Task AddAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }
}