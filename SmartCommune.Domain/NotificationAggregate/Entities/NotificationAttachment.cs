using SmartCommune.Domain.Common.Models;
using SmartCommune.Domain.NotificationAggregate.ValueObjects;

namespace SmartCommune.Domain.NotificationAggregate.Entities;

public sealed class NotificationAttachment : Entity<NotificationAttachmentId>
{
#pragma warning disable CS8618
    private NotificationAttachment()
    {
    }
#pragma warning restore CS8618

    private NotificationAttachment(
        NotificationAttachmentId id,
        NotificationId notificationId,
        string fileName,
        string fileUrl,
        string fileType,
        long fileSize)
        : base(id)
    {
        NotificationId = notificationId;
        FileName = fileName;
        FileUrl = fileUrl;
        FileType = fileType;
        FileSize = fileSize;
    }

    public string FileName { get; private set; }
    public string FileUrl { get; private set; }
    public string FileType { get; private set; }
    public long FileSize { get; private set; }
    public NotificationId NotificationId { get; private set; }

    internal static NotificationAttachment Create(
        NotificationId notificationId,
        string fileName,
        string fileUrl,
        string fileType,
        long fileSize)
    {
        return new NotificationAttachment(
            NotificationAttachmentId.CreateUnique(),
            notificationId,
            fileName,
            fileUrl,
            fileType,
            fileSize);
    }
}