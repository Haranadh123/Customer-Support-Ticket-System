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
public class TicketsController : ControllerBase
{
    private readonly AppDbContext _context;

    public TicketsController(AppDbContext context)
    {
        _context = context;
    }

    private int GetUserId()
    {
        var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (string.IsNullOrEmpty(idStr))
        {
            throw new UnauthorizedAccessException("User ID claim not found.");
        }
        return int.Parse(idStr);
    }

    private bool IsAdmin() => User.IsInRole("Admin") || User.HasClaim(ClaimTypes.Role, "Admin");

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TicketDto>>> GetTickets()
    {
        var userId = GetUserId();
        var isAdmin = IsAdmin();

        var query = _context.Tickets
            .Include(t => t.CreatedBy)
            .Include(t => t.AssignedTo)
            .AsQueryable();

        if (!isAdmin)
        {
            query = query.Where(t => t.CreatedByUserId == userId);
        }

        var dbTickets = await query
            .OrderByDescending(t => t.CreatedDate)
            .ToListAsync();

        var tickets = dbTickets
            .Select(t => new TicketDto(
                t.Id,
                t.TicketNumber,
                t.Subject,
                t.Description,
                t.Priority,
                t.Status,
                t.CreatedDate,
                t.CreatedBy?.Username ?? "Unknown",
                t.AssignedTo?.Username,
                t.AssignedToAdminId
            ))
            .ToList();

        return Ok(tickets);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TicketDetailsDto>> GetTicketDetails(int id)
    {
        var userId = GetUserId();
        var isAdmin = IsAdmin();

        var ticket = await _context.Tickets
            .Include(t => t.CreatedBy)
            .Include(t => t.AssignedTo)
            .Include(t => t.Comments).ThenInclude(c => c.CreatedBy)
            .Include(t => t.History).ThenInclude(h => h.ChangedBy)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (ticket == null) return NotFound();
        if (!isAdmin && ticket.CreatedByUserId != userId) return Forbid();

        var ticketDto = new TicketDto(
            ticket.Id,
            ticket.TicketNumber,
            ticket.Subject,
            ticket.Description,
            ticket.Priority,
            ticket.Status,
            ticket.CreatedDate,
            ticket.CreatedBy!.Username,
            ticket.AssignedTo?.Username,
            ticket.AssignedToAdminId
        );

        var comments = ticket.Comments
            .Where(c => isAdmin || !c.IsInternal)
            .Select(c => new CommentDto(c.Id, c.CommentText, c.IsInternal, c.CreatedDate, c.CreatedBy!.Username))
            .ToList();

        var history = ticket.History
            .Select(h => new HistoryDto(h.OldStatus, h.NewStatus, h.ChangedBy!.Username, h.ChangedDate))
            .ToList();

        return Ok(new TicketDetailsDto(ticketDto, comments, history));
    }

    [HttpPost]
    public async Task<ActionResult<TicketDto>> CreateTicket(CreateTicketRequest request)
    {
        var userId = GetUserId();
        var ticketCount = await _context.Tickets.CountAsync();
        var ticketNumber = $"TKT-{DateTime.UtcNow:yyyyMMdd}-{ticketCount + 1:D4}";

        var ticket = new Ticket
        {
            TicketNumber = ticketNumber,
            Subject = request.Subject,
            Description = request.Description,
            Priority = request.Priority,
            Status = TicketStatus.Open,
            CreatedDate = DateTime.UtcNow,
            CreatedByUserId = userId
        };

        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTicketDetails), new { id = ticket.Id }, new TicketDto(
            ticket.Id,
            ticket.TicketNumber,
            ticket.Subject,
            ticket.Description,
            ticket.Priority,
            ticket.Status,
            ticket.CreatedDate,
            ticket.CreatedBy!.Username,
            null,
            null
        ));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{id}/assign")]
    public async Task<IActionResult> AssignTicket(int id, AssignTicketRequest request)
    {
        var ticket = await _context.Tickets.FindAsync(id);
        if (ticket == null) return NotFound();

        ticket.AssignedToAdminId = request.AdminId;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, UpdateStatusRequest request)
    {
        var ticket = await _context.Tickets.FindAsync(id);
        if (ticket == null) return NotFound();

        var oldStatus = ticket.Status;
        ticket.Status = request.NewStatus;

        var history = new TicketStatusHistory
        {
            TicketId = id,
            OldStatus = oldStatus,
            NewStatus = request.NewStatus,
            ChangedByUserId = GetUserId(),
            ChangedDate = DateTime.UtcNow
        };

        _context.TicketStatusHistory.Add(history);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("admins")]
    public async Task<ActionResult<IEnumerable<User>>> GetAdmins()
    {
        var admins = await _context.Users
            .Where(u => u.Role == UserRole.Admin)
            .Select(u => new { u.Id, u.Username })
            .ToListAsync();
        return Ok(admins);
    }
}
