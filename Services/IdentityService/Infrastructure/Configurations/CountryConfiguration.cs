using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class CountryConfiguration : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> builder)
        {
            builder.ToTable("countries");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .ValueGeneratedOnAdd();

            builder.OwnsOne(c => c.Name, nameBuilder =>
            {
                nameBuilder.Property(n => n.Value)
                    .HasColumnName("name")
                    .HasMaxLength(100)
                    .IsRequired();

                nameBuilder.HasIndex(n => n.Value)
                    .IsUnique();
            });

            builder.HasIndex(c => c.Id)
                .IsUnique();
        }
    }
}