using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SmartCommune.Domain.WorkItemAggregate.Entities;
using SmartCommune.Domain.WorkItemAggregate.ValueObjects;

namespace SmartCommune.Infrastructure.Persistence.Configurations;

public class WorkItemReportAttachmentConfiguration : IEntityTypeConfiguration<WorkItemReportAttachment>
{
    public void Configure(EntityTypeBuilder<WorkItemReportAttachment> builder)
    {
        builder.ToTable("WorkItemReportAttachments");

        builder.HasKey(wra => wra.Id);

        builder.Property(wra => wra.Id)
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => WorkItemReportAttachmentId.Create(value));

        builder.Property(wra => wra.WorkItemReportId)
            .HasConversion(
                id => id.Value,
                value => WorkItemReportId.Create(value))
            .IsRequired();

        builder.Property(wra => wra.FileName)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(wra => wra.FileUrl)
            .HasMaxLength(2000) // URL có thể khá dài (đặc biệt nếu dùng presigned URL của S3/MinIO).
            .IsRequired();

        builder.Property(wra => wra.FileType)
            .HasMaxLength(100) // Ví dụ: "application/pdf", "image/png"
            .IsRequired();

        builder.Property(wra => wra.FileSize)
            .IsRequired();

        builder.Property(wra => wra.UploadedAt)
            .IsRequired();

        // Cấu hình quan hệ với WorkItemReprot.
        builder.HasOne<WorkItemReport>()
            .WithMany(wr => wr.Attachments)
            .HasForeignKey(wra => wra.WorkItemReportId)
            .OnDelete(DeleteBehavior.Cascade); // Xóa report -> xóa các tệp liên quan.
    }
}