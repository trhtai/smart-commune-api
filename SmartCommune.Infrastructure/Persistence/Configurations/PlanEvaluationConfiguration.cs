using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SmartCommune.Domain.PlanAggregate.Entities;
using SmartCommune.Domain.PlanAggregate.ValueObjects;

namespace SmartCommune.Infrastructure.Persistence.Configurations;

public class PlanEvaluationConfiguration : IEntityTypeConfiguration<PlanEvaluation>
{
    public void Configure(EntityTypeBuilder<PlanEvaluation> builder)
    {
        builder.ToTable("PlanEvaluations");

        builder.HasKey(pe => pe.PlanId);

        builder.Property(pe => pe.PlanId)
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => PlanId.Create(value))
            .IsRequired();

        builder.Property(pe => pe.Result)
            .HasMaxLength(2000);

        builder.Property(pe => pe.Strengths)
            .HasMaxLength(2000);

        builder.Property(pe => pe.Weaknesses)
            .HasMaxLength(2000);

        builder.Property(pe => pe.Causes)
            .HasMaxLength(2000);

        builder.Property(pe => pe.LessonsLearned)
            .HasMaxLength(2000);

        builder.Property(pe => pe.Conclusion)
            .HasMaxLength(2000);
    }
}