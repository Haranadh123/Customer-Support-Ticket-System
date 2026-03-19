using System.Security.Claims;
using CustomerSupport.API.Data;
using CustomerSupport.Shared.DTOs;
using CustomerSupport.Shared.Enums;
using CustomerSupport.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CustomerSupport.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly AppDbContext _context;

    public CommentsController(AppDbContext context)
    {
        _context = context;
    }

    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    private bool IsAdmin() => User.IsInRole(UserRole.Admin.ToString());

    [HttpPost("{ticketId}")]
    public async Task<IActionResult> AddComment(int ticketId, AddCommentRequest request)
    {
        var userId = GetUserId();
        var isAdmin = IsAdmin();

        var ticket = await _context.Tickets.FindAsync(ticketId);
        if (ticket == null) return NotFound();

        if (ticket.Status == TicketStatus.Closed && !isAdmin)
        {
            return BadRequest("Cannot add comments to a closed ticket.");
        }

        if (request.IsInternal && !isAdmin)
        {
            return Forbid("Only admins can add internal comments.");
        }

        var comment = new TicketComment
        {
            TicketId = ticketId,
            CommentText = request.CommentText,
            IsInternal = request.IsInternal,
            CreatedDate = DateTime.UtcNow,
            CreatedByUserId = userId
        };

        _context.TicketComments.Add(comment);
        await _context.SaveChangesAsync();

        return Ok();
    }
}
