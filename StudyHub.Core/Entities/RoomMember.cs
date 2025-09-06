namespace StudyHub.Core.Entities;

public class RoomMember
{
    public Guid RoomId { get; set; }
    public Guid UserId { get; set; }
 
    //nav props - optional
    public Room? Room { get; set; }
    public User? User { get; set; }
    
}