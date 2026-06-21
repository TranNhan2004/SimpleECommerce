using Microsoft.EntityFrameworkCore;
using SimpleECommerceBackend.Application.Interfaces.Repositories.Uam;
using SimpleECommerceBackend.Application.Models.Users;
using SimpleECommerceBackend.Domain.Entities.Uam;
using SimpleECommerceBackend.Infrastructure.Persistence;

namespace SimpleECommerceBackend.Infrastructure.Repositories.Uam;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(Serilog.ILogger logger, AppDbContext appDbContext) : base(logger, appDbContext)
    {
    }

    public async Task<Guid?> FindIdByKeycloakSubjectIdAsync(Guid keycloakSubjectId)
    {
        return await DbContext.Set<User>()
            .Where(u => u.KeycloakSubjectId == keycloakSubjectId)
            .Select(u => (Guid?)u.Id)
            .FirstOrDefaultAsync();
    }

    public async Task<GetMeResult?> FindWithPermissionsByIdAsync(Guid userId, bool trackChanges = false)
    {
        IQueryable<User> users = DbContext.Set<User>();
        if (!trackChanges)
            users = users.AsNoTracking();

        var flatRows = await (
            from user in users
            where user.Id == userId
            join userRole in DbContext.Set<UserRole>() on user.Id equals userRole.UserId into userRoles
            from userRole in userRoles.DefaultIfEmpty()
            join rolePermission in DbContext.Set<RolePermission>() on userRole.RoleId equals rolePermission.RoleId into rolePermissions
            from rolePermission in rolePermissions.DefaultIfEmpty()
            join permission in DbContext.Set<Permission>() on rolePermission.PermissionId equals permission.Id into permissions
            from permission in permissions.DefaultIfEmpty()
            select new
            {
                user.Id,
                user.Email,
                user.FirstName,
                user.LastName,
                user.NickName,
                user.Sex,
                user.Status,
                user.BirthDate,
                user.AvatarUrl,
                PermissionCode = permission != null ? permission.Code : null
            }
        ).ToListAsync();

        if (flatRows.Count == 0)
            return null;

        var firstRow = flatRows[0];

        return new GetMeResult
        {
            Id = firstRow.Id,
            Email = firstRow.Email,
            FirstName = firstRow.FirstName,
            LastName = firstRow.LastName,
            NickName = firstRow.NickName,
            Sex = firstRow.Sex,
            Status = firstRow.Status,
            BirthDate = firstRow.BirthDate,
            AvatarUrl = firstRow.AvatarUrl,
            Permissions = [.. flatRows
                .Select(row => row.PermissionCode)
                .Where(code => !string.IsNullOrWhiteSpace(code))
                .Cast<string>()
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(code => code)]
        };
    }
}
