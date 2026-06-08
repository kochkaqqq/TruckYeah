using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.Entities.Configurations
{
    public class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.ToTable("companies");

            builder.HasKey(c => c.Id);

            builder.OwnsOne(c => c.Name, n =>
            {
                n.Property(n => n.Value)
                    .HasColumnName("name")
                    .HasMaxLength(200)
                    .IsRequired()
                    .HasColumnType("varchar(200)");

                n.HasIndex(v => v.Value)
                .IsUnique()
                .HasDatabaseName("UX_companies_name");
            });
        }
    }
}