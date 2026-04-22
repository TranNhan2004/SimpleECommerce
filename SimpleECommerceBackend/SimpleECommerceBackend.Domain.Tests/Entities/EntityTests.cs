using FluentAssertions;
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Entities.Abstracts;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Domain.Tests.Entities;

public class EntityTests
{
    [Fact]
    public void SetId_ShouldAssignId_WhenIdIsValid()
    {
        var entity = new TestEntity();
        var id = Guid.NewGuid();

        entity.AssignId(id);

        entity.Id.Should().Be(id);
    }

    [Fact]
    public void SetId_ShouldThrowValidationException_WhenIdIsEmpty()
    {
        var entity = new TestEntity();
        var action = () => entity.AssignId(Guid.Empty);

        action.Should().Throw<ValidationException>()
            .Which.ErrorCode.Should().Be(EntityErrorCodes.EmptyId);
    }

    private sealed class TestEntity : Entity
    {
        public void AssignId(Guid id)
        {
            SetId(id);
        }
    }
}
