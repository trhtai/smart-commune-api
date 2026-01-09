using System.Text.Json;

using SmartCommune.Domain.Common.Models;

namespace SmartCommune.Domain.MenuItemAggregate.ValueObjects;

public sealed class MenuItemConfig : ValueObject
{
    private MenuItemConfig(
        string type,
        string? path,
        string? icon,
        string? activeIcon,
        List<string> checkRoutes,
        List<string> relatedPaths)
    {
        Type = type;
        Path = path;
        Icon = icon;
        ActiveIcon = activeIcon;
        CheckRoutes = checkRoutes;
        RelatedPaths = relatedPaths;
    }

    /// <summary>
    /// "item", "submenu", "group",...
    /// </summary>
    public string Type { get; private set; }

    /// <summary>
    /// "/work", "/auth",...
    /// </summary>
    public string? Path { get; private set; }

    /// <summary>
    /// "ListTaskIcon", "TagsIcon",...
    /// </summary>
    public string? Icon { get; private set; }

    /// <summary>
    /// "ListTaskActiveIcon", "TagsActiveIcon",...
    /// </summary>
    public string? ActiveIcon { get; private set; }

    /// <summary>
    /// Giúp menu active khi user đứng ở bất kì trang con nào bên trong route này.
    /// VD: menu cha "Quản lí nhân sự".
    /// checkRoutes: ["/work/employee", "/work/department", "/work/position"].
    /// -> Khi user đứng ở trang "/work/employee", menu "Quản lí nhân sự" vẫn active.
    /// </summary>
    public List<string> CheckRoutes { get; private set; } = [];

    /// <summary>
    /// Dùng cho Item con. Để giữ cho menu con vẫn sáng đèn khi user đi vào các trang chi tiết (trang ẩn không có trên menu).
    /// VD: menu con "Danh sách văn bản" có link là "/documents".
    /// User bấm vào xem chi tiết văn bản có link là "/documents/details/123".
    /// Lúc này URL là "documents/details/123" khác với "documents", nên bình thường menu con sẽ không active.
    /// Nhưng nhờ "relatedPaths: ['/documents/details/123']", hệ thống sẽ biết rằng "À, đang ở trang chi tiết thì cũng như đang ở trang danh sách"
    /// -> Tô màu menu con "Danh sách văn bản".
    /// </summary>
    public List<string> RelatedPaths { get; private set; } = [];

    public static MenuItemConfig Create(
        string type,
        string? path,
        string? icon,
        string? activeIcon,
        List<string> checkRoutes,
        List<string> relatedPaths)
    {
        return new MenuItemConfig(
            type,
            path,
            icon,
            activeIcon,
            checkRoutes,
            relatedPaths);
    }

    /// <summary>
    /// Việc kế thừa ValueObject class là để so sánh tham trị của 2 đối tượng,
    /// nhưng MenuSystemConfig lại không cần so sánh, vậy việc này có lợi ích gì?
    /// 1. Kiểm tra xem Admin có thật sự sửa gì đó không hay chỉ bấm lưu "chơi".
    /// 2. Tiện cho Unit Test: khi test ta thường tạo 1 danh sách expected để so sánh với danh sách actual.
    /// Nếu không có ValueObject, ta sẽ phải assert từng dòng (Assert.Equal(a.Type, b.Type), Assert.Equal(a.Path, b.Path)...).
    /// Có ValueObject ta chỉ cần 1 dòng Assert.Equal(expected, actual).
    /// 3. Loại bỏ các cấu hình trùng nhau: "var uniqueConfigs = listConfigs.Distinct().ToList();".
    /// </summary>
    /// <returns>Một tập hợp các đối tượng để so sánh.</returns>
    public override IEnumerable<object> GetEqualityComponents()
    {
        // 1. Các kiểu dữ liệu nguyên thủy thì yield trực tiếp.
        yield return Type;
        yield return Path ?? string.Empty;
        yield return Icon ?? string.Empty;
        yield return ActiveIcon ?? string.Empty;

        // 2. Các kiểu dữ liệu danh sách (List/Dictionary).
        // VẤN ĐỀ: List<string> không so sánh value.
        // GIẢI PHÁP: Serialize nó thành string hoặc sắp xếp rồi yield từng phần tử.
        yield return JsonSerializer.Serialize(CheckRoutes.OrderBy(x => x));
        yield return JsonSerializer.Serialize(RelatedPaths.OrderBy(x => x));
    }
}