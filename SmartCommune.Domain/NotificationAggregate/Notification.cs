using SmartCommune.Domain.Common.Models;
using SmartCommune.Domain.NotificationAggregate.Entities;
using SmartCommune.Domain.NotificationAggregate.ValueObjects;
using SmartCommune.Domain.UserAggregate;
using SmartCommune.Domain.UserAggregate.ValueObjects;

namespace SmartCommune.Domain.NotificationAggregate;

public sealed class Notification : AggregateRoot<NotificationId>
{
    private readonly List<NotificationAttachment> _attachments = [];

#pragma warning disable CS8618
    private Notification()
    {
    }
#pragma warning restore CS8618

    private Notification(
        NotificationId id,
        string title,
        string content,
        NotificationType type,
        DateTime createdAt,
        bool isGlobal,
        ApplicationUserId senderId)
        : base(id)
    {
        Title = title;
        Content = content;
        Type = type;
        CreatedAt = createdAt;
        IsGlobal = isGlobal;
        SenderId = senderId;
    }

    public string Title { get; private set; }
    public string Content { get; private set; }
    public NotificationType Type { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsGlobal { get; private set; }
    public ApplicationUserId SenderId { get; private set; }

    public ApplicationUser Sender { get; private set; } = null!;
    public IReadOnlyCollection<NotificationAttachment> Attachments => _attachments.AsReadOnly();

    public static Notification Create(
        string title,
        string content,
        NotificationType type,
        DateTime createdAt,
        bool isGlobal,
        ApplicationUserId senderId)
    {
        var notification = new Notification(
            NotificationId.CreateUnique(),
            title,
            content,
            type,
            createdAt,
            isGlobal,
            senderId);

        return notification;
    }

    // Thêm tệp đính kèm vào thông báo.
    public void AddAttachment(string fileName, string fileUrl, string fileType, long fileSize)
    {
        var attachment = NotificationAttachment.Create(
            Id, // Notification Id.
            fileName,
            fileUrl,
            fileType,
            fileSize);
        _attachments.Add(attachment);
    }
}