using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SimpleECommerceBackend.Application.Interfaces.UseCases;
using SimpleECommerceBackend.Application.Models.Permissions;
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Api.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class AllowPermissionsAttribute : TypeFilterAttribute
{
    public AllowPermissionsAttribute(params string[] permissions) : base(typeof(AllowPermissionsFilter))
    {
        Arguments = [permissions];
    }
}

internal sealed class AllowPermissionsFilter : IAsyncActionFilter
{
    private readonly IUseCaseDispatcher _dispatcher;
    private readonly IReadOnlyList<string> _permissions;

    public AllowPermissionsFilter(
        IUseCaseDispatcher dispatcher,
        string[] permissions
    )
    {
        _dispatcher = dispatcher;
        _permissions = [..permissions
            .Where(permission => !string.IsNullOrWhiteSpace(permission))
            .Select(permission => permission.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)];
    }

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next
    )
    {
        if (_permissions.Count == 0)
        {
            await next();
            return;
        }

        var result = await _dispatcher.SendAsync<GetMyPermissionsQuery, GetMyPermissionsResult>(
            new GetMyPermissionsQuery(),
            context.HttpContext.RequestAborted
        );

        var permissionSet = result.Permissions.ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (_permissions.Any(permissionSet.Contains))
        {
            await next();
            return;
        }

        throw new ForbiddenException(
            AuthorizationErrorCodes.Forbidden,
            "Current user does not have the required permissions.",
            new Dictionary<string, object?>
            {
                ["permissions"] = _permissions
            }
        );
    }
}
