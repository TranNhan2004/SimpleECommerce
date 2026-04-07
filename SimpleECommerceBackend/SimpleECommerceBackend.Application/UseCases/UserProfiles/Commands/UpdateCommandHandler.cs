using MediatR;
using SimpleECommerceBackend.Application.Interfaces.Repositories;
using SimpleECommerceBackend.Application.Interfaces.Repositories.Business;
using SimpleECommerceBackend.Application.Models.UserProfiles;

namespace SimpleECommerceBackend.Application.UseCases.UserProfiles.Commands;

[AutoConstructor]
public partial class UpdateCommandHandler : IRequestHandler<UpdateCommand, UpdateResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserProfileRepository _userProfileRepository;

    public async Task<UpdateResult> Handle(UpdateCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new UpdateResult();
    }
}