using ChatService.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatService.Infrastructure.Configurations;

public class ChatMessageConfiguration : IEntityTypeConfiguration<ChatMessage>
{
    public void Configure(EntityTypeBuilder<ChatMessage> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.ChatId).IsRequired();
        builder.Property(m => m.SenderUserId).IsRequired();
        builder.Property(m => m.Text).IsRequired().HasMaxLength(4000);
        builder.Property(m => m.CreatedAt).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(m => m.ReadAt);
        builder.Property(m => m.IsDeleted).IsRequired().HasDefaultValue(false);

        builder.HasIndex(m => new { m.ChatId, m.CreatedAt });
        builder.HasIndex(m => m.SenderUserId);
    }
}
