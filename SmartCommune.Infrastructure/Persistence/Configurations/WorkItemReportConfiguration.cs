using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SmartCommune.Domain.UserAggregate.ValueObjects;
using SmartCommune.Domain.WorkItemAggregate;
using SmartCommune.Domain.WorkItemAggregate.Entities;
using SmartCommune.Domain.WorkItemAggregate.ValueObjects;

namespace SmartCommune.Infrastructure.Persistence.Configurations;

public class WorkItemReportConfiguration : IEntityTypeConfiguration<WorkItemReport>
{
    public void Configure(EntityTypeBuilder<WorkItemReport> builder)
    {
        builder.ToTable("WorkItemReports");

        builder.HasKey(wr => wr.Id);

        builder.Property(wr => wr.Id)
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => WorkItemReportId.Create(value));

        builder.Property(wr => wr.WorkItemId)
            .HasConversion(
                id => id.Value,
                value => WorkItemId.Create(value))
            .IsRequired();

        builder.Property(wr => wr.ReporterId)
            .HasConversion(
                id => id.Value,
                value => ApplicationUserId.Create(value))
            .IsRequired();

        builder.Property(wr => wr.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(wr => wr.Description)
            .HasMaxLength(1000);

        builder.Property(wr => wr.Progress)
            .HasDefaultValue(0);

        // Cấu hình quan hệ với WorkItem.
        builder.HasOne<WorkItem>()
            .WithMany()
            .HasForeignKey(wr => wr.WorkItemId)
            .OnDelete(DeleteBehavior.Cascade); // Xóa WorkItem -> xóa các Reports liên quan.

        builder.HasOne(wr => wr.Reporter)
            .WithMany()
            .HasForeignKey(wr => wr.ReporterId)
            .OnDelete(DeleteBehavior.Restrict); // Chặn xóa User nếu đã từng báo cáo công việc.

        // Backing Fields.
        builder.Navigation(wr => wr.Attachments)
            .HasField("_attachments")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}