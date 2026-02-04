using Microsoft.EntityFrameworkCore;

namespace SimpleECommerceBackend.Infrastructure.Persistence.DbContext;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
}