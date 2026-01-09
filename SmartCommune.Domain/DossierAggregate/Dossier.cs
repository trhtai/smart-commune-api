using SmartCommune.Domain.Common.Models;
using SmartCommune.Domain.DossierAggregate.Entities;
using SmartCommune.Domain.DossierAggregate.ValueObjects;
using SmartCommune.Domain.UserAggregate;
using SmartCommune.Domain.UserAggregate.ValueObjects;

namespace SmartCommune.Domain.DossierAggregate;

public sealed class Dossier : AggregateRoot<DossierId>
{
    private readonly List<DossierAttachment> _attachments = [];
    private readonly List<DossierMember> _members = [];

#pragma warning disable CS8618
    private Dossier()
    {
    }
#pragma warning restore CS8618

    private Dossier(
        DossierId dossierId,
        string dossierNumber,
        string title,
        string note,
        DossierStatus status,
        DossierPriority priority,
        DateTime? deadline,
        DateTime createdAt,
        ApplicationUserId createdById)
        : base(dossierId)
    {
        DossierNumber = dossierNumber;
        Title = title;
        Note = note;
        Status = status;
        Priority = priority;
        Deadline = deadline;
        CreatedAt = createdAt;
        CreatedById = createdById;
    }

    public string DossierNumber { get; private set; }
    public string Title { get; private set; }
    public string Note { get; private set; }
    public DossierStatus Status { get; private set; }
    public DossierPriority Priority { get; private set; }
    public DateTime? Deadline { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public ApplicationUserId CreatedById { get; private set; }

    public ApplicationUser CreatedBy { get; private set; } = null!;

    public IReadOnlyCollection<DossierAttachment> Attachments => _attachments.AsReadOnly();
    public IReadOnlyCollection<DossierMember> Members => _members.AsReadOnly();

    public static Dossier Create(
        string dossierNumber,
        string title,
        string note,
        DossierStatus status,
        DossierPriority priority,
        DateTime? deadline,
        DateTime createdAt,
        ApplicationUserId createdById)
    {
        return new Dossier(
            DossierId.CreateUnique(),
            dossierNumber,
            title,
            note,
            status,
            priority,
            deadline,
            createdAt,
            createdById);
    }
}