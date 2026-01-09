using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SmartCommune.Domain.DossierAggregate;
using SmartCommune.Domain.DossierAggregate.Entities;
using SmartCommune.Domain.DossierAggregate.ValueObjects;
using SmartCommune.Domain.UserAggregate.ValueObjects;

namespace SmartCommune.Infrastructure.Persistence.Configurations;

public class DossierMemberConfiguration : IEntityTypeConfiguration<DossierMember>
{
    public void Configure(EntityTypeBuilder<DossierMember> builder)
    {
        builder.ToTable("DossierMembers");

        builder.HasKey(dm => new { dm.DossierId, dm.MemberId });

        builder.Property(dm => dm.DossierId)
            .HasConversion(
                id => id.Value,
                value => DossierId.Create(value))
            .IsRequired();

        builder.Property(dm => dm.MemberId)
            .HasConversion(
                id => id.Value,
                value => ApplicationUserId.Create(value))
            .IsRequired();

        builder.Property(dm => dm.DisplayAlias)
            .HasMaxLength(100);

        // Cấu hình quan hệ với Dossier.
        builder.HasOne<Dossier>()
            .WithMany(dm => dm.Members)
            .HasForeignKey(dm => dm.DossierId)
            .OnDelete(DeleteBehavior.Cascade); // Xóa Dossier -> Xóa Members liên quan.

        // Cấu hình quan hệ với User.
        builder.HasOne(dm => dm.Member)
            .WithMany()
            .HasForeignKey(dm => dm.MemberId)
            .OnDelete(DeleteBehavior.Restrict); // Không được xóa User nếu đang tham gia hồ sơ nào đó.
    }
}