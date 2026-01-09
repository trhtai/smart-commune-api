using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SmartCommune.Domain.UserAggregate.ValueObjects;
using SmartCommune.Domain.WorkItemAggregate;
using SmartCommune.Domain.WorkItemAggregate.Entities;
using SmartCommune.Domain.WorkItemAggregate.ValueObjects;

namespace SmartCommune.Infrastructure.Persistence.Configurations;

public class WorkItemAttachmentConfiguration : IEntityTypeConfiguration<WorkItemAttachment>
{
    public void Configure(EntityTypeBuilder<WorkItemAttachment> builder)
    {
        builder.ToTable("WorkItemAttachments");

        builder.HasKey(wa => wa.Id);

        builder.Property(wa => wa.Id)
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => WorkItemAttachmentId.Create(value));

        builder.Property(wa => wa.WorkItemId)
            .HasConversion(
                id => id.Value,
                value => WorkItemId.Create(value))
            .IsRequired();

        builder.Property(wa => wa.UploadedById)
            .HasConversion(
                id => id.Value,
                value => ApplicationUserId.Create(value))
            .IsRequired();

        builder.Property(wa => wa.FileName)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(wa => wa.FileUrl)
            .HasMaxLength(2048) // URL có thể khá dài (đặc biệt nếu dùng presigned URL của S3/MinIO).
            .IsRequired();

        builder.Property(wa => wa.FileType)
            .HasMaxLength(100) // Ví dụ: "application/pdf", "image/png"
            .IsRequired();

        builder.Property(wa => wa.FileSize)
            .IsRequired();

        builder.Property(wa => wa.UploadedAt)
            .IsRequired();

        // Cấu hình với WorkItem.
        builder.HasOne<WorkItem>()
            .WithMany(w => w.Attachments)
            .HasForeignKey(w => w.WorkItemId)
            .OnDelete(DeleteBehavior.Cascade); // Xóa work -> xóa các tệp liên quan.
    }
}