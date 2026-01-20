using ErrorOr;

namespace SmartCommune.Domain.Common.Errors;

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