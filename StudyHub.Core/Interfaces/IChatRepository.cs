using StudyHub.Core.Entities;

namespace StudyHub.Core.Interfaces;

public interface IChatRepository
{
    Task AddMessageAsync(ChatMessage message, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ChatMessage>> GetRecentMessagesAsync(string roomId, int count = 50,
        CancellationToken cancellationToken = default);
}