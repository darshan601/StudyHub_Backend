using Microsoft.EntityFrameworkCore;
using StudyHub.Core.Entities;
using StudyHub.Core.Interfaces;
using StudyHub.Infrastructure.Data;

namespace StudyHub.Infrastructure.Repositories;

public class RoomRepository:IRoomRepository
{
    private readonly ChatDbContext _context;
    public RoomRepository(ChatDbContext context) => _context = context;

    public Task<Room?> GetByIdAsync(Guid id)
        => _context.Rooms.FirstOrDefaultAsync(r => r.Id == id);

    public Task<Room?> GetBySlugAsync(string slug)
        => _context.Rooms.FirstOrDefaultAsync(r => r.Slug == slug);

    public async Task AddAsync(Room room)
    {
        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();
    }

    public Task<bool> IsMemberAsync(Guid roomId, Guid userId)
        => _context.RoomMembers.AnyAsync(m => m.RoomId == roomId && m.UserId == userId);

    public async Task AddMemberAsync(Guid roomId, Guid userId)
    {
        _context.RoomMembers.Add(new RoomMember { RoomId = roomId, UserId = userId });
        await _context.SaveChangesAsync();
    }

    public async Task RemoveMemberAsync(Guid roomId, Guid userId)
    {
        var entity = await _context.RoomMembers.FirstOrDefaultAsync(m=> m.RoomId == roomId && m.UserId == userId);

        if (entity is not null)
        {
            _context.RoomMembers.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IReadOnlyList<Room>> GetRoomsForUserAsync(Guid userId)
    {
        var roomIds = await _context.RoomMembers.Where(m => m.UserId == userId)
            .Select(m => m.RoomId).ToListAsync();

        return await _context.Rooms.Where(r => roomIds.Contains(r.Id)).ToListAsync();
    }
}