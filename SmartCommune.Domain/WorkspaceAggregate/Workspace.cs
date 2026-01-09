using SmartCommune.Domain.Common.Models;
using SmartCommune.Domain.UserAggregate.ValueObjects;
using SmartCommune.Domain.WorkspaceAggregate.Entities;
using SmartCommune.Domain.WorkspaceAggregate.ValueObjects;

namespace SmartCommune.Domain.WorkspaceAggregate;

public sealed class Workspace : AggregateRoot<WorkspaceId>
{
    private readonly List<WorkItemCollection> _collections = [];

#pragma warning disable CS8618
    private Workspace()
    {
    }
#pragma warning restore CS8618

    private Workspace(
        WorkspaceId workspaceId,
        string title,
        string description,
        DateTime createdAt,
        ApplicationUserId ownerId)
        : base(workspaceId)
    {
        Title = title;
        Description = description;
        CreatedAt = createdAt;
        OwnerId = ownerId;
    }

    public string Title { get; private set; }
    public string Description { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public ApplicationUserId OwnerId { get; private set; }

    public IReadOnlyCollection<WorkItemCollection> Collections => _collections.AsReadOnly();

    public static Workspace Create(
        string title,
        string description,
        DateTime createdAt,
        ApplicationUserId ownerId)
    {
        return new Workspace(
            WorkspaceId.CreateUnique(),
            title,
            description,
            createdAt,
            ownerId);
    }
}