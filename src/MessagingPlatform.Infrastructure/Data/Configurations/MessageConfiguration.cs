using System.Text.Json;
using MessagingPlatform.Domain.Entities;
using MessagingPlatform.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MessagingPlatform.Infrastructure.Data.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("Messages");

        builder.HasKey(m => m.Id);

        // Properties
        builder.Property(m => m.ConversationId)
            .IsRequired();

        builder.Property(m => m.SenderId)
            .IsRequired()
            .HasConversion(
                v => v.Value,
                v => UserId.From(v));

        builder.Property(m => m.ParentMessageId)
            .IsRequired(false);

        builder.Property(m => m.ThreadDepth)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(m => m.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(m => m.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(m => m.DeletedAt)
            .IsRequired(false);

        builder.Property(m => m.DeletedBy)
            .IsRequired(false)
            .HasConversion(
                v => v != null ? v.Value : (Guid?)null,
                v => v.HasValue ? UserId.From(v.Value) : null);

        // MessageContent as owned type - SIMPLIFIED
        builder.OwnsOne(m => m.Content, contentBuilder =>
        {
            contentBuilder.Property(c => c.Text)
                .HasColumnName("ContentText")
                .IsRequired()
                .HasMaxLength(5000);
            
            contentBuilder.Property(c => c.Type)
                .HasColumnName("ContentType")
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();
            
            // Metadata as JSON
            contentBuilder.Property(c => c.Metadata)
                .HasColumnName("ContentMetadata")
                .HasColumnType("jsonb");
        });

        // Message Metadata property
        builder.Property(m => m.Metadata)
            .HasColumnName("Metadata")
            .HasColumnType("jsonb");

        // ReadReceipts as owned collection
        builder.OwnsMany(m => m.ReadReceipts, rr =>
        {
            rr.WithOwner().HasForeignKey("MessageId");
            rr.Property<int>("Id").ValueGeneratedOnAdd();
            rr.HasKey("Id");

            rr.Property(r => r.UserId)
                .HasConversion(
                    v => v.Value,
                    v => UserId.From(v));

            rr.Property(r => r.ReadAt)
                .IsRequired();

            rr.HasIndex(r => r.UserId)
                .HasDatabaseName("IX_MessageReadReceipts_UserId");
        });

        // Indexes for performance
        builder.HasIndex(m => new { m.ConversationId, m.CreatedAt })
            .HasDatabaseName("IX_Messages_Conversation_CreatedAt")
            .IsDescending(false, true);

        builder.HasIndex(m => new { m.ParentMessageId, m.ThreadDepth })
            .HasDatabaseName("IX_Messages_Parent_ThreadDepth")
            .HasFilter("\"ParentMessageId\" IS NOT NULL");

        builder.HasIndex(m => m.SenderId)
            .HasDatabaseName("IX_Messages_SenderId");

        // Foreign key to parent message
        builder.HasOne<Message>()
            .WithMany()
            .HasForeignKey(m => m.ParentMessageId)
            .OnDelete(DeleteBehavior.Restrict);

        // Query filter for soft delete
        builder.HasQueryFilter(m => !m.IsDeleted);
    }
}