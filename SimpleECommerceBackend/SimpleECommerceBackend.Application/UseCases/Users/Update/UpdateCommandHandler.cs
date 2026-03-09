using MediatR;
using SimpleECommerceBackend.Application.Interfaces.Repositories;
using SimpleECommerceBackend.Application.Models.Users.Update;

namespace SimpleECommerceBackend.Application.UseCases.Users.Update;

[AutoConstructor]
public partial class UpdateCommandHandler : IRequestHandler<UpdateCommand, UpdateResult>
{
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IUnitOfWork _unitOfWork;

    public async Task<UpdateResult> Handle(UpdateCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new UpdateResult();
    }
}