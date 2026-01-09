using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SmartCommune.Domain.WorkItemAggregate.ValueObjects;
using SmartCommune.Domain.WorkspaceAggregate;
using SmartCommune.Domain.WorkspaceAggregate.Entities;
using SmartCommune.Domain.WorkspaceAggregate.ValueObjects;

namespace SmartCommune.Infrastructure.Persistence.Configurations;

public class WorkItemCollectionConfiguration : IEntityTypeConfiguration<WorkItemCollection>
{
    public void Configure(EntityTypeBuilder<WorkItemCollection> builder)
    {
        builder.ToTable("WorkItemCollections");

        builder.HasKey(wc => new { wc.WorkspaceId, wc.WorkItemId });

        builder.Property(wc => wc.WorkspaceId)
            .HasConversion(
                id => id.Value,
                value => WorkspaceId.Create(value))
            .IsRequired();

        builder.Property(wc => wc.WorkItemId)
            .HasConversion(
                id => id.Value,
                value => WorkItemId.Create(value))
            .IsRequired();

        // Cấu hình quan hệ với Workspace.
        builder.HasOne<Workspace>()
            .WithMany(wc => wc.Collections)
            .HasForeignKey(wc => wc.WorkspaceId)
            .OnDelete(DeleteBehavior.Cascade); // Xóa workspace -> xóa item liên quan.

        // Cấu hình quan hệ với WorkItem.
        builder.HasOne(wc => wc.Item)
            .WithMany()
            .HasForeignKey(wc => wc.WorkItemId)
            .OnDelete(DeleteBehavior.Cascade); // Xóa công việc thì xóa luôn item công việc trong workspace.
    }
}