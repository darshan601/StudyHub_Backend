using StudyHub.Core.Entities;

namespace StudyHub.Core.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string userName);

    Task<User?> GetByIdAsync(Guid id);

    Task AddAsync(User user);
}