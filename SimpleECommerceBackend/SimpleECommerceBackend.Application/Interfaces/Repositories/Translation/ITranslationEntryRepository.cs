using SimpleECommerceBackend.Domain.Entities.Translation;

namespace SimpleECommerceBackend.Application.Interfaces.Repositories.Translation;

public interface ITranslationEntryRepository : IGenericRepository<TranslationEntry>
{
    Task<TranslationEntry?> FindAsync(
        string entityName,
        string fieldName,
        Guid rowId,
        string locale,
        bool trackChanges = false
    );
}