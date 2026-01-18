using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MessagingPlatform.Domain.Entities;
using MessagingPlatform.Domain.Enums;

namespace MessagingPlatform.Infrastructure.Persistence.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("messages");

        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id)
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(m => m.ConversationId)
            .IsRequired()
            .HasColumnName("conversation_id");

        builder.Property(m => m.SenderId)
            .IsRequired()
            .HasColumnName("sender_id");

        // Convert MessageContent value object
        builder.OwnsOne(m => m.Content, content =>
        {
            content.Property(c => c.Content)
                .IsRequired()
                .HasMaxLength(5000)
                .HasColumnName("content");

            content.Property(c => c.MediaUrl)
                .HasMaxLength(2048)
                .HasColumnName("media_url");
        });

        builder.Property(m => m.Status)
            .IsRequired()
            .HasConversion(
                v => v.ToString(),
                v => (MessageStatus)Enum.Parse(typeof(MessageStatus), v))
            .HasColumnName("status");

        builder.Property(m => m.ParentMessageId)
            .HasColumnName("parent_message_id");

        builder.Property(m => m.IsEdited)
            .IsRequired()
            .HasColumnName("is_edited");
        
        builder.Property(m => m.IsDeleted)
            .IsRequired()
            .HasColumnName("is_deleted")
            .HasDefaultValue(false);

        builder.Property(m => m.DeletedAt)
            .HasColumnName("deleted_at");

        // Add query filter for soft delete
        builder.HasQueryFilter(m => !m.IsDeleted);

        builder.Property(m => m.ReadAt)
            .HasColumnName("read_at");

        builder.Property(m => m.DeliveredAt)
            .HasColumnName("delivered_at");

        builder.Property(m => m.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        builder.Property(m => m.UpdatedAt)
            .HasColumnName("updated_at");

        // Self-referencing relationship for threading
        builder.HasOne(m => m.ParentMessage)
            .WithMany(m => m.Replies)
            .HasForeignKey(m => m.ParentMessageId)
            .OnDelete(DeleteBehavior.Restrict);

        // Navigation to conversation
        builder.HasOne<Conversation>()
            .WithMany()
            .HasForeignKey(m => m.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);
        
        

        // Indexes for common queries
        builder.HasIndex(m => m.ConversationId);
        builder.HasIndex(m => m.SenderId);
        builder.HasIndex(m => m.ParentMessageId);
        builder.HasIndex(m => m.Status);
        builder.HasIndex(m => m.CreatedAt).IsDescending();
        builder.HasIndex(m => new { m.ConversationId, m.CreatedAt }).IsDescending();
        builder.HasIndex(m => new { m.ConversationId, m.Status, m.CreatedAt });
    }
}