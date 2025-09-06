using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using StudyHub.Core.Services;

namespace Presentation.Hubs;

[Authorize]
public class ChatHub:Hub
{
    private readonly ChatService _chatService;
    private readonly RoomService _roomService;

    public ChatHub(ChatService chatService, RoomService roomService)
    {
        _chatService = chatService;
        _roomService = roomService;
    }

    private Guid CurrentUserId => Guid.Parse(Context.User!.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    public async Task JoinRoom(string roomId)
    {
        if (!Guid.TryParse(roomId, out var rid)) throw new HubException("Invalid Room");
        var uid = CurrentUserId;

        var isMember = await _roomService.IsMemberAsync(rid, uid);
        if (!isMember) await _roomService.JoinAsync(rid, uid);
        
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
        await Clients.Group(roomId).SendAsync("SystemMessage", $"{Context.User!.Identity!.Name} joined {roomId}");
    }

    public async Task LeaveRoom(string roomId)
    {
        if (!Guid.TryParse(roomId, out var rid)) throw new HubException("Invalid RoomId");
        var uid = CurrentUserId;

        // await _roomService.LeaveAsync(rid, uid);

        try
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
            await Clients.Group(roomId).SendAsync("SystemMessage", $"{Context.User!.Identity!.Name} left {roomId}");
        }
        catch (TaskCanceledException e)
        {
            Console.WriteLine($"Redis timeout when leaving room {roomId} for connection {Context.ConnectionId}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error leaving room {roomId} : Error : {e.Message}");
            throw;
        }
        
    }

    public async Task SendMessage(string roomId, string content)
    {
        if (!Guid.TryParse(roomId, out var rid)) throw new HubException("Invalid RoomId");
        var uid = CurrentUserId;

        var isMember = await _roomService.IsMemberAsync(rid, uid);
        if (!isMember) throw new HubException("Not a room member");
        
        // var userId = Context.User!.Identity!.Name!;
        
        var dto = await _chatService.SendMessageAsync(roomId, uid.ToString(), content);
        await Clients.Group(roomId).SendAsync("ReceiveMessage", dto);
    }

    // public override Task OnConnectedAsync() => base.OnConnectedAsync();

    public async Task GetHistory(string roomId, int count = 50)
    {
        var history = await _chatService.GetHistoryAsync(roomId, count);
        await Clients.Caller.SendAsync("MessageHistory", history);
    }
}