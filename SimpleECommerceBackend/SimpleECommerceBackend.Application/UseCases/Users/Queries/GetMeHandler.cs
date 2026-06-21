using SimpleECommerceBackend.Application.Interfaces.Security;
using SimpleECommerceBackend.Application.Interfaces.Services.Uam;
using SimpleECommerceBackend.Application.Interfaces.UseCases;
using SimpleECommerceBackend.Application.Models.Users;

namespace SimpleECommerceBackend.Application.UseCases.Users.Queries;

public class GetMeHandler : IUseCaseHandler<GetMeQuery, GetMeResult>
{
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IUserService _userService;

    public GetMeHandler(
        ICurrentUserContext currentUserContext,
        IUserService userService
    )
    {
        _currentUserContext = currentUserContext;
        _userService = userService;
    }

    public async Task<GetMeResult> HandleAsync(
        GetMeQuery request,
        CancellationToken cancellationToken
    )
    {
        return await _userService.GetByIdAsync(_currentUserContext.Id);
    }
}
