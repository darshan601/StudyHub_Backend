using Microsoft.EntityFrameworkCore;
using StudyHub.Core.Entities;
using StudyHub.Core.Interfaces;
using StudyHub.Infrastructure.Data;

namespace StudyHub.Infrastructure.Repositories;

public class ChatRepository:IChatRepository
{
    private ChatDbContext _context;

    public ChatRepository(ChatDbContext context)
    {
        this._context = context;
    }
    
    public async Task AddMessageAsync(ChatMessage message, CancellationToken cancellationToken = default)
    {
        _context.Messages.Add(message);
        await _context.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<ChatMessage>> GetRecentMessagesAsync(string roomId, int count = 50, CancellationToken cancellationToken = default)
    {
        return await _context.Messages
            .Where(m => m.RoomId == roomId)
            .OrderByDescending(m => m.TimeStamp)
            .Take(count)
            .ToListAsync(cancellationToken);
    }
}