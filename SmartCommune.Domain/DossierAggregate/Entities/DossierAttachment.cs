using SmartCommune.Domain.Common.Models;
using SmartCommune.Domain.DossierAggregate.ValueObjects;

namespace SmartCommune.Domain.DossierAggregate.Entities;

public sealed class DossierAttachment : Entity<DossierAttachmentId>
{
#pragma warning disable CS8618
    private DossierAttachment()
    {
    }
#pragma warning restore CS8618

    private DossierAttachment(
        DossierAttachmentId documentAttachmentId,
        DossierId dossierId,
        string fileName,
        string fileUrl,
        string fileType,
        long fileSize,
        DateTime uploadedAt)
        : base(documentAttachmentId)
    {
        DossierId = dossierId;
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
    public DossierId DossierId { get; private set; }

    internal static DossierAttachment Create(
        DossierId dossierId,
        string fileName,
        string fileUrl,
        string fileType,
        long fileSize,
        DateTime uploadedAt)
    {
        return new DossierAttachment(
            DossierAttachmentId.CreateUnique(),
            dossierId,
            fileName,
            fileUrl,
            fileType,
            fileSize,
            uploadedAt);
    }
}