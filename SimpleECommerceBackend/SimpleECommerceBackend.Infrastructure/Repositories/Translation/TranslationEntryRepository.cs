using SimpleECommerceBackend.Application.Interfaces.Repositories.Translation;
using SimpleECommerceBackend.Domain.Entities.Translation;
using SimpleECommerceBackend.Infrastructure.Persistence;

namespace SimpleECommerceBackend.Infrastructure.Repositories.Translation;


public class TranslationEntryRepository : GenericRepository<TranslationEntry>, ITranslationEntryRepository
{
    public TranslationEntryRepository(Serilog.ILogger logger, AppDbContext appDbContext) : base(logger, appDbContext)
    {
    }

    public Task<TranslationEntry?> FindAsync(
        string entityName,
        string fieldName,
        Guid rowId,
        string locale,
        bool trackChanges = false
    )
    {
        return base.FindFirstByConditionAsync(
            q => q.Where(te =>
                te.EntityName == entityName &&
                te.FieldName == fieldName &&
                te.RowId == rowId &&
                te.Locale == locale
            ),
            trackChanges
        );
    }
}