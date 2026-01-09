using SmartCommune.Domain.Common.Models;
using SmartCommune.Domain.UserAggregate;
using SmartCommune.Domain.UserAggregate.ValueObjects;
using SmartCommune.Domain.WorkItemAggregate.ValueObjects;

namespace SmartCommune.Domain.WorkItemAggregate.Entities;

public sealed class WorkItemAttachment : Entity<WorkItemAttachmentId>
{
#pragma warning disable CS8618
    private WorkItemAttachment()
    {
    }
#pragma warning restore CS8618

    private WorkItemAttachment(
        WorkItemAttachmentId id,
        WorkItemId workItemId,
        string fileName,
        string fileUrl,
        string fileType,
        long fileSize,
        DateTime uploadedAt,
        ApplicationUserId uploadedById)
        : base(id)
    {
        WorkItemId = workItemId;
        FileName = fileName;
        FileUrl = fileUrl;
        FileType = fileType;
        FileSize = fileSize;
        UploadedAt = uploadedAt;
        UploadedById = uploadedById;
    }

    public string FileName { get; private set; }
    public string FileUrl { get; private set; }
    public string FileType { get; private set; }
    public long FileSize { get; private set; }
    public DateTime UploadedAt { get; private set; }
    public WorkItemId WorkItemId { get; private set; }
    public ApplicationUserId UploadedById { get; private set; }

    public ApplicationUser UploadedBy { get; private set; } = null!;

    internal static WorkItemAttachment Create(
        WorkItemId workItemId,
        string fileName,
        string fileUrl,
        string fileType,
        long fileSize,
        DateTime uploadedAt,
        ApplicationUserId uploadedById)
    {
        return new WorkItemAttachment(
            WorkItemAttachmentId.CreateUnique(),
            workItemId,
            fileName,
            fileUrl,
            fileType,
            fileSize,
            uploadedAt,
            uploadedById);
    }
}