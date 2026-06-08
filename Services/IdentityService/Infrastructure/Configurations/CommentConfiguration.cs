using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.ToTable("comments");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.AutorId)
                .HasColumnName("autor_id")
                .IsRequired();

            builder.Property(c => c.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            builder.Property(c => c.Rating)
                .HasColumnName("rating")
                .IsRequired()
                .HasDefaultValue(0.0f)
                .HasColumnType("float");

            builder.OwnsOne(c => c.CommentText, commentTextBuilder =>
            {
                commentTextBuilder.Property(ct => ct.Value)
                    .HasColumnName("comment_text")
                    .HasMaxLength(1000)
                    .IsRequired()
                    .HasColumnType("varchar(1000)");
            });

            builder.HasIndex(c => c.AutorId)
                .HasDatabaseName("IX_comments_autor_id");

            builder.HasIndex(c => c.UserId)
                .HasDatabaseName("IX_comments_user_id");

            builder.HasIndex(c => new { c.AutorId, c.UserId })
                .HasDatabaseName("IX_comments_autor_id_user_id");
        }
    }
}