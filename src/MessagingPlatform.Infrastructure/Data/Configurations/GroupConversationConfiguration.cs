using MessagingPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MessagingPlatform.Infrastructure.Data.Configurations;

public class GroupConversationConfiguration : IEntityTypeConfiguration<GroupConversation>
{
    public void Configure(EntityTypeBuilder<GroupConversation> builder)
    {
        // Configure GroupConversation specific properties
        
        builder.Property(g => g.Name)
            .HasMaxLength(100)
            .HasColumnName("GroupName")
            .IsRequired();

        builder.Property(g => g.Description)
            .HasMaxLength(500)
            .HasColumnName("GroupDescription")
            .IsRequired(false);

        builder.Property(g => g.AvatarUrl)
            .HasMaxLength(500)
            .HasColumnName("GroupAvatarUrl")
            .IsRequired(false);
    }
}