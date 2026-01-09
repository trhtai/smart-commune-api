using SmartCommune.Domain.Common.Models;

namespace SmartCommune.Domain.UserAggregate.ValueObjects;

public sealed class PasswordHash : ValueObject
{
    private PasswordHash(string value, string salt)
    {
        Value = value;
        Salt = salt;
    }

    public string Value { get; private set; }
    public string Salt { get; private set; }

    public static PasswordHash Create(string rawPassword)
    {
        // 1. Mỗi user sẽ có một salt riêng biệt.
        string salt = BCrypt.Net.BCrypt.GenerateSalt();

        // 2. Hash password với salt.
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(rawPassword, salt);

        return new PasswordHash(hashedPassword, salt);
    }

    public bool Verify(string rawPassword)
    {
        // BCrypt tự trích xuất salt từ chuỗi hash để verify, nên không cần truyền Salt vào.
        return BCrypt.Net.BCrypt.Verify(rawPassword, Value);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}