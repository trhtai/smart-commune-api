using SmartCommune.Domain.PlanAggregate.ValueObjects;

namespace SmartCommune.Domain.PlanAggregate.Entities;

public sealed class PlanEvaluation
{
#pragma warning disable CS8618
    private PlanEvaluation()
    {
    }
#pragma warning restore CS8618

    private PlanEvaluation(
        PlanId planId,
        string result,
        string strengths,
        string weaknesses,
        string causes,
        string lessonsLearned,
        string conclusion)
    {
        PlanId = planId;
        Result = result;
        Strengths = strengths;
        Weaknesses = weaknesses;
        Causes = causes;
        LessonsLearned = lessonsLearned;
        Conclusion = conclusion;
    }

    // Foreign Key trỏ ngược về Plan (nhưng cũng là PK của bảng này luôn).
    public PlanId PlanId { get; private set; }
    public string Result { get; private set; }
    public string Strengths { get; private set; }
    public string Weaknesses { get; private set; }
    public string Causes { get; private set; }
    public string LessonsLearned { get; private set; }
    public string Conclusion { get; private set; }

    public Plan Plan { get; private set; } = null!;

    internal static PlanEvaluation Create(
        PlanId planId,
        string result,
        string strengths,
        string weaknesses,
        string causes,
        string lessonsLearned,
        string conclusion)
    {
        var evaluation = new PlanEvaluation(
            planId,
            result,
            strengths,
            weaknesses,
            causes,
            lessonsLearned,
            conclusion);

        return evaluation;
    }
}