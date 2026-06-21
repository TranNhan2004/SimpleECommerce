using System.Reflection;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SimpleECommerceBackend.Application.Interfaces.Security;
using SimpleECommerceBackend.Domain.Constants.Uam;
using SimpleECommerceBackend.Domain.Entities.Abstracts;
using SimpleECommerceBackend.Domain.Entities.AuditTracking;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Infrastructure.Persistence.Interceptors;

namespace SimpleECommerceBackend.Infrastructure.Tests.Persistence.Interceptors;

public class AuditSaveChangesInterceptorTests
{
    private const string TraceId = "trace-123";
    private const string IpAddress = "203.0.113.25";

    [Fact]
    public void SavingChanges_ShouldCreateAuditForAddedEntity_AndSetCreatedAt()
    {
        using var context = CreateContext();
        var actorId = Guid.NewGuid();
        var interceptor = new AuditSaveChangesInterceptor(
            new StubCurrentRequestContext(actorId, TraceId, IpAddress)
        );
        var entity = new TestEntity
        {
            Id = Guid.NewGuid(),
            Name = "Added entity"
        };

        context.Add(entity);

        InvokeApplyAudit(interceptor, context);

        entity.CreatedAt.Should().NotBe(default);
        entity.UpdatedAt.Should().BeNull();

        var audit = GetPendingAudits(context).Should().ContainSingle().Subject;
        audit.EntityName.Should().Be(nameof(TestEntity));
        audit.EntityId.Should().Be(entity.Id);
        audit.OperationType.Should().Be(AuditOperationType.Add);
        audit.AuditedById.Should().Be(actorId);
        audit.TraceId.Should().Be(TraceId);
        audit.IpAddress.Should().Be(IpAddress);
        audit.AuditedAt.Should().Be(entity.CreatedAt);
        audit.OldValues.Should().BeNull();
        audit.NewValues.Should().Contain(nameof(TestEntity.Name));
    }

    [Fact]
    public void SavingChanges_ShouldCreateAuditForModifiedEntity_AndSetUpdatedAt()
    {
        using var context = CreateContext();
        var actorId = Guid.NewGuid();
        var interceptor = new AuditSaveChangesInterceptor(
            new StubCurrentRequestContext(actorId, TraceId, IpAddress)
        );
        var entity = new TestEntity
        {
            Id = Guid.NewGuid(),
            Name = "Updated entity",
            CreatedAt = DateTimeOffset.UtcNow.AddDays(-1)
        };

        context.Attach(entity);
        context.Entry(entity).State = EntityState.Modified;

        InvokeApplyAudit(interceptor, context);

        entity.UpdatedAt.Should().NotBeNull();

        var audit = GetPendingAudits(context).Should().ContainSingle().Subject;
        audit.EntityName.Should().Be(nameof(TestEntity));
        audit.EntityId.Should().Be(entity.Id);
        audit.OperationType.Should().Be(AuditOperationType.Update);
        audit.AuditedById.Should().Be(actorId);
        audit.TraceId.Should().Be(TraceId);
        audit.IpAddress.Should().Be(IpAddress);
        audit.AuditedAt.Should().Be(entity.UpdatedAt!.Value);
        audit.OldValues.Should().Contain(nameof(TestEntity.Name));
        audit.NewValues.Should().Contain(nameof(TestEntity.Name));
    }

    [Fact]
    public void SavingChanges_ShouldCreateAuditForDeletedEntity()
    {
        using var context = CreateContext();
        var actorId = Guid.NewGuid();
        var interceptor = new AuditSaveChangesInterceptor(
            new StubCurrentRequestContext(actorId, TraceId, IpAddress)
        );
        var entity = new TestEntity
        {
            Id = Guid.NewGuid(),
            Name = "Deleted entity",
            CreatedAt = DateTimeOffset.UtcNow.AddDays(-1)
        };

        context.Attach(entity);
        context.Remove(entity);

        InvokeApplyAudit(interceptor, context);

        var audit = GetPendingAudits(context).Should().ContainSingle().Subject;
        audit.EntityName.Should().Be(nameof(TestEntity));
        audit.EntityId.Should().Be(entity.Id);
        audit.OperationType.Should().Be(AuditOperationType.Delete);
        audit.AuditedById.Should().Be(actorId);
        audit.TraceId.Should().Be(TraceId);
        audit.IpAddress.Should().Be(IpAddress);
        audit.OldValues.Should().Contain(nameof(TestEntity.Name));
        audit.NewValues.Should().BeNull();
    }

    [Fact]
    public void SavingChanges_ShouldNotCreateAuditForAuditEntity()
    {
        using var context = CreateContext();
        var interceptor = new AuditSaveChangesInterceptor(
            new StubCurrentRequestContext(WellKnownUserIds.System, TraceId, IpAddress)
        );
        var audit = new Audit
        {
            Id = Guid.NewGuid(),
            EntityName = nameof(TestEntity),
            EntityId = Guid.NewGuid(),
            OperationType = AuditOperationType.Add,
            TraceId = TraceId,
            IpAddress = IpAddress,
            AuditedById = WellKnownUserIds.System,
            AuditedAt = DateTimeOffset.UtcNow
        };

        context.Add(audit);

        InvokeApplyAudit(interceptor, context);

        GetPendingAudits(context).Should().ContainSingle()
            .Which.Should().BeSameAs(audit);
    }

    private static TestDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlServer("Server=localhost;Database=AuditInterceptorTests;User Id=sa;Password=StrongPassword!123;TrustServerCertificate=True;")
            .Options;

        return new TestDbContext(options);
    }

    private static IReadOnlyList<Audit> GetPendingAudits(DbContext context)
    {
        return context.ChangeTracker.Entries<Audit>()
            .Select(entry => entry.Entity)
            .ToList();
    }

    private static void InvokeApplyAudit(AuditSaveChangesInterceptor interceptor, DbContext context)
    {
        var applyAuditMethod = typeof(AuditSaveChangesInterceptor).GetMethod(
            "ApplyAudit",
            BindingFlags.Instance | BindingFlags.NonPublic
        );

        applyAuditMethod.Should().NotBeNull();
        applyAuditMethod!.Invoke(interceptor, [context]);
    }

    private sealed class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TestEntity>(builder =>
            {
                builder.HasKey(entity => entity.Id);
                builder.Property(entity => entity.Name).IsRequired();
                builder.Property(entity => entity.CreatedAt).IsRequired();
                builder.Property(entity => entity.UpdatedAt);
            });

            modelBuilder.Entity<Audit>(builder =>
            {
                builder.HasKey(audit => audit.Id);
            });
        }
    }

    private sealed class TestEntity : EntityBase, ICreatedTrackable, IUpdatedTrackable
    {
        public string Name { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }

    private sealed class StubCurrentRequestContext(Guid actorId, string traceId, string ipAddress) : ICurrentRequestContext
    {
        public Guid ActorId { get; } = actorId;
        public string TraceId { get; } = traceId;
        public string IpAddress { get; } = ipAddress;
    }
}
