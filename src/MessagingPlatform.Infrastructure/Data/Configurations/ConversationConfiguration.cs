using System;
using MessagingPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MessagingPlatform.Infrastructure.Data.Configurations;

public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.ToTable("Conversations");

        builder.HasKey(c => c.Id);

        // Discriminator for inheritance (TPH - Table Per Hierarchy)
        builder.HasDiscriminator<ConversationType>("ConversationType")
            .HasValue<OneToOneConversation>(ConversationType.OneToOne)
            .HasValue<GroupConversation>(ConversationType.Group);

        // Properties
        builder.Property(c => c.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(c => c.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(c => c.DeletedAt)
            .IsRequired(false);

        builder.Property(c => c.LastMessageAt)
            .IsRequired(false);

        // Indexes
        builder.HasIndex(c => new { c.IsDeleted, c.LastMessageAt })
            .HasDatabaseName("IX_Conversations_Deleted_LastMessage");

        // Navigation properties
        builder.HasMany(c => c.Messages)
            .WithOne()
            .HasForeignKey(m => m.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Participants)
            .WithOne()
            .HasForeignKey(p => p.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Query filter for soft delete
        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}