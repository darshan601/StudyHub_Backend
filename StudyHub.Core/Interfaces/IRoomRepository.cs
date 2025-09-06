using StudyHub.Core.Entities;

namespace StudyHub.Core.Interfaces;

public interface IRoomRepository
{
    Task<Room?> GetByIdAsync(Guid id);

    Task<Room?> GetBySlugAsync(string slug);

    Task AddAsync(Room room);

    Task<bool> IsMemberAsync(Guid roomId, Guid userId);

    Task AddMemberAsync(Guid roomId, Guid userId);

    Task RemoveMemberAsync(Guid roomId, Guid userId);

    Task<IReadOnlyList<Room>> GetRoomsForUserAsync(Guid userId);
}