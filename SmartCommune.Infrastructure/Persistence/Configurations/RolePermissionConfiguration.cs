using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SmartCommune.Domain.PermissionAggregate.ValueObjects;
using SmartCommune.Domain.RoleAggregate;
using SmartCommune.Domain.RoleAggregate.Entities;
using SmartCommune.Domain.RoleAggregate.ValueObjects;

namespace SmartCommune.Infrastructure.Persistence.Configurations;

public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("RolePermissions");

        builder.HasKey(rp => new { rp.RoleId, rp.PermissionId });

        builder.Property(rp => rp.RoleId)
            .HasConversion(
                id => id.Value,
                value => RoleId.Create(value))
            .IsRequired();

        builder.Property(rp => rp.PermissionId)
            .HasConversion(
                id => id.Value,
                value => PermissionId.Create(value))
            .IsRequired();

        // Cấu hình quan hệ với Role.
        builder.HasOne<Role>()
            .WithMany(r => r.Permissions)
            .HasForeignKey(rp => rp.RoleId)
            .OnDelete(DeleteBehavior.Cascade); // Xóa Role -> Xóa Permissions liên quan.

        // Cấu hình quan hệ với Permission.
        builder.HasOne(rp => rp.Permission)
            .WithMany()
            .HasForeignKey(rp => rp.PermissionId)
            .OnDelete(DeleteBehavior.Restrict); // Không được xóa Permission nếu đang có Role dùng.
    }
}