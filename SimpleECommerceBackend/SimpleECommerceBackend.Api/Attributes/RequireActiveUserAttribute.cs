using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SimpleECommerceBackend.Application.Interfaces.Contexts;
using SimpleECommerceBackend.Application.Interfaces.Services.Business;
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Api.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class RequireActiveUserAttribute : TypeFilterAttribute
{
    public RequireActiveUserAttribute() : base(typeof(RequireActiveUserFilter))
    {
    }
}

internal sealed class RequireActiveUserFilter : IAsyncActionFilter
{
    private readonly IUserContextHolder _userContextHolder;
    private readonly IUserProfileService _userProfileService;

    public RequireActiveUserFilter(
        IUserContextHolder userContextHolder,
        IUserProfileService userProfileService
    )
    {
        _userContextHolder = userContextHolder;
        _userProfileService = userProfileService;
    }

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next
    )
    {
        var userContext = _userContextHolder.GetUserContext();
        var isActive = await _userProfileService.IsActiveUserAsync(userContext.Id);

        if (!isActive)
            throw new UnauthorizedException(
                UserProfileErrorCodes.InactiveUser,
                "Current user is not active."
            );

        await next();
    }
}
