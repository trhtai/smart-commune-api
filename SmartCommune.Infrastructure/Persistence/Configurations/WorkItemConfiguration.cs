using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SmartCommune.Domain.PlanAggregate.ValueObjects;
using SmartCommune.Domain.UserAggregate.ValueObjects;
using SmartCommune.Domain.WorkItemAggregate;
using SmartCommune.Domain.WorkItemAggregate.ValueObjects;

namespace SmartCommune.Infrastructure.Persistence.Configurations;

public class WorkItemConfiguration : IEntityTypeConfiguration<WorkItem>
{
    public void Configure(EntityTypeBuilder<WorkItem> builder)
    {
        builder.ToTable("WorkItems");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.Id)
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => WorkItemId.Create(value));

        builder.Property(w => w.CreatedById)
            .HasConversion(
                id => id.Value,
                value => ApplicationUserId.Create(value))
            .IsRequired();

        builder.Property(w => w.ParentId)
            .HasConversion(
                id => id != null ? id.Value : (Guid?)null,
                value => value != null ? WorkItemId.Create(value.Value) : null);

        builder.Property(w => w.PlanId)
            .HasConversion(
                id => id != null ? id.Value : (Guid?)null,
                value => value != null ? PlanId.Create(value.Value) : null);

        builder.Property(w => w.Title)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(w => w.Description)
            .HasMaxLength(2000);

        builder.Property(w => w.ExpectedResult)
            .HasMaxLength(1000);

        builder.Property(w => w.Progress)
            .HasDefaultValue(0);

        builder.Property(w => w.Workload)
            .HasDefaultValue(0);

        builder.Property(w => w.IsNotifiedBeforeDeadline)
            .HasDefaultValue(false);

        builder.Property(w => w.IsNotifiedExpired)
            .HasDefaultValue(false);

        builder.Property(x => x.Status)
            .HasConversion(
                p => p.Value,
                v => WorkItemStatus.FromValue(v))
            .HasDefaultValue(WorkItemStatus.Todo)
            .IsRequired();

        builder.Property(x => x.Priority)
            .HasConversion(
                p => p.Value,
                v => WorkItemPriority.FromValue(v))
            .HasDefaultValue(WorkItemPriority.Low)
            .IsRequired();

        builder.OwnsOne(x => x.Timeline, tlb =>
        {
            tlb.Property(d => d.StartDate)
                .HasColumnName("StartDate")
                .IsRequired();

            tlb.Property(d => d.EndDate)
                .HasColumnName("EndDate")
                .IsRequired();
        });

        // Cấu hình quan hệ công việc cha - con.
        builder.HasOne<WorkItem>()
            .WithMany()
            .HasForeignKey(x => x.ParentId)
            .OnDelete(DeleteBehavior.Restrict); // Tránh cascade delete vòng tròn.

        // Cấu hình quan hệ với User (người tạo công việc).
        builder.HasOne(w => w.CreatedBy)
            .WithMany()
            .HasForeignKey(x => x.CreatedById)
            .OnDelete(DeleteBehavior.Restrict); // Rất nguy hiểm nếu 1 user bị xóa dẫn đến cả nghìn công việc mất theo.

        // Cấu hình quan hệ với Plan.
        builder.HasOne(w => w.Plan)
            .WithMany()
            .HasForeignKey(x => x.PlanId)
            .OnDelete(DeleteBehavior.Restrict); // Tránh xóa nhầm công việc khi xóa kế hoạch.

        // Backing Fields.
        builder.Navigation(w => w.Attachments)
            .HasField("_attachments")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(w => w.Members)
            .HasField("_members")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}