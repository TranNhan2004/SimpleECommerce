using FluentAssertions;
using SimpleECommerceBackend.Application.Enums;
using SimpleECommerceBackend.Application.Utils;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Application.Tests.Utils;

public class FilterGroupConverterTests
{
    [Fact]
    public void Build_ShouldReturnNull_WhenCriterionCountIsZero()
    {
        var result = FilterGroupConverter.Build(null, 0);

        result.Should().BeNull();
    }

    [Fact]
    public void Build_ShouldCreateDefaultAndGroup_WhenPatternIsNull()
    {
        var result = FilterGroupConverter.Build(null, 3);

        result.Should().NotBeNull();
        result.Logic.Should().Be(FilterGroupLogic.And);
        result.Children.Should().HaveCount(3);
        result.Children.Select(child => child.CriterionIndex).Should().Equal(0, 1, 2);
    }

    [Fact]
    public void Build_ShouldAppendMissingCriteriaWithAnd_WhenPatternDoesNotCoverAllIndexes()
    {
        var result = FilterGroupConverter.Build("({0} AND {2}) OR {3}", 5);

        result.Should().NotBeNull();
        result.Logic.Should().Be(FilterGroupLogic.And);
        result.Children.Should().HaveCount(3);
        result.Children[0].Group.Should().NotBeNull();
        result.Children[1].CriterionIndex.Should().Be(1);
        result.Children[2].CriterionIndex.Should().Be(4);

        var parsedGroup = result.Children[0].Group!;
        parsedGroup.Logic.Should().Be(FilterGroupLogic.Or);
    }

    [Fact]
    public void Build_ShouldParseNestedNotPattern()
    {
        var result = FilterGroupConverter.Build("(NOT ({1} AND {2})) OR {3}", 4);

        result.Should().NotBeNull();
        result.Logic.Should().Be(FilterGroupLogic.And);
        result.Children.Should().HaveCount(2);
        result.Children[1].CriterionIndex.Should().Be(0);

        var parsedGroup = result.Children[0].Group!;
        parsedGroup.Logic.Should().Be(FilterGroupLogic.Or);
        parsedGroup.Children[0].Group!.Logic.Should().Be(FilterGroupLogic.Not);
    }

    [Fact]
    public void Build_ShouldThrowValidationException_WhenNotIsNotWrappedInParentheses()
    {
        var action = () => FilterGroupConverter.Build("NOT {1} AND {2}", 3);

        action.Should().Throw<ValidationException>();
    }
}
