using ChatService.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatService.Infrastructure.Configurations;

public class ChatParticipantConfiguration : IEntityTypeConfiguration<ChatParticipant>
{
    public void Configure(EntityTypeBuilder<ChatParticipant> builder)
    {
        builder.HasKey(p => new { p.ChatId, p.UserId });

        builder.Property(p => p.UserId).IsRequired();
        builder.Property(p => p.LastReadAt);
        builder.Property(p => p.UnreadCount).IsRequired().HasDefaultValue(0);

        builder.HasIndex(p => p.UserId);
    }
}
