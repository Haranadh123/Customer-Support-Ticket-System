namespace CustomerSupport.Shared.Models;

using CustomerSupport.Shared.Enums;

public class TicketStatusHistory
{
    public int Id { get; set; }
    public int TicketId { get; set; }
    public TicketStatus OldStatus { get; set; }
    public TicketStatus NewStatus { get; set; }
    public int ChangedByUserId { get; set; }
    public DateTime ChangedDate { get; set; }
    public User? ChangedBy { get; set; }
}
