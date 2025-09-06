namespace StudyHub.Core.Entities;

public class ChatMessage
{
    public Guid Id { get; set; }
    
    public string RoomId { get; set; }
    
    public string UserId { get; set; }
    
    public string Content { get; set; }
    
    public DateTime TimeStamp { get; set; }
}