using CoordinateUtilityServices;
using FluentAssertions;
using Xunit;

namespace CoordinateUtilityServices.Tests;

public sealed class RectangleContainsTests
{
    [Theory]
    [InlineData(0, 0, true)]
    [InlineData(0, 50, true)]
    [InlineData(50, 0, true)]
    [InlineData(50, 50, true)]
    [InlineData(99.9, 99.9, true)]
    [InlineData(100, 50, false)]
    [InlineData(50, 100, false)]
    [InlineData(100, 100, false)]
    [InlineData(-0.01, 50, false)]
    [InlineData(50, -0.01, false)]
    [InlineData(100.01, 50, false)]
    [InlineData(50, 100.01, false)]
    public void Contains_HalfOpenBoundaries_ShouldReturnExpectedResult(
        double x, double y, bool expected)
    {
        Rectangle rectangle = Rectangle.FromXYWH(0, 0, 100, 100);

        bool result = rectangle.Contains(new Point(x, y));

        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(double.NaN, 50)]
    [InlineData(50, double.NaN)]
    [InlineData(double.PositiveInfinity, 50)]
    [InlineData(50, double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity, 50)]
    [InlineData(50, double.NegativeInfinity)]
    public void Contains_NonFinitePoint_ShouldReturnFalse(double x, double y)
    {
        Rectangle rectangle = Rectangle.FromXYWH(0, 0, 100, 100);

        rectangle.Contains(new Point(x, y)).Should().BeFalse();
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(0, 10)]
    [InlineData(10, 0)]
    public void Contains_DegenerateRectangle_ShouldReturnFalse(double width, double height)
    {
        Rectangle rectangle = Rectangle.FromXYWH(5, 5, width, height);

        rectangle.Contains(new Point(5, 5)).Should().BeFalse();
    }

    [Fact]
    public void Contains_FractionalCoordinates_ShouldPreservePrecision()
    {
        Rectangle rectangle = new(new Point(0.25, 0.5), new Point(1.75, 2.5));

        rectangle.Contains(new Point(1.749, 2.499)).Should().BeTrue();
        rectangle.Contains(new Point(1.75, 2.499)).Should().BeFalse();
    }

    [Fact]
    public void Contains_NormalizedRectangleConvertedToPixels_ShouldUseSameBoundaryRules()
    {
        NormalizedRectangle region = new(new Point(0.25, 0.25), new Point(0.75, 0.75));
        Rectangle rectangle = region.ToRectangle(200, 100);

        rectangle.Contains(new Point(50, 25)).Should().BeTrue();
        rectangle.Contains(new Point(150, 75)).Should().BeFalse();
    }
}
