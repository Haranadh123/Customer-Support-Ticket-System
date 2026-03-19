namespace CustomerSupport.Shared.DTOs;

using CustomerSupport.Shared.Enums;

public record LoginRequest(string Username, string Password);
public record LoginResponse(int Id, string Username, UserRole Role, string Token);

public record CreateTicketRequest(string Subject, string Description, TicketPriority Priority);

public record TicketDto(
    int Id,
    string TicketNumber,
    string Subject,
    string Description,
    TicketPriority Priority,
    TicketStatus Status,
    DateTime CreatedDate,
    string CreatedByUsername,
    string? AssignedToUsername
);

public record CommentDto(
    int Id,
    string CommentText,
    bool IsInternal,
    DateTime CreatedDate,
    string CreatedByUsername
);

public record HistoryDto(
    TicketStatus OldStatus,
    TicketStatus NewStatus,
    string ChangedByUsername,
    DateTime ChangedDate
);

public record TicketDetailsDto(
    TicketDto Ticket,
    List<CommentDto> Comments,
    List<HistoryDto> History
);

public record UpdateStatusRequest(TicketStatus NewStatus);
public record AssignTicketRequest(int AdminId);
public record AddCommentRequest(string CommentText, bool IsInternal);
public record AdminDto(int Id, string Username);
