using SimpleECommerceBackend.Application.Interfaces.UseCases;
using SimpleECommerceBackend.Application.Models.UserProfiles;

namespace SimpleECommerceBackend.Application.UseCases.UserProfiles.Commands;

public class DeleteMyProfileHandler : IUseCaseHandler<DeleteMyProfileCommand>
{
    public async Task HandleAsync(
        DeleteMyProfileCommand request,
        CancellationToken cancellationToken
    )
    {
    }
}