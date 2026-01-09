using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SmartCommune.Domain.DossierAggregate;
using SmartCommune.Domain.DossierAggregate.Entities;
using SmartCommune.Domain.DossierAggregate.ValueObjects;

namespace SmartCommune.Infrastructure.Persistence.Configurations;

public class DossierAttachmentConfiguration : IEntityTypeConfiguration<DossierAttachment>
{
    public void Configure(EntityTypeBuilder<DossierAttachment> builder)
    {
        builder.ToTable("DossierAttachments");

        builder.HasKey(da => da.Id);

        builder.Property(da => da.Id)
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => DossierAttachmentId.Create(value));

        builder.Property(da => da.DossierId)
            .HasConversion(
                id => id.Value,
                value => DossierId.Create(value))
            .IsRequired();

        builder.Property(da => da.FileName)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(da => da.FileUrl)
            .IsRequired()
            .HasMaxLength(2048);

        builder.Property(da => da.FileType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(da => da.FileSize)
            .IsRequired();

        // Cấu hình với Dossier.
        builder.HasOne<Dossier>()
            .WithMany(d => d.Attachments)
            .HasForeignKey(da => da.DossierId)
            .OnDelete(DeleteBehavior.Cascade); // Xóa dossier -> xóa các tệp liên quan.
    }
}