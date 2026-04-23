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
                .HasColumnName("companyId")
                .IsRequired(false);
        }
    }
}