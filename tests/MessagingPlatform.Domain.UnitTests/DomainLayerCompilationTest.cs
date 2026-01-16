using MessagingPlatform.Domain.Entities;
using MessagingPlatform.Domain.ValueObjects;

namespace MessagingPlatform.Domain.UnitTests;

public class DomainLayerCompilationTest
{
    [Fact]
    public void Can_Create_OneToOne_Conversation()
    {
        var user1Id = UserId.New();
        var user2Id = UserId.New();

        var conversation = Conversation.CreatedOneToOne(user1Id, user2Id);

        Assert.NotNull(conversation);
        Assert.Equal(2, conversation.Participants.Count);
        Assert.IsType<OneToOneConversation>(conversation);
    }

    [Fact]
    public void Can_Add_Message_To_Conversation()
    {
        var user1Id = UserId.New();
        var user2Id = UserId.New();
        var conversation = Conversation.CreatedOneToOne(user1Id, user2Id);

        var messageContent = MessageContent.CreateText("Hello World");
        var message = conversation.AddMessage(user1Id, messageContent);

        Assert.NotNull(message);
        Assert.Single(conversation.Messages);
        Assert.Equal("Hello World", message.Content.Text);
    }
}
