using MessagingPlatform.Domain.Entities;
using MessagingPlatform.Domain.Enums;
using MessagingPlatform.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MessagingPlatform.Infrastructure.Data.Configurations;

public class ParticipantConfiguration : IEntityTypeConfiguration<Participant>
{
    public void Configure(EntityTypeBuilder<Participant> builder)
    {
        builder.ToTable("Participants");

        builder.HasKey(p => p.Id);

        // Composite unique constraint
        builder.HasIndex(p => new { p.ConversationId, p.UserId })
            .IsUnique()
            .HasDatabaseName("IX_Participants_Conversation_User");

        // Properties
        builder.Property(p => p.ConversationId)
            .IsRequired();

        builder.Property(p => p.UserId)
            .IsRequired()
            .HasConversion(
                v => v.Value,
                v => UserId.From(v));

        builder.Property(p => p.Role)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired()
            .HasDefaultValue(ParticipantRole.Member);

        builder.Property(p => p.JoinedAt)
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        builder.Property(p => p.LastReadAt)
            .IsRequired(false);

        builder.Property(p => p.UnreadCount)
            .IsRequired()
            .HasDefaultValue(0);

        // Indexes
        builder.HasIndex(p => p.UserId)
            .HasDatabaseName("IX_Participants_UserId");

        builder.HasIndex(p => new { p.UserId, p.LastReadAt })
            .HasDatabaseName("IX_Participants_User_LastRead");
    }
}