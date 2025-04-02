using NotificationService.Domain.Enums;

namespace NotificationService.Domain.DTOs;

public record class SendEmailRequest(
    NotificationTypes Type,
    string UserId,
    string Username,
    string Email,
    Dictionary<string, string> Data);