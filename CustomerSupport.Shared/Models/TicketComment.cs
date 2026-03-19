namespace CustomerSupport.Shared.Models;

public class TicketComment
{
    public int Id { get; set; }
    public int TicketId { get; set; }
    public string CommentText { get; set; } = string.Empty;
    public bool IsInternal { get; set; }
    public DateTime CreatedDate { get; set; }
    public int CreatedByUserId { get; set; }
    public User? CreatedBy { get; set; }
}
