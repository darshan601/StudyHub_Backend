using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyHub.Core.Constants;
using StudyHub.Core.DTOs;
using StudyHub.Core.Services;

namespace Presentation.Controllers;



[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RoomsController:ControllerBase
{
    private readonly RoomService _roomService;

    public RoomsController(RoomService roomService) => _roomService = roomService;

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<RoomDto>> Create([FromBody] CreateRoomRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var dto = await _roomService.CreateRoomAsync(userId, request);
        return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RoomDto>> Get([FromRoute] Guid id)
    {
        //optional return roominfo; keep minimal here
        return Ok(new { Id = id });
    }

    [HttpPost("{id:guid}/join")]
    public async Task<IActionResult> Join([FromRoute] Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _roomService.JoinAsync(id, userId);
        return NoContent();
    }

    [HttpPost("{id:guid}/leave")]
    public async Task<IActionResult> Leave([FromRoute] Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _roomService.LeaveAsync(id, userId);
        return NoContent();
    }

    [HttpGet("mine")]
    public async Task<IActionResult> MyRooms()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var list = await _roomService.GetRoomsForUserAsync(userId);

        return Ok(list);
    }
    
    
}