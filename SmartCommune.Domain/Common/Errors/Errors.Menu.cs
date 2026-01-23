using ErrorOr;

namespace SmartCommune.Domain.Common.Errors;

/// <summary>
/// Đây là một static class chứa các định nghĩa về lỗi liên quan đến menu người dùng.
/// </summary>
public static partial class Errors
{
    public static class Menu
    {
        public static readonly Error ParentNotFound =
            Error.NotFound(
                code: "Menu.ParentNotFound",
                description: "Menu cha không tồn tại");

        public static readonly Error NotFound =
            Error.NotFound(
                code: "Menu.NotFound",
                description: "Menu Item không tồn tại");
    }
}