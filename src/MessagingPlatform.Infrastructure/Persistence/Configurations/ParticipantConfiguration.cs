using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MessagingPlatform.Domain.Entities;
using MessagingPlatform.Domain.Enums;

namespace MessagingPlatform.Infrastructure.Persistence.Configurations;

public class ParticipantConfiguration : IEntityTypeConfiguration<Participant>
{
    public void Configure(EntityTypeBuilder<Participant> builder)
    {
        builder.ToTable("participants");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(p => p.UserId)
            .IsRequired()
            .HasColumnName("user_id");

        builder.Property(p => p.ConversationId)
            .IsRequired()
            .HasColumnName("conversation_id");

        builder.Property(p => p.Role)
            .IsRequired()
            .HasConversion(
                v => v.ToString(),
                v => (ParticipantRole)Enum.Parse(typeof(ParticipantRole), v))
            .HasColumnName("role");

        builder.Property(p => p.JoinedAt)
            .IsRequired()
            .HasColumnName("joined_at");

        builder.Property(p => p.LeftAt)
            .HasColumnName("left_at");

        builder.Property(p => p.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        builder.Property(p => p.UpdatedAt)
            .HasColumnName("updated_at");

        // Composite unique constraint - a user can only be a participant once per conversation
        builder.HasIndex(p => new { p.UserId, p.ConversationId })
            .IsUnique()
            .HasFilter("left_at IS NULL");

        // Indexes
        builder.HasIndex(p => p.UserId);
        builder.HasIndex(p => p.ConversationId);
        builder.HasIndex(p => p.Role);
        builder.HasIndex(p => p.LeftAt);
        builder.HasIndex(p => new { p.ConversationId, p.LeftAt });
    }
}