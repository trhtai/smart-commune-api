using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SmartCommune.Domain.MenuItemAggregate;
using SmartCommune.Domain.MenuItemAggregate.Entities;
using SmartCommune.Domain.MenuItemAggregate.ValueObjects;
using SmartCommune.Domain.PermissionAggregate.ValueObjects;

namespace SmartCommune.Infrastructure.Persistence.Configurations;

public class MenuItemPermissionConfiguration : IEntityTypeConfiguration<MenuItemPermission>
{
    public void Configure(EntityTypeBuilder<MenuItemPermission> builder)
    {
        builder.ToTable("MenuItemPermissions");

        builder.HasKey(smp => new { smp.MenuItemId, smp.PermissionId });

        builder.Property(smp => smp.MenuItemId)
            .HasConversion(
                id => id.Value,
                value => MenuItemId.Create(value))
            .IsRequired();

        builder.Property(smp => smp.PermissionId)
            .HasConversion(
                id => id.Value,
                value => PermissionId.Create(value))
            .IsRequired();

        // Cấu hình quan hệ với MenuItem.
        builder.HasOne<MenuItem>()
            .WithMany(sm => sm.Permissions)
            .HasForeignKey(smp => smp.MenuItemId)
            .OnDelete(DeleteBehavior.Cascade); // Xóa MenuItem -> Xóa Permissions liên quan.

        // Cấu hình quan hệ với Permission.
        builder.HasOne(smp => smp.Permission)
            .WithMany()
            .HasForeignKey(smp => smp.PermissionId)
            .OnDelete(DeleteBehavior.Restrict); // Không được xóa Permission nếu đang có Menu dùng.
    }
}