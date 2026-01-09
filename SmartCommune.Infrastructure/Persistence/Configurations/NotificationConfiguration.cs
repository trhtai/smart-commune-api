using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SmartCommune.Domain.NotificationAggregate;
using SmartCommune.Domain.NotificationAggregate.ValueObjects;
using SmartCommune.Domain.UserAggregate.ValueObjects;

namespace SmartCommune.Infrastructure.Persistence.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications");

        builder.HasKey(n => n.Id);

        builder.Property(n => n.Id)
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => NotificationId.Create(value));

        builder.Property(n => n.SenderId)
            .HasConversion(
                id => id.Value,
                value => ApplicationUserId.Create(value))
            .IsRequired();

        builder.Property(n => n.Title)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(n => n.Content)
            .HasMaxLength(5000);

        builder.Property(n => n.IsGlobal)
            .HasDefaultValue(false);

        // Cấu hình Type - SmartEnum.
        builder.Property(n => n.Type)
            .HasConversion(
                type => type.Value, // 1. Lưu xuống DB: Lấy property .Value (int)
                value => NotificationType.FromValue(value)) // 2. Đọc từ DB: Dùng hàm static FromValue của thư viện.
            .IsRequired();

        // Cấu hình quan hệ với ApplicationUser (Sender).
        builder.HasOne(n => n.Sender)
            .WithMany()
            .HasForeignKey(n => n.SenderId)
            .OnDelete(DeleteBehavior.Cascade); // Xóa User -> Xóa thông báo do user gửi.

        // Backing Fields.
        builder.Navigation(n => n.Attachments)
            .HasField("_attachments")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}