using Microsoft.EntityFrameworkCore;

namespace SimpleECommerceBackend.Infrastructure.Persistence.AppDbContext;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
}