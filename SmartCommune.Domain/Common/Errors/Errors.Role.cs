using ErrorOr;

namespace SmartCommune.Domain.Common.Errors;

/// <summary>
/// Đây là một static class chứa các định nghĩa về lỗi liên quan đến vai trò người dùng.
/// </summary>
public static partial class Errors
{
    public static class Role
    {
        public static readonly Error NotFound =
            Error.NotFound(
                code: "Role.NotFound",
                description: "Không tìm thấy vai trò");
    }
}