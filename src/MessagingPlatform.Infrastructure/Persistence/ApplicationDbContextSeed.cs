using MessagingPlatform.Domain.Entities;
using MessagingPlatform.Domain.ValueObjects;

namespace MessagingPlatform.Infrastructure.Persistence;

public static class ApplicationDbContextSeed
{
    public static async Task SeedSampleDataAsync(ApplicationDbContext context)
    {
        // Only seed if database is empty
        if (!context.Conversations.Any())
        {
            // Create sample users
            var user1 = UserId.Create(Guid.Parse("00000000-0000-0000-0000-000000000001"));
            var user2 = UserId.Create(Guid.Parse("00000000-0000-0000-0000-000000000002"));
            var user3 = UserId.Create(Guid.Parse("00000000-0000-0000-0000-000000000003"));

            // Create a one-on-one conversation
            var conversation1 = Conversation.CreateOneOnOne(user1, user2);
            await context.Conversations.AddAsync(conversation1);

            // Create a group conversation
            var group = Group.Create("Development Team", user1, "Team for development discussions");
            group.AddMember(user2);
            group.AddMember(user3);
            await context.Groups.AddAsync(group);

            var conversation2 = Conversation.CreateGroup("Development Team Chat", user1);
            conversation2.AddParticipant(user2, Domain.Enums.ParticipantRole.Member);
            conversation2.AddParticipant(user3, Domain.Enums.ParticipantRole.Member);
            await context.Conversations.AddAsync(conversation2);

            // Link group to conversation
            conversation2.GetType().GetProperty("GroupId")?.SetValue(conversation2, group.Id);

            // Add some sample messages
            var message1 = Message.Create(
                conversation1.Id,
                user1,
                "Hello there!",
                null,
                null);
            await context.Messages.AddAsync(message1);
            conversation1.AddMessage(message1.Id);

            var message2 = Message.Create(
                conversation1.Id,
                user2,
                "Hi! How are you?",
                null,
                null);
            await context.Messages.AddAsync(message2);
            conversation1.AddMessage(message2.Id);

            var reply1 = Message.CreateReply(
                conversation1.Id,
                user1,
                message2.Id,
                "I'm doing great, thanks!");
            await context.Messages.AddAsync(reply1);
            conversation1.AddMessage(reply1.Id);

            await context.SaveChangesAsync();
        }
    }
}