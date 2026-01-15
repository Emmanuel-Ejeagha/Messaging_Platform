using System;
using AutoMapper;
using MessagingPlatform.Application.Features.Conversations.DTOs;
using MessagingPlatform.Application.Features.Messages.DTOs;
using MessagingPlatform.Domain.Entities;

namespace MessagingPlatform.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Conversation mappings
        CreateMap<Conversation, ConversationDto>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src =>
                (src as GroupConversation) != null ? ((GroupConversation)src).Name : null))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src =>
                (src as GroupConversation) != null ? ((GroupConversation)src).Description : null))
            .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src =>
                (src as GroupConversation) != null ? ((GroupConversation)src).AvatarUrl : null));

        CreateMap<GroupConversation, ConversationDto>()
            .IncludeBase<Conversation, ConversationDto>();

        CreateMap<Participant, ParticipantDto>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));

        // Message mappings
        CreateMap<Message, MessageDto>()
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content.Text))
            .ForMember(dest => dest.ContentType, opt => opt.MapFrom(src => src.ContentType.ToString()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<MessageReadReceipt, ReadReceiptDto>();
            
    }
}
