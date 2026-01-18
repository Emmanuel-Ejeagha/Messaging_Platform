using MediatR;
using MessagingPlatform.Application.Common.Exceptions;
using MessagingPlatform.Application.Common.Interfaces;
using MessagingPlatform.Domain.ValueObjects;

namespace MessagingPlatform.Application.Groups.Commands.DeleteGroup;

public record DeleteGroupCommand : IRequest
{
    public Guid GroupId { get; init; }
    public Guid UserId { get; init; }
}

public class DeleteGroupCommandHandler : IRequestHandler<DeleteGroupCommand>
{
    private readonly IGroupRepository _groupRepository;

    public DeleteGroupCommandHandler(IGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }

    public async Task Handle(DeleteGroupCommand request, CancellationToken cancellationToken)
    {
        var group = await _groupRepository.GetByIdAsync(request.GroupId, cancellationToken);

        if (group == null)
            throw new NotFoundException(nameof(Domain.Entities.Group), request.GroupId);

        group.Delete(UserId.Create(request.UserId));
        
        _groupRepository.Update(group);
    }
}