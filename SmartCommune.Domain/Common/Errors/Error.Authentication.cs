using ErrorOr;

namespace SmartCommune.Domain.Common.Errors;

/// <summary>
/// Đây là một static class chứa các định nghĩa về lỗi liên quan đến xác thực.
/// </summary>
public static partial class Errors
{
    public static class Authentication
    {
        public static readonly Error InvalidCredentials =
            Error.Validation(
                code: "Authentication.InvalidCredentials",
                description: "Thông tin được cung cấp không hợp lệ.");

        public static readonly Error Unauthorized =
            Error.Validation(
                code: "Authentication.Unauthorized",
                description: "Người dùng chưa được xác thực.");

        public static readonly Error AccountLocked =
            Error.Conflict(
                code: "Authentication.AccountLocked",
                description: "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ Admin.");
    }
}