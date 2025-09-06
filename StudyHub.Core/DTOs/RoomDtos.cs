namespace StudyHub.Core.DTOs;

public record CreateRoomRequest(string Slug, string Title);

public record RoomDto(Guid Id, string Slug, string Title, Guid OwnerId);