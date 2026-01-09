using SmartCommune.Domain.Common.Models;

namespace SmartCommune.Domain.Common.ValueObjects;

public sealed class DateRange : ValueObject
{
    private DateRange(DateTime startDate, DateTime endDate)
    {
        StartDate = startDate;
        EndDate = endDate;
    }

    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }

    public static DateRange Create(DateTime startDate, DateTime endDate)
    {
        return startDate > endDate
            ? throw new ArgumentException("Ngày bắt đầu không thể lớn hơn ngày kết thúc.")
            : new DateRange(startDate, endDate);
    }

    /// <summary>
    /// Tính khoảng thời gian.
    /// </summary>
    /// <returns>Trả về khoảng thời gian kiểu TimeSpan.</returns>
    public TimeSpan Duration()
    {
        return EndDate - StartDate;
    }

    /// <summary>
    /// Kiểm tra xem đã quá hạn chưa.
    /// </summary>
    /// <param name="nowDate">Thời gian hiện tại.</param>
    /// <returns>Trả về true nếu quá hạn, ngược lại thì false.</returns>
    public bool IsOverdue(DateTime nowDate)
    {
        return nowDate > EndDate;
    }

    /// <summary>
    /// Kiểm tra 1 ngày có nằm trong khoảng này không.
    /// </summary>
    /// <param name="date">Ngày cần kiểm tra trong khoảng.</param>
    /// <returns>Trả về truy nếu ngày cần kiểm tra nằm trong khoảng.</returns>
    public bool Includes(DateTime date)
    {
        return date >= StartDate && date <= EndDate;
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return StartDate;
        yield return EndDate;
    }
}