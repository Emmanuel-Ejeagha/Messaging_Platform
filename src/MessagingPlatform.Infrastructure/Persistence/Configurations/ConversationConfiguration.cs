using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MessagingPlatform.Domain.Entities;
using MessagingPlatform.Domain.Enums;

namespace MessagingPlatform.Infrastructure.Persistence.Configurations;

public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.ToTable("conversations");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(c => c.Title)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("title");

        builder.Property(c => c.Type)
            .IsRequired()
            .HasConversion(
                v => v.ToString(),
                v => (ConversationType)Enum.Parse(typeof(ConversationType), v))
            .HasColumnName("type");

        builder.Property(c => c.GroupId)
            .HasColumnName("group_id");

        builder.Property(c => c.LastMessageAt)
            .IsRequired()
            .HasColumnName("last_message_at");

        builder.Property(c => c.IsArchived)
            .IsRequired()
            .HasColumnName("is_archived");
        // Add to ConversationConfiguration after IsArchived property configuration
        builder.Property(c => c.IsDeleted)
            .IsRequired()
            .HasColumnName("is_deleted")
            .HasDefaultValue(false);

        builder.Property(c => c.DeletedAt)
            .HasColumnName("deleted_at");

        builder.HasQueryFilter(c => !c.IsArchived && !c.IsDeleted);

        builder.Property(c => c.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        builder.Property(c => c.UpdatedAt)
            .HasColumnName("updated_at");

        // Value objects stored as owned types (if any)

        // Navigation properties
        builder.HasMany(c => c.Participants)
            .WithOne(p => p.Conversation)
            .HasForeignKey(p => p.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Group>()
            .WithOne(g => g.Conversation)
            .HasForeignKey<Conversation>(c => c.GroupId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        builder.HasIndex(c => c.Type);
        builder.HasIndex(c => c.GroupId).IsUnique();
        builder.HasIndex(c => c.LastMessageAt).IsDescending();
        builder.HasIndex(c => new { c.IsArchived, c.LastMessageAt });
    }
}