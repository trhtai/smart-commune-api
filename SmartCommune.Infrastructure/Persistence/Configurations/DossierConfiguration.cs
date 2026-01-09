using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SmartCommune.Domain.DossierAggregate;
using SmartCommune.Domain.DossierAggregate.ValueObjects;
using SmartCommune.Domain.UserAggregate.ValueObjects;

namespace SmartCommune.Infrastructure.Persistence.Configurations;

public class DossierConfiguration : IEntityTypeConfiguration<Dossier>
{
    public void Configure(EntityTypeBuilder<Dossier> builder)
    {
        builder.ToTable("Dossiers");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Id)
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => DossierId.Create(value));

        builder.Property(d => d.CreatedById)
            .HasConversion(
                id => id.Value,
                value => ApplicationUserId.Create(value))
            .IsRequired();

        builder.Property(d => d.DossierNumber)
            .HasMaxLength(50);

        builder.Property(d => d.Title)
            .HasMaxLength(200);

        builder.Property(d => d.Note)
            .HasMaxLength(1000);

        builder.Property(x => x.Status)
            .HasConversion(
                p => p.Value,
                v => DossierStatus.FromValue(v))
            .HasDefaultValue(DossierStatus.Todo)
            .IsRequired();

        builder.Property(x => x.Priority)
            .HasConversion(
                p => p.Value,
                v => DossierPriority.FromValue(v))
            .HasDefaultValue(DossierPriority.Low)
            .IsRequired();

        // Cấu hình quan hệ với User (người tạo hồ sơ).
        builder.HasOne(d => d.CreatedBy)
            .WithMany()
            .HasForeignKey(x => x.CreatedById)
            .OnDelete(DeleteBehavior.Restrict); // Rất nguy hiểm nếu 1 user bị xóa dẫn đến cả nghìn hồ sơ mất theo.

        // Backing Fields.
        builder.Navigation(d => d.Attachments)
            .HasField("_attachments")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(d => d.Members)
            .HasField("_members")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}