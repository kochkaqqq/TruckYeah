using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.Entities.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Id)
                .HasColumnName("id")
                .IsRequired();

            builder.Property(u => u.PasswordHash)
                .HasColumnName("password_hash")
                .HasMaxLength(255)
                .IsRequired()
                .HasColumnType("varchar(255)");

            builder.OwnsOne(u => u.Email, emailBuilder =>
            {
                emailBuilder.Property(e => e.Value)
                    .HasColumnName("email")
                    .HasMaxLength(255)
                    .IsRequired()
                    .HasColumnType("varchar(255)");
            });

            builder.OwnsOne(u => u.Phone, phoneBuilder =>
            {
                phoneBuilder.Property(p => p.CountryCode)
                    .HasColumnName("phone_country_code")
                    .HasMaxLength(4)
                    .IsRequired()
                    .HasColumnType("varchar(4)");

                phoneBuilder.Property(p => p.Number)
                    .HasColumnName("phone_number")
                    .HasMaxLength(10)
                    .IsRequired()
                    .HasColumnType("char(10)")
                    .IsFixedLength();
            });

            builder.HasOne(u => u.Country)
                .WithMany()
                .HasForeignKey("countryId")
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Users_Countries_CountryId");

            builder.Property<Guid>("countryId")
                .HasColumnName("country_id")
                .IsRequired();

            builder.OwnsOne(u => u.Postcode, postcodeBuilder =>
            {
                postcodeBuilder.Property(p => p.Value)
                    .HasColumnName("postcode")
                    .HasMaxLength(10)
                    .IsRequired()
                    .HasColumnType("varchar(10)");
            });

            builder.OwnsOne(u => u.City, cityBuilder =>
            {
                cityBuilder.Property(c => c.Value)
                    .HasColumnName("city")
                    .HasMaxLength(100)
                    .IsRequired(false)
                    .HasColumnType("varchar(100)");
            });

            builder.Property(u => u.UserType)
                .HasColumnName("user_type")
                .IsRequired()
                .HasConversion<string>()
                .HasColumnType("varchar(50)");

            builder.OwnsOne(u => u.VatId, vatIdBuilder =>
            {
                vatIdBuilder.Property(v => v.Value)
                    .HasColumnName("vat_id")
                    .HasMaxLength(15)
                    .IsRequired(false)
                    .HasColumnType("varchar(15)");
            });

            builder.OwnsOne(u => u.FullName, fullNameBuilder =>
            {
                fullNameBuilder.Property(f => f.FirstName)
                    .HasColumnName("first_name")
                    .HasMaxLength(50)
                    .IsRequired();

                fullNameBuilder.Property(f => f.LastName)
                    .HasColumnName("last_name")
                    .HasMaxLength(50)
                    .IsRequired();

                fullNameBuilder.Property(f => f.MiddleName)
                    .HasColumnName("middle_name")
                    .HasMaxLength(50)
                    .IsRequired(false);
            });

            builder.HasOne(u => u.Company)
                .WithMany()
                .HasForeignKey("companyId")
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Users_Companies_CompanyId");

            builder.Property<Guid?>("companyId")
                .HasColumnName("company_id")
                .IsRequired(false);

            builder.Property(u => u.AvatarLink)
                .HasColumnName("avatar_link")
                .HasMaxLength(500)
                .IsRequired(false)
                .HasColumnType("varchar(500)");

            builder.Property(u => u.Rating)
                .HasColumnName("rating")
                .IsRequired()
                .HasDefaultValue(0.0f)
                .HasColumnType("float");

            builder.Property(u => u.Role)
                .HasColumnName("role")
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired()
                .HasDefaultValue(Domain.Enums.AccountRole.User);

            builder.Property(u => u.Status)
                .HasColumnName("status")
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired()
                .HasDefaultValue(Domain.Enums.AccountStatus.Active);

            builder.Property(u => u.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp with time zone")
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.HasIndex(u => u.Role);
            builder.HasIndex(u => u.Status);
        }
    }
}
