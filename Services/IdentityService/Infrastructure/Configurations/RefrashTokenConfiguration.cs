using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.Entities.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens", "dbo");

            builder.HasKey(rt => rt.TokenHash);

            builder.Property(rt => rt.TokenHash)
                .HasMaxLength(256)
                .IsRequired()
                .HasColumnType("varchar(256)")
                .HasColumnName("token_hash");

            builder.HasOne(rt => rt.User)
                .WithMany()
                .HasForeignKey("user_id")
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_RefreshTokens_Users_UserId");

            builder.Property(rt => rt.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            builder.Property(rt => rt.CreateAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            builder.Property(rt => rt.ExpireAt)
                .HasColumnName("expires_at")
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            builder.Property(rt => rt.RevokedAt)
                .HasColumnName("revoked_at")
                .HasColumnType("timestamp with time zone")
                .IsRequired(false);

            builder.Property(rt => rt.DeviceId)
                .HasColumnName("device_id")
                .HasMaxLength(255)
                .IsRequired()
                .HasColumnType("varchar(255)");

            builder.Property(rt => rt.UserAgent)
                .HasColumnName("user_agent")
                .HasMaxLength(500)
                .IsRequired()
                .HasColumnType("varchar(500)");

            builder.Property(rt => rt.IpAddress)
                .HasColumnName("ip_address")
                .HasMaxLength(45)
                .IsRequired()
                .HasColumnType("varchar(45)");

            ConfigureIndexes(builder);
        }

        private void ConfigureIndexes(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.HasIndex("UserId")
                .HasDatabaseName("IX_RefreshTokens_UserId");

            builder.HasIndex("UserId", "ExpireAt")
                .HasDatabaseName("IX_RefreshTokens_UserId_ExpiresAt");

            builder.HasIndex(rt => rt.ExpireAt)
                .HasDatabaseName("IX_RefreshTokens_ExpiresAt");

            builder.HasIndex(rt => rt.IpAddress)
                .HasDatabaseName("IX_RefreshTokens_IpAddress");
        }
    }
}
