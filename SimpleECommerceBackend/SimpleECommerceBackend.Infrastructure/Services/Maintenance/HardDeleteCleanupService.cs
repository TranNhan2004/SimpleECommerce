using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Infrastructure.Options.Maintenance;
using SimpleECommerceBackend.Infrastructure.Persistence;
using Serilog;
using SimpleECommerceBackend.Domain.Entities;


namespace SimpleECommerceBackend.Infrastructure.Services.Maintenance;

public sealed class HardDeleteCleanupService
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger _logger;
    private readonly HardDeleteOptions _options;
    private readonly TimeProvider _timeProvider;

    public HardDeleteCleanupService(
        AppDbContext dbContext,
        IOptions<HardDeleteOptions> options,
        TimeProvider timeProvider,
        ILogger logger
    )
    {
        _dbContext = dbContext;
        _timeProvider = timeProvider;
        _logger = logger;
        _options = options.Value;
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        if (_options.DaysUntilHardDelete < 0)
        {
            throw new InvalidOperationException(
                $"{HardDeleteOptions.SectionName}:{nameof(HardDeleteOptions.DaysUntilHardDelete)} must be greater than or equal to 0."
            );
        }

        var cutoff = _timeProvider.GetUtcNow().AddDays(-_options.DaysUntilHardDelete);
        var softDeleteEntityTypes = ResolveSoftDeleteEntityTypes();

        if (softDeleteEntityTypes.Count == 0)
        {
            _logger.Information(
                "Hard delete cleanup skipped because no mapped entities implement {InterfaceName}.",
                nameof(ISoftDeleteTrackable)
            );

            return;
        }

        foreach (var entityType in softDeleteEntityTypes)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (entityType == typeof(CustomerShippingAddress))
            {
                await HardDeleteAsync<CustomerShippingAddress>(cutoff, cancellationToken);
                continue;
            }

            if (entityType == typeof(SellerWarehouse))
            {
                await HardDeleteAsync<SellerWarehouse>(cutoff, cancellationToken);
                continue;
            }

            throw new InvalidOperationException(
                $"Hard delete cleanup does not support mapped soft-delete entity '{entityType.FullName}'."
            );
        }
    }

    private List<Type> ResolveSoftDeleteEntityTypes()
    {
        return [.._dbContext.Model
            .GetEntityTypes()
            .Select(entityType => entityType.ClrType)
            .Where(clrType => typeof(ISoftDeleteTrackable).IsAssignableFrom(clrType))
            .Distinct()
            .OrderBy(clrType => clrType.Name, StringComparer.Ordinal)];
    }

    private async Task HardDeleteAsync<T>(
        DateTimeOffset cutoff,
        CancellationToken cancellationToken
    )
    where T : class, ISoftDeleteTrackable
    {
        var deletedCount = await _dbContext.Set<T>()
            .Where(entity =>
                entity.IsDeleted &&
                entity.DeletedAt.HasValue &&
                entity.DeletedAt <= cutoff
            )
            .ExecuteDeleteAsync(cancellationToken);

        _logger.Information(
            "Hard delete cleanup removed {DeletedCount} rows from {TableName} with cutoff {Cutoff}.",
            deletedCount,
            nameof(T),
            cutoff
        );
    }
}
