using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SmartCommune.Domain.PlanAggregate;
using SmartCommune.Domain.PlanAggregate.Entities;
using SmartCommune.Domain.PlanAggregate.ValueObjects;
using SmartCommune.Domain.UserAggregate.ValueObjects;
using SmartCommune.Domain.WorkItemAggregate;

namespace SmartCommune.Infrastructure.Persistence.Configurations;

public class PlanConfiguration : IEntityTypeConfiguration<Plan>
{
    public void Configure(EntityTypeBuilder<Plan> builder)
    {
        builder.ToTable("Plans");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => PlanId.Create(value));

        builder.Property(p => p.CreatedById)
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => ApplicationUserId.Create(value))
            .IsRequired();

        builder.Property(p => p.ParentId)
            .HasConversion(
                id => id != null ? id.Value : (Guid?)null,
                value => value != null ? PlanId.Create(value.Value) : null);

        builder.Property(p => p.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Status)
            .HasConversion(
                p => p.Value,
                v => PlanStatus.FromValue(v))
            .HasDefaultValue(PlanStatus.Todo)
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

        // Cấu hình quan hệ kế hoạch cha - con.
        builder.HasOne<Plan>()
            .WithMany(p => p.SubPlans)
            .HasForeignKey(p => p.ParentId)
            .OnDelete(DeleteBehavior.Restrict); // Tránh cascade delete vòng tròn.

        // Cấu hình quan hệ một - một giữa Plan và PlanEvaluation.
        builder.HasOne(p => p.Evaluation)
           .WithOne(e => e.Plan)
           .HasForeignKey<PlanEvaluation>(e => e.PlanId) // FK trỏ vào PlanId của bảng Evaluation.
           .OnDelete(DeleteBehavior.Cascade);

        // Cấu hình quan hệ nhiều - một giữa Plan và ApplicationUser (người tạo).
        builder.HasOne(p => p.CreatedBy)
            .WithMany()
            .HasForeignKey(p => p.CreatedById)
            .OnDelete(DeleteBehavior.Restrict); // Tránh xóa người dùng khi xóa kế hoạch.
    }
}