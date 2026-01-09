using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SmartCommune.Domain.NotificationAggregate.Entities;
using SmartCommune.Domain.NotificationAggregate.ValueObjects;
using SmartCommune.Domain.UserAggregate;
using SmartCommune.Domain.UserAggregate.ValueObjects;

namespace SmartCommune.Infrastructure.Persistence.Configurations;

public class UserNotificationConfiguration : IEntityTypeConfiguration<UserNotification>
{
    public void Configure(EntityTypeBuilder<UserNotification> builder)
    {
        builder.ToTable("UserNotifications");

        builder.HasKey(un => un.Id);

        builder.Property(un => un.Id)
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => UserNotificationId.Create(value));

        builder.Property(un => un.ReceiverId)
            .HasConversion(
                id => id.Value,
                value => ApplicationUserId.Create(value))
            .IsRequired();

        builder.Property(un => un.NotificationId)
            .HasConversion(
                id => id.Value,
                value => NotificationId.Create(value))
            .IsRequired();

        // Cấu hình quan hệ với Notification.
        builder.HasOne(un => un.Notification)
            .WithMany()
            .HasForeignKey(un => un.NotificationId)
            .OnDelete(DeleteBehavior.Cascade); // Khi Notification bị xóa, các UserNotification liên quan cũng bị xóa.

        // Cấu hình quan hệ với ApplicationUser (Người nhận).
        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(un => un.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict); // Không cho phép xóa User nếu còn thông báo liên quan.

        // Tối ưu hiệu năng.
        // 1. Lấy danh sách thông báo của người dùng.
        // Sắp xếp giảm dần theo thời gian để lấy 10 thông báo mới nhất chẳng hạn.
        builder.HasIndex(un => new { un.ReceiverId, un.ReceivedAt });

        // 2. Đảm bảo user không nhận trùng lặp cùng một thông báo.
        builder.HasIndex(un => new { un.ReceiverId, un.NotificationId }).IsUnique();

        // 3. Đếm số lượng thông báo chưa đọc.
        builder.HasIndex(un => new { un.ReceiverId, un.IsRead });
    }
}