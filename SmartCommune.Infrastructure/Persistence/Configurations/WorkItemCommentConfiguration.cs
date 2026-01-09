using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SmartCommune.Domain.MenuItemAggregate.ValueObjects;
using SmartCommune.Domain.UserAggregate.ValueObjects;
using SmartCommune.Domain.WorkItemAggregate;
using SmartCommune.Domain.WorkItemAggregate.Entities;
using SmartCommune.Domain.WorkItemAggregate.ValueObjects;

namespace SmartCommune.Infrastructure.Persistence.Configurations;

public class WorkItemCommentConfiguration : IEntityTypeConfiguration<WorkItemComment>
{
    public void Configure(EntityTypeBuilder<WorkItemComment> builder)
    {
        builder.ToTable("WorkItemComments");

        builder.HasKey(wc => wc.Id);

        builder.Property(wc => wc.Id)
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => WorkItemCommentId.Create(value));

        builder.Property(wc => wc.WorkItemId)
            .HasConversion(
                id => id.Value,
                value => WorkItemId.Create(value))
            .IsRequired();

        builder.Property(wc => wc.UserId)
            .HasConversion(
                id => id.Value,
                value => ApplicationUserId.Create(value))
            .IsRequired();

        builder.Property(wc => wc.ParentId)
            .HasConversion(
                id => id != null ? id.Value : (Guid?)null,
                value => value != null ? WorkItemCommentId.Create(value.Value) : null);

        builder.Property(wc => wc.Content)
            .HasMaxLength(1000)
            .IsRequired();

        // Cấu hình quan hệ bình luận cha - con.
        builder.HasOne<WorkItemComment>()
            .WithMany()
            .HasForeignKey(x => x.ParentId)
            .OnDelete(DeleteBehavior.Restrict); // Tránh cascade delete vòng tròn.

        // Cấu hình quan hệ với WorkItem.
        builder.HasOne<WorkItem>()
            .WithMany()
            .HasForeignKey(wc => wc.WorkItemId)
            .OnDelete(DeleteBehavior.Cascade); // Xóa công việc -> xóa các bình luận liên quan.

        // Cấu hình quan hệ với User (người comment).
        builder.HasOne(wc => wc.User)
            .WithMany()
            .HasForeignKey(wc => wc.UserId)
            .OnDelete(DeleteBehavior.Restrict); // Ngăn xóa User nếu đã bình luận ở đâu đó.
    }
}