using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SmartCommune.Domain.PermissionAggregate.ValueObjects;
using SmartCommune.Domain.UserAggregate;
using SmartCommune.Domain.UserAggregate.Entities;
using SmartCommune.Domain.UserAggregate.ValueObjects;

namespace SmartCommune.Infrastructure.Persistence.Configurations;

public class UserPermissionConfiguration : IEntityTypeConfiguration<UserPermission>
{
    public void Configure(EntityTypeBuilder<UserPermission> builder)
    {
        builder.ToTable("UserPermissions");

        builder.HasKey(up => new { up.UserId, up.PermissionId });

        builder.Property(up => up.UserId)
            .HasConversion(
                id => id.Value,
                value => ApplicationUserId.Create(value))
            .IsRequired();

        builder.Property(up => up.PermissionId)
            .HasConversion(
                id => id.Value,
                value => PermissionId.Create(value))
            .IsRequired();

        // Cấu hình quan hệ với User.
        builder.HasOne<ApplicationUser>()
            .WithMany(u => u.Permissions)
            .HasForeignKey(up => up.UserId)
            .OnDelete(DeleteBehavior.Cascade); // Xóa User -> Xóa Permissions liên quan.

        // Cấu hình quan hệ với Permission.
        builder.HasOne(rp => rp.Permission)
            .WithMany()
            .HasForeignKey(rp => rp.PermissionId)
            .OnDelete(DeleteBehavior.Restrict); // Không được xóa Permission nếu đang có User dùng.
    }
}