using SimpleECommerceBackend.Domain.Entities;

namespace SimpleECommerceBackend.Application.Interfaces.Repositories;

public interface ITranslationRepository
{
    Task<TranslationEntry?> FindAsync(
        string entityName,
        string fieldName,
        Guid rowId,
        string locale,
        CancellationToken cancellationToken = default
    );

    Task UpsertAsync(
        TranslationEntry entry,
        CancellationToken cancellationToken = default
    );
}
