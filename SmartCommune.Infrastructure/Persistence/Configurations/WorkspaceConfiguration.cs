using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SmartCommune.Domain.UserAggregate;
using SmartCommune.Domain.UserAggregate.ValueObjects;
using SmartCommune.Domain.WorkspaceAggregate;
using SmartCommune.Domain.WorkspaceAggregate.ValueObjects;

namespace SmartCommune.Infrastructure.Persistence.Configurations;

public class WorkspaceConfiguration : IEntityTypeConfiguration<Workspace>
{
    public void Configure(EntityTypeBuilder<Workspace> builder)
    {
        builder.ToTable("Workspaces");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.Id)
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => WorkspaceId.Create(value));

        builder.Property(w => w.OwnerId)
            .HasConversion(
                id => id.Value,
                value => ApplicationUserId.Create(value))
            .IsRequired();

        builder.Property(w => w.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(w => w.Description)
            .HasMaxLength(500);

        // Cấu hình quan hệ với User (chủ sở hữu workspace này).
        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(w => w.OwnerId)
            .OnDelete(DeleteBehavior.Cascade); // Xóa user -> xóa workspaces liên quan.

        // Backing Fields.
        builder.Navigation(w => w.Collections)
            .HasField("_collections")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}