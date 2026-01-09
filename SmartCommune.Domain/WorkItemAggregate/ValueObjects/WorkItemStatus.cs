using Ardalis.SmartEnum;

namespace SmartCommune.Domain.WorkItemAggregate.ValueObjects;

public sealed class WorkItemStatus : SmartEnum<WorkItemStatus>
{
    public static readonly WorkItemStatus Todo = new("Todo", 1, "Mới tạo");
    public static readonly WorkItemStatus InProgress = new("InProgress", 2, "Đang thực hiện");
    public static readonly WorkItemStatus InReview = new("InReview", 3, "Chờ duyệt");
    public static readonly WorkItemStatus Done = new("Done", 4, "Hoàn thành");
    public static readonly WorkItemStatus Canceled = new("Canceled", 5, "Đã hủy");
    public static readonly WorkItemStatus Reject = new("Reject", 6, "Từ chối");

    private WorkItemStatus(string name, int value, string title)
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