using SimpleECommerceBackend.Domain.Entities.Translation;

namespace SimpleECommerceBackend.Application.Interfaces.Repositories.Translation;

public interface ITranslationEntryRepository
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