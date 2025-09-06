namespace StudyHub.Core.DTOs;


// can use automapper to convert dto to entity
public record ChatMessageDto(
    Guid Id,
    string RoomId,
    string UserId,
    string Content,
    DateTime TimeStamp);