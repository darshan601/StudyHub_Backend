using StudyHub.Core.Constants;
using StudyHub.Core.DTOs;
using StudyHub.Core.Entities;
using StudyHub.Core.Interfaces;

namespace StudyHub.Core.Services;

public class RoomService
{
    private readonly IRoomRepository _roomRepository;
    private readonly IUserRepository _userRepository;

    public RoomService(IRoomRepository rooms, IUserRepository users)
    {
        _roomRepository = rooms;
        _userRepository = users;
    }

    public async Task<RoomDto> CreateRoomAsync(Guid ownerId, CreateRoomRequest request)
    {
        var existing = await _roomRepository.GetBySlugAsync(request.Slug);
        if (existing is not null) throw new InvalidOperationException("Slug already in use");

        var room = new Room { Slug = request.Slug, Title = request.Title, OwnerId = ownerId };
        await _roomRepository.AddAsync(room);
        await _roomRepository.AddMemberAsync(room.Id, ownerId);

        return new RoomDto(room.Id, room.Slug, room.Title, room.OwnerId);
    }

    public async Task JoinAsync(Guid roomId, Guid userId)
    {
        var room = await _roomRepository.GetByIdAsync(roomId) ?? throw new KeyNotFoundException("Room not Found");
        var isMember = await _roomRepository.IsMemberAsync(roomId, userId);
        if (!isMember) await _roomRepository.AddMemberAsync(roomId, userId);
        
    }

    public Task LeaveAsync(Guid roomId, Guid userId) => _roomRepository.RemoveMemberAsync(roomId, userId);

    public Task<bool> IsMemberAsync(Guid roomId, Guid userId) => _roomRepository.IsMemberAsync(roomId, userId);

    public async Task<bool> IsOwnerOrAdminAsync(Guid roomId, Guid userId)
    {
        var room = await _roomRepository.GetByIdAsync(roomId);
        if (room is null) return false;
        
        if(room.OwnerId == userId) return true;
        var user = await _userRepository.GetByIdAsync(userId);

        return user?.Role == Roles.Admin;
    }

    public async Task<IReadOnlyList<RoomDto>> GetRoomsForUserAsync(Guid userId)
    {
        var rooms = await _roomRepository.GetRoomsForUserAsync(userId);

        return rooms.Select(r => new RoomDto(r.Id, r.Slug, r.Title, r.OwnerId)).ToList();
    }
}