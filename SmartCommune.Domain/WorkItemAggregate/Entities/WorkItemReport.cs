using SmartCommune.Domain.Common.Models;
using SmartCommune.Domain.UserAggregate;
using SmartCommune.Domain.UserAggregate.ValueObjects;
using SmartCommune.Domain.WorkItemAggregate.ValueObjects;

namespace SmartCommune.Domain.WorkItemAggregate.Entities;

public sealed class WorkItemReport : Entity<WorkItemReportId>
{
    private readonly List<WorkItemReportAttachment> _attachments = [];

#pragma warning disable CS8618
    private WorkItemReport()
    {
    }
#pragma warning restore CS8618

    private WorkItemReport(
        WorkItemReportId workItemReportId,
        string title,
        string description,
        int progress,
        DateTime reportDate,
        WorkItemId workItemId,
        ApplicationUserId reporterId)
        : base(workItemReportId)
    {
        Title = title;
        Description = description;
        Progress = progress;
        ReportDate = reportDate;
        WorkItemId = workItemId;
        ReporterId = reporterId;
    }

    public string Title { get; private set; }
    public string Description { get; private set; }
    public int Progress { get; private set; }
    public DateTime ReportDate { get; private set; }
    public WorkItemId WorkItemId { get; private set; }
    public ApplicationUserId ReporterId { get; private set; }

    public ApplicationUser Reporter { get; private set; } = null!;
    public IReadOnlyCollection<WorkItemReportAttachment> Attachments => _attachments.AsReadOnly();

    public static WorkItemReport Create(
        string title,
        string description,
        int progress,
        DateTime reportDate,
        WorkItemId workItemId,
        ApplicationUserId reporterId)
    {
        return new WorkItemReport(
            WorkItemReportId.CreateUnique(),
            title,
            description,
            progress,
            reportDate,
            workItemId,
            reporterId);
    }
}