using SmartCommune.Domain.Common.Models;
using SmartCommune.Domain.Common.ValueObjects;
using SmartCommune.Domain.PlanAggregate.Entities;
using SmartCommune.Domain.PlanAggregate.ValueObjects;
using SmartCommune.Domain.UserAggregate;
using SmartCommune.Domain.UserAggregate.ValueObjects;

namespace SmartCommune.Domain.PlanAggregate;

public sealed class Plan : AggregateRoot<PlanId>
{
    private readonly List<Plan> _subPlans = [];

#pragma warning disable CS8618
    private Plan()
    {
    }
#pragma warning restore CS8618

    private Plan(
        PlanId planId,
        string title,
        DateRange timeline,
        PlanStatus status,
        string basis,
        string currentSituation,
        string goal,
        string targets,
        string executionStrategy,
        string solutions,
        DateTime createdAt,
        PlanId? parentId,
        ApplicationUserId createdById)
        : base(planId)
    {
        Title = title;
        Timeline = timeline;
        Status = status;
        Basis = basis;
        CurrentSituation = currentSituation;
        Goal = goal;
        Targets = targets;
        ExecutionStrategy = executionStrategy;
        Solutions = solutions;
        CreatedAt = createdAt;
        ParentId = parentId;
        CreatedById = createdById;
    }

    public string Title { get; private set; }
    public DateRange Timeline { get; private set; }
    public PlanStatus Status { get; private set; }

    /// <summary>
    /// Sở cứ.
    /// </summary>
    public string Basis { get; private set; }

    /// <summary>
    /// Đánh giá hiện trạng.
    /// </summary>
    public string CurrentSituation { get; private set; }

    /// <summary>
    /// Mục tiêu.
    /// </summary>
    public string Goal { get; private set; }

    /// <summary>
    /// Chỉ tiêu.
    /// </summary>
    public string Targets { get; private set; }

    /// <summary>
    /// Kế hoạch hành động (Mô tả).
    /// </summary>
    public string ExecutionStrategy { get; private set; }

    /// <summary>
    /// Giải pháp.
    /// </summary>
    public string Solutions { get; private set; }

    /// <summary>
    /// Đánh giá.
    /// </summary>
    public PlanEvaluation? Evaluation { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public PlanId? ParentId { get; private set; }
    public ApplicationUserId CreatedById { get; private set; }

    public ApplicationUser CreatedBy { get; private set; } = null!;
    public IReadOnlyCollection<Plan> SubPlans => _subPlans.AsReadOnly();

    public static Plan Create(
        string title,
        DateRange timeline,
        PlanStatus status,
        string basis,
        string currentSituation,
        string goal,
        string targets,
        string executionStrategy,
        string solutions,
        DateTime createdAt,
        PlanId? parentId,
        ApplicationUserId createdById)
    {
        return new Plan(
            PlanId.CreateUnique(),
            title,
            timeline,
            status,
            basis,
            currentSituation,
            goal,
            targets,
            executionStrategy,
            solutions,
            createdAt,
            parentId,
            createdById);
    }

    /// <summary>
    /// Đánh giá kế hoạch.
    /// </summary>
    /// <param name="result">Kết quả đạt được.</param>
    /// <param name="strengths">Điểm mạnh (Ưu điểm).</param>
    /// <param name="weaknesses">Nhược điểm (Hạn chế).</param>
    /// <param name="causes">Nguyên nhân (của hạn chế).</param>
    /// <param name="lessons">Bài học kinh nghiệm.</param>
    /// <param name="conclusion">Kết luận chung.</param>
    public void Evaluate(
        string result,
        string strengths,
        string weaknesses,
        string causes,
        string lessons,
        string conclusion)
    {
        Evaluation = PlanEvaluation.Create(
            Id, // Truyền ID của Plan vào làm ID của Evaluation.
            result,
            strengths,
            weaknesses,
            causes,
            lessons,
            conclusion);
    }

    /// <summary>
    /// Thêm kế hoạch con.
    /// </summary>
    /// <param name="subPlan">Kế hoạch con.</param>
    public void AddSubPlan(Plan subPlan)
    {
        _subPlans.Add(subPlan);
    }
}