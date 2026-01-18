using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SmartCommune.Domain.RoleAggregate.ValueObjects;
using SmartCommune.Domain.UserAggregate;
using SmartCommune.Domain.UserAggregate.ValueObjects;

namespace SmartCommune.Infrastructure.Persistence.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => ApplicationUserId.Create(value));

        builder.Property(u => u.RoleId)
            .HasConversion(
                id => id.Value,
                value => RoleId.Create(value))
            .IsRequired();

        builder.Property(u => u.UserName)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(u => u.UserName)
            .IsUnique();

        builder.Property(u => u.FullName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(u => u.IsActived)
            .HasDefaultValue(true);

        builder.OwnsOne(
            u => u.PasswordHash,
            passwordBuilder =>
            {
                passwordBuilder.Property(p => p.Value)
                    .HasColumnName("PasswordHash")
                    .HasMaxLength(256)
                    .IsRequired();

                passwordBuilder.Property(p => p.Salt)
                    .HasColumnName("PasswordSalt")
                    .HasMaxLength(100)
                    .IsRequired();
            });

        // Cấu hình quan hệ với Role.
        builder.HasOne(u => u.Role)
            .WithMany()
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        // Backing Fields.
        builder.Navigation(u => u.RefreshTokens)
            .HasField("_refreshTokens")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}