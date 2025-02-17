namespace NotificationService.Domain.DTOs;

public record class EmailMessageDto(
    string To,
    string Subject,
    string Body);