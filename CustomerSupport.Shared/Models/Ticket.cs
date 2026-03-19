namespace CustomerSupport.Shared.Models;

using CustomerSupport.Shared.Enums;

public class Ticket
{
    public int Id { get; set; }
    public string TicketNumber { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TicketPriority Priority { get; set; }
    public TicketStatus Status { get; set; } = TicketStatus.Open;
    public DateTime CreatedDate { get; set; }
    public int CreatedByUserId { get; set; }
    public int? AssignedToAdminId { get; set; }

    // Navigation properties (not used in DTOs, but good for EF)
    public User? CreatedBy { get; set; }
    public User? AssignedTo { get; set; }
    public List<TicketComment> Comments { get; set; } = new();
    public List<TicketStatusHistory> History { get; set; } = new();
}
