using SmartCommune.Domain.Common.Models;
using SmartCommune.Domain.WorkItemAggregate.ValueObjects;

namespace SmartCommune.Domain.WorkItemAggregate.Entities;

public sealed class WorkItemReportAttachment : Entity<WorkItemReportAttachmentId>
{
#pragma warning disable CS8618
    private WorkItemReportAttachment()
    {
    }
#pragma warning restore CS8618

    private WorkItemReportAttachment(
        WorkItemReportAttachmentId workItemReportAttachmentId,
        WorkItemReportId workItemReportId,
        string fileName,
        string fileUrl,
        string fileType,
        long fileSize,
        DateTime uploadedAt)
        : base(workItemReportAttachmentId)
    {
        WorkItemReportId = workItemReportId;
        FileName = fileName;
        FileUrl = fileUrl;
        FileType = fileType;
        FileSize = fileSize;
        UploadedAt = uploadedAt;
    }

    public string FileName { get; private set; }
    public string FileUrl { get; private set; }
    public string FileType { get; private set; }
    public long FileSize { get; private set; }
    public DateTime UploadedAt { get; private set; }
    public WorkItemReportId WorkItemReportId { get; private set; }

    internal static WorkItemReportAttachment Create(
        WorkItemReportId workItemReportId,
        string fileName,
        string fileUrl,
        string fileType,
        long fileSize,
        DateTime uploadedAt)
    {
        return new WorkItemReportAttachment(
            WorkItemReportAttachmentId.CreateUnique(),
            workItemReportId,
            fileName,
            fileUrl,
            fileType,
            fileSize,
            uploadedAt);
    }
}