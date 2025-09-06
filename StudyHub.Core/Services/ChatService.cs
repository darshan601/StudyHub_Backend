using StudyHub.Core.DTOs;
using StudyHub.Core.Entities;
using StudyHub.Core.Interfaces;

namespace StudyHub.Core.Services;

public class ChatService
{
    private IChatRepository _chatRepository;

    public ChatService(IChatRepository chatRepository)
    {
        this._chatRepository = chatRepository;
    }


    public async Task<ChatMessageDto> SendMessageAsync(string roomId, string userId, string content)
    {
        var entity = new ChatMessage()
        {
            Id = Guid.NewGuid(),
            RoomId = roomId,
            UserId = userId,
            Content = content,
            TimeStamp = DateTime.UtcNow
        };

        await _chatRepository.AddMessageAsync(entity);

        return new ChatMessageDto(entity.Id, entity.RoomId, entity.UserId, entity.Content, entity.TimeStamp);
    }

    public async Task<IReadOnlyList<ChatMessageDto>> GetHistoryAsync(string roomId, int count=50)
    {
        var messages = await _chatRepository.GetRecentMessagesAsync(roomId, count);
        
        return messages.
            OrderByDescending(m=>m.TimeStamp)
            .Select(m=> new ChatMessageDto(m.Id, m.RoomId, m.UserId, m.Content, m.TimeStamp))
            .ToList();
    }
}