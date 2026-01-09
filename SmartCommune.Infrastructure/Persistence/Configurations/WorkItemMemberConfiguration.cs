using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SmartCommune.Domain.MenuItemAggregate;
using SmartCommune.Domain.UserAggregate.ValueObjects;
using SmartCommune.Domain.WorkItemAggregate;
using SmartCommune.Domain.WorkItemAggregate.Entities;
using SmartCommune.Domain.WorkItemAggregate.ValueObjects;

namespace SmartCommune.Infrastructure.Persistence.Configurations;

public class WorkItemMemberConfiguration : IEntityTypeConfiguration<WorkItemMember>
{
    public void Configure(EntityTypeBuilder<WorkItemMember> builder)
    {
        builder.ToTable("WorkItemMembers");

        builder.HasKey(wm => new { wm.WorkItemId, wm.MemberId });

        builder.Property(wm => wm.WorkItemId)
            .HasConversion(
                id => id.Value,
                value => WorkItemId.Create(value))
            .IsRequired();

        builder.Property(wm => wm.MemberId)
            .HasConversion(
                id => id.Value,
                value => ApplicationUserId.Create(value))
            .IsRequired();

        builder.Property(wm => wm.DisplayAlias)
            .HasMaxLength(100);

        // Cấu hình quan hệ với WorkItem.
        builder.HasOne<WorkItem>()
            .WithMany(wm => wm.Members)
            .HasForeignKey(wm => wm.WorkItemId)
            .OnDelete(DeleteBehavior.Cascade); // Xóa Work -> Xóa Members liên quan.

        // Cấu hình quan hệ với User.
        builder.HasOne(wp => wp.Member)
            .WithMany()
            .HasForeignKey(wm => wm.MemberId)
            .OnDelete(DeleteBehavior.Restrict); // Không được xóa User nếu đang tham gia công việc nào đó.
    }
}