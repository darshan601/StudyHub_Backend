namespace StudyHub.Core.Entities;

public class Room
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Slug { get; set; } = default!;
    public string Title { get; set; } = default!;
    public Guid OwnerId { get; set; }

    public List<RoomMember> Members { get; set; } = new();
}