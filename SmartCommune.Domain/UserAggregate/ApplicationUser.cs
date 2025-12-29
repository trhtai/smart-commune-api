using SmartCommune.Domain.Common.Models;
using SmartCommune.Domain.UserAggregate.ValueObjects;

namespace SmartCommune.Domain.UserAggregate;

public sealed class ApplicationUser : AggregateRoot<ApplicationUserId>
{
    private ApplicationUser()
    {
        UserName = null!;
        PasswordHash = null!;
        FullName = null!;
    }

    private ApplicationUser(
        ApplicationUserId appUserId,
        string userName,
        string passwordHash,
        string fullName,
        DateTime createdAt)
        : base(appUserId)
    {
        UserName = userName;
        PasswordHash = passwordHash;
        FullName = fullName;
        IsActived = true;
        CreatedAt = createdAt;
        DisableAt = null;
    }

    public string UserName { get; private set; }
    public string PasswordHash { get; private set; }
    public string FullName { get; private set; }
    public bool IsActived { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? DisableAt { get; private set; }

    public static ApplicationUser Create(
        string userName,
        string passwordHash,
        string fullName,
        DateTime createdAt)
    {
        var user = new ApplicationUser(
            ApplicationUserId.CreateUnique(),
            userName,
            passwordHash,
            fullName,
            createdAt);

        return user;
    }
}