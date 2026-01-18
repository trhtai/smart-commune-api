using ErrorOr;

namespace SmartCommune.Domain.Common.Errors;

/// <summary>
/// Đây là một static class chứa các định nghĩa về lỗi liên quan đến người dùng.
/// </summary>
public static partial class Errors
{
    public static class User
    {
        public static readonly Error NotFound =
            Error.NotFound(
                code: "User.NotFound",
                description: "Không tìm thấy người dùng.");
    }
}