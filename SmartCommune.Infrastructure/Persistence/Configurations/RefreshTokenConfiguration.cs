using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SmartCommune.Domain.UserAggregate;
using SmartCommune.Domain.UserAggregate.Entities;
using SmartCommune.Domain.UserAggregate.ValueObjects;

namespace SmartCommune.Infrastructure.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");

        builder.HasKey(rt => rt.Id);

        builder.Property(rt => rt.Id)
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => RefreshTokenId.Create(value));

        builder.Property(rt => rt.UserId)
            .HasConversion(
                id => id.Value,
                value => ApplicationUserId.Create(value))
            .IsRequired();

        builder.Property(rt => rt.Token)
            .HasMaxLength(256)
            .IsRequired();

        builder.HasIndex(rt => rt.Token);

        // Cấu hình quan hệ với User.
        builder.HasOne<ApplicationUser>()
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}