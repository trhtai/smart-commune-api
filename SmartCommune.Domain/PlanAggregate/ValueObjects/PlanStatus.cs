using Ardalis.SmartEnum;

using SmartCommune.Domain.WorkItemAggregate.ValueObjects;

namespace SmartCommune.Domain.PlanAggregate.ValueObjects;

public sealed class PlanStatus : SmartEnum<PlanStatus>
{
    public static readonly PlanStatus Todo = new("Todo", 1, "Mới tạo");
    public static readonly PlanStatus InProgress = new("InProgress", 2, "Đang thực hiện");
    public static readonly PlanStatus Done = new("Done", 3, "Hoàn thành");
    public static readonly PlanStatus Canceled = new("Canceled", 4, "Đã hủy");

    private PlanStatus(string name, int value, string title)
        : base(name, value)
    {
        Title = title;
    }

    public string Title { get; }

    /// <summary>
    /// Kiểm tra xem có được chuyển trạng thái không.
    /// </summary>
    /// <param name="nextStatus">Trạng thái tiếp theo.</param>
    /// <returns>True: được phép thay đổi trạng thái, ngược lại thì không.</returns>
    public bool CanTransitionTo(WorkItemStatus nextStatus)
    {
        // Không thể chuyển từ "Đã hủy" về "Mới tạo".
        if (this == Canceled && nextStatus == Todo)
        {
            return false;
        }

        // Đã xong thì không được quay lại đang làm.
        if (this == Done && nextStatus == InProgress)
        {
            return false;
        }

        return true;
    }
}