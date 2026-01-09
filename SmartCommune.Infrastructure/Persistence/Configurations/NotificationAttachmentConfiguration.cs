using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SmartCommune.Domain.NotificationAggregate;
using SmartCommune.Domain.NotificationAggregate.Entities;
using SmartCommune.Domain.NotificationAggregate.ValueObjects;

namespace SmartCommune.Infrastructure.Persistence.Configurations;

public class NotificationAttachmentConfiguration : IEntityTypeConfiguration<NotificationAttachment>
{
    public void Configure(EntityTypeBuilder<NotificationAttachment> builder)
    {
        builder.ToTable("NotificationAttachments");

        builder.HasKey(na => na.Id);

        builder.Property(na => na.Id)
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => NotificationAttachmentId.Create(value));

        builder.Property(na => na.NotificationId)
            .IsRequired()
            .HasConversion(
                id => id.Value,
                value => NotificationId.Create(value))
            .IsRequired();

        builder.HasIndex(na => na.NotificationId);

        builder.Property(na => na.FileName)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(na => na.FileUrl)
            .IsRequired()
            .HasMaxLength(2048);

        builder.Property(na => na.FileType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(na => na.FileSize)
            .IsRequired();

        // Cấu hình quan hệ với Notification.
        builder.HasOne<Notification>()
            .WithMany(n => n.Attachments)
            .HasForeignKey(na => na.NotificationId)
            .OnDelete(DeleteBehavior.Cascade); // Xóa thông báo sẽ xóa các tệp đính kèm liên quan.
    }
}