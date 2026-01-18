using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MessagingPlatform.Domain.Entities;

namespace MessagingPlatform.Infrastructure.Persistence.Configurations;

public class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder.ToTable("groups");

        builder.HasKey(g => g.Id);
        builder.Property(g => g.Id)
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(g => g.Name)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnName("name");

        builder.Property(g => g.Description)
            .HasMaxLength(500)
            .HasColumnName("description");

        builder.Property(g => g.OwnerId)
            .IsRequired()
            .HasColumnName("owner_id");

        builder.Property(g => g.AvatarUrl)
            .HasMaxLength(2048)
            .HasColumnName("avatar_url");

        builder.Property(g => g.IsPublic)
            .IsRequired()
            .HasColumnName("is_public");

        builder.Property(g => g.MaxMembers)
            .IsRequired()
            .HasColumnName("max_members")
            .HasDefaultValue(100);
        builder.Property(g => g.IsDeleted)
            .IsRequired()
            .HasColumnName("is_deleted")
            .HasDefaultValue(false);

        builder.Property(g => g.DeletedAt)
            .HasColumnName("deleted_at");

        // Add query filter for soft delete
        builder.HasQueryFilter(g => !g.IsDeleted);

                builder.Property(g => g.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        builder.Property(g => g.UpdatedAt)
            .HasColumnName("updated_at");

        // Map the private backing field for MemberIds
        var memberIdsComparer = new ValueComparer<List<Guid>>(
            (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList());

        builder.Property<List<Guid>>("_memberIds")
            .HasColumnName("member_ids")
            .HasColumnType("jsonb")
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<List<Guid>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<Guid>(),
                memberIdsComparer);

        // Navigation property
        builder.HasOne(g => g.Conversation)
            .WithOne()
            .HasForeignKey<Conversation>(c => c.GroupId)
            .IsRequired(false);

        // Indexes
        builder.HasIndex(g => g.OwnerId);
        builder.HasIndex(g => g.IsPublic);
        builder.HasIndex(g => g.Name);
        builder.HasIndex(g => g.CreatedAt).IsDescending();
        
        // GIN index for JSONB column for efficient member queries
        builder.HasIndex("_memberIds")
            .HasMethod("gin")
            .HasOperators("jsonb_path_ops");
    }
}