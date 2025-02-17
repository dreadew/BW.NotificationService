namespace NotificationService.Domain.DTOs;

public record class SendEmailRequest(
    string Type,
    string UserId,
    string Username,
    string Email,
    Dictionary<string, string> Data);