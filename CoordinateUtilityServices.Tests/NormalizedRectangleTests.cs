using CoordinateUtilityServices;
using FluentAssertions;
using Xunit;

namespace CoordinateUtilityServices.Tests;

public sealed class NormalizedRectangleTests
{
    [Fact]
    public void Constructor_ValidCorners_ShouldPreserveCoordinates()
    {
        Point topLeft = new(0.25, 0.2);
        Point bottomRight = new(0.75, 0.8);

        NormalizedRectangle result = new(topLeft, bottomRight);

        result.TopLeft.Should().Be(topLeft);
        result.BottomRight.Should().Be(bottomRight);
    }

    [Fact]
    public void Constructor_FullRange_ShouldAcceptZeroAndOne()
    {
        NormalizedRectangle result = new(new Point(0, 0), new Point(1, 1));

        result.TopLeft.Should().Be(new Point(0, 0));
        result.BottomRight.Should().Be(new Point(1, 1));
    }

    [Theory]
    [InlineData(-0.01, 0)]
    [InlineData(0, -0.01)]
    [InlineData(1.01, 0)]
    [InlineData(0, 1.01)]
    [InlineData(double.NaN, 0)]
    [InlineData(0, double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity, 0)]
    public void Constructor_InvalidTopLeft_ShouldThrowWithParameterName(double x, double y)
    {
        Action act = () => _ = new NormalizedRectangle(new Point(x, y), new Point(1, 1));

        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*Normalized coordinates must be finite values between zero and one.*")
            .And.ParamName.Should().Be("topLeft");
    }

    [Theory]
    [InlineData(-0.01, 1)]
    [InlineData(1, -0.01)]
    [InlineData(1.01, 1)]
    [InlineData(1, 1.01)]
    [InlineData(double.NaN, 1)]
    [InlineData(1, double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity, 1)]
    public void Constructor_InvalidBottomRight_ShouldThrowWithParameterName(double x, double y)
    {
        Action act = () => _ = new NormalizedRectangle(new Point(0, 0), new Point(x, y));

        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*Normalized coordinates must be finite values between zero and one.*")
            .And.ParamName.Should().Be("bottomRight");
    }

    [Theory]
    [InlineData(0.5, 0, 0.5, 1)]
    [InlineData(0, 0.5, 1, 0.5)]
    [InlineData(0.8, 0, 0.2, 1)]
    [InlineData(0, 0.8, 1, 0.2)]
    public void Constructor_EmptyOrReversedRectangle_ShouldThrow(
        double left, double top, double right, double bottom)
    {
        Action act = () => _ = new NormalizedRectangle(
            new Point(left, top), new Point(right, bottom));

        act.Should().Throw<ArgumentException>()
            .WithMessage("*The bottom-right point must be greater than the top-left point.*");
    }

    [Fact]
    public void ToRectangle_ValidDimensions_ShouldScaleBothCorners()
    {
        NormalizedRectangle region = new(new Point(0.25, 0.2), new Point(0.75, 0.8));

        Rectangle result = region.ToRectangle(1920, 1080);

        result.TopLeft.X.Should().BeApproximately(480, 1e-9);
        result.TopLeft.Y.Should().BeApproximately(216, 1e-9);
        result.BottomRight.X.Should().BeApproximately(1440, 1e-9);
        result.BottomRight.Y.Should().BeApproximately(864, 1e-9);
    }

    [Fact]
    public void ToRectangle_FractionalCoordinates_ShouldNotRoundToIntegerPixels()
    {
        NormalizedRectangle region = new(new Point(0.1, 0.1), new Point(0.9, 0.9));

        Rectangle result = region.ToRectangle(7, 9);

        result.TopLeft.X.Should().BeApproximately(0.7, 1e-12);
        result.TopLeft.Y.Should().BeApproximately(0.9, 1e-12);
        result.BottomRight.X.Should().BeApproximately(6.3, 1e-12);
        result.BottomRight.Y.Should().BeApproximately(8.1, 1e-12);
    }

    [Theory]
    [InlineData(0, 100, "width")]
    [InlineData(-1, 100, "width")]
    [InlineData(100, 0, "height")]
    [InlineData(100, -1, "height")]
    public void ToRectangle_NonPositiveDimension_ShouldThrow(
        int width, int height, string expectedParameter)
    {
        NormalizedRectangle region = new(new Point(0, 0), new Point(1, 1));

        Action act = () => _ = region.ToRectangle(width, height);

        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*must be a non-negative and non-zero value*")
            .And.ParamName.Should().Be(expectedParameter);
    }

    [Fact]
    public void ValueEquality_SameCorners_ShouldBeEqual()
    {
        NormalizedRectangle first = new(new Point(0.1, 0.2), new Point(0.9, 1));
        NormalizedRectangle second = new(new Point(0.1, 0.2), new Point(0.9, 1));

        first.Should().Be(second);
        first.GetHashCode().Should().Be(second.GetHashCode());
    }

    [Fact]
    public void DefaultValue_BypassesConstructor_ShouldBeDocumentedAsInvalid()
    {
        // A readonly record struct always permits default construction.
        NormalizedRectangle region = default;

        region.TopLeft.Should().Be(new Point(0, 0));
        region.BottomRight.Should().Be(new Point(0, 0));
    }
}
