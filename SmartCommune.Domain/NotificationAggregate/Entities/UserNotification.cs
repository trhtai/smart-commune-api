using SmartCommune.Domain.Common.Models;
using SmartCommune.Domain.NotificationAggregate.ValueObjects;
using SmartCommune.Domain.UserAggregate.ValueObjects;

namespace SmartCommune.Domain.NotificationAggregate.Entities;

public sealed class UserNotification : Entity<UserNotificationId>
{
#pragma warning disable CS8618
    private UserNotification()
    {
    }
#pragma warning restore CS8618

    private UserNotification(
        UserNotificationId userNotificationId,
        ApplicationUserId receiverId,
        NotificationId notificationId,
        bool isRead,
        DateTime receivedAt,
        DateTime? readAt)
        : base(userNotificationId)
    {
        ReceiverId = receiverId;
        NotificationId = notificationId;
        IsRead = isRead;
        ReceivedAt = receivedAt;
        ReadAt = readAt;
    }

    public ApplicationUserId ReceiverId { get; private set; }
    public NotificationId NotificationId { get; private set; }
    public bool IsRead { get; private set; }
    public DateTime ReceivedAt { get; private set; }
    public DateTime? ReadAt { get; private set; }

    public Notification Notification { get; private set; } = null!;

    public static UserNotification Create(
        ApplicationUserId receiverId,
        NotificationId notificationId,
        bool isRead,
        DateTime receivedAt)
    {
        var userNotification = new UserNotification(
            UserNotificationId.CreateUnique(),
            receiverId,
            notificationId,
            isRead,
            receivedAt,
            readAt: null);

        return userNotification;
    }

    /// <summary>
    /// Đánh dấu thông báo đã đọc.
    /// </summary>
    /// <param name="readAt">Thời gian đọc thông báo.</param>
    public void MarkAsRead(DateTime readAt)
    {
        if (!IsRead)
        {
            IsRead = true;
            ReadAt = readAt;
        }
    }
}