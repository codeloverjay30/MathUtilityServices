
using FluentAssertions;
using Xunit;
using CoordinateUtilityServices;

namespace CoordinateUtilityServices.Tests;

public sealed class RectangleTests
{
    [Fact]
    public void Constructor_ValidCorners_ShouldPreserveCorners()
    {
        // Arrange
        Point topLeft = new(100, 200);
        Point bottomRight = new(300, 250);

        // Act
        Rectangle rectangle = new(topLeft, bottomRight);

        // Assert
        rectangle.TopLeft.Should().Be(topLeft);
        rectangle.BottomRight.Should().Be(bottomRight);
    }

    [Fact]
    public void Constructor_ValidCorners_ShouldCalculateDimensions()
    {
        // Arrange
        Point topLeft = new(100, 200);
        Point bottomRight = new(300, 250);

        // Act
        Rectangle rectangle = new(topLeft, bottomRight);

        // Assert
        rectangle.Width.Should().Be(200);
        rectangle.Height.Should().Be(50);
    }

    [Fact]
    public void Constructor_ValidCorners_ShouldCalculateCenter()
    {
        // Arrange
        Rectangle rectangle = new(
            new Point(100, 200),
            new Point(300, 250));

        // Act
        Point center = rectangle.Center;

        // Assert
        center.Should().Be(new Point(200, 225));
    }

    [Fact]
    public void Constructor_FractionalCorners_ShouldPreservePrecision()
    {
        // Arrange
        Rectangle rectangle = new(
            new Point(10.25, 20.5),
            new Point(30.75, 40.25));

        // Act
        double width = rectangle.Width;
        double height = rectangle.Height;
        Point center = rectangle.Center;

        // Assert
        width.Should().BeApproximately(20.5, 0.0000001);
        height.Should().BeApproximately(19.75, 0.0000001);
        center.X.Should().BeApproximately(20.5, 0.0000001);
        center.Y.Should().BeApproximately(30.375, 0.0000001);
    }

    [Theory]
    [InlineData(100, 200, 100, 250)]
    [InlineData(100, 200, 300, 200)]
    [InlineData(100, 200, 100, 200)]
    public void Constructor_ZeroWidthOrHeight_ShouldAllowDegenerateRectangle(
        double left,
        double top,
        double right,
        double bottom)
    {
        // Act
        Rectangle rectangle = new(
            new Point(left, top),
            new Point(right, bottom));

        // Assert
        rectangle.Width.Should().BeGreaterThanOrEqualTo(0);
        rectangle.Height.Should().BeGreaterThanOrEqualTo(0);
        rectangle.TopLeft.Should().Be(new Point(left, top));
        rectangle.BottomRight.Should().Be(new Point(right, bottom));
    }

    [Theory]
    [InlineData(300, 200, 100, 250)]
    [InlineData(100, 300, 300, 250)]
    [InlineData(300, 300, 100, 250)]
    public void Constructor_ReversedCorners_ShouldThrowArgumentException(
        double left,
        double top,
        double right,
        double bottom)
    {
        // Arrange
        Action act = () => _ = new Rectangle(
            new Point(left, top),
            new Point(right, bottom));

        // Act & Assert
        act.Should()
            .Throw<ArgumentException>()
            .WithMessage(
                "*Bottom-right coordinates must not precede top-left coordinates.*");
    }

    [Theory]
    [InlineData(double.NaN, 0, 100, 100)]
    [InlineData(0, double.NaN, 100, 100)]
    [InlineData(0, 0, double.NaN, 100)]
    [InlineData(0, 0, 100, double.NaN)]
    [InlineData(double.PositiveInfinity, 0, 100, 100)]
    [InlineData(0, double.NegativeInfinity, 100, 100)]
    [InlineData(0, 0, double.PositiveInfinity, 100)]
    [InlineData(0, 0, 100, double.NegativeInfinity)]
    public void Constructor_NonFiniteCoordinate_ShouldThrowArgumentException(
        double left,
        double top,
        double right,
        double bottom)
    {
        // Arrange
        Action act = () => _ = new Rectangle(
            new Point(left, top),
            new Point(right, bottom));

        // Act & Assert
        act.Should()
            .Throw<ArgumentException>()
            .WithMessage("*Rectangle coordinates must be finite.*");
    }

    [Fact]
    public void FromXYWH_ValidDimensions_ShouldCreateExpectedRectangle()
    {
        // Act
        Rectangle rectangle = Rectangle.FromXYWH(
            x: 100,
            y: 200,
            width: 200,
            height: 50);

        // Assert
        rectangle.TopLeft.Should().Be(new Point(100, 200));
        rectangle.BottomRight.Should().Be(new Point(300, 250));
        rectangle.Width.Should().Be(200);
        rectangle.Height.Should().Be(50);
        rectangle.Center.Should().Be(new Point(200, 225));
    }

    [Fact]
    public void FromXYWH_ZeroDimensions_ShouldCreatePointRectangle()
    {
        // Act
        Rectangle rectangle = Rectangle.FromXYWH(
            x: 100,
            y: 200,
            width: 0,
            height: 0);

        // Assert
        rectangle.TopLeft.Should().Be(new Point(100, 200));
        rectangle.BottomRight.Should().Be(new Point(100, 200));
        rectangle.Width.Should().Be(0);
        rectangle.Height.Should().Be(0);
        rectangle.Center.Should().Be(new Point(100, 200));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void FromXYWH_InvalidWidth_ShouldThrowArgumentOutOfRangeException(
        double width)
    {
        // Arrange
        Action act = () => _ = Rectangle.FromXYWH(
            x: 0,
            y: 0,
            width: width,
            height: 10);

        // Act & Assert
        act.Should()
            .Throw<ArgumentOutOfRangeException>()
            .WithMessage("*Width must be finite and non-negative.*")
            .And.ParamName.Should().Be("width");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void FromXYWH_InvalidHeight_ShouldThrowArgumentOutOfRangeException(
        double height)
    {
        // Arrange
        Action act = () => _ = Rectangle.FromXYWH(
            x: 0,
            y: 0,
            width: 10,
            height: height);

        // Act & Assert
        act.Should()
            .Throw<ArgumentOutOfRangeException>()
            .WithMessage("*Height must be finite and non-negative.*")
            .And.ParamName.Should().Be("height");
    }

    [Theory]
    [InlineData(double.NaN, 0)]
    [InlineData(double.PositiveInfinity, 0)]
    [InlineData(0, double.NegativeInfinity)]
    public void FromXYWH_NonFiniteOrigin_ShouldThrowArgumentException(
        double x,
        double y)
    {
        // Arrange
        Action act = () => _ = Rectangle.FromXYWH(
            x: x,
            y: y,
            width: 10,
            height: 10);

        // Act & Assert
        act.Should()
            .Throw<ArgumentException>()
            .WithMessage("*Rectangle coordinates must be finite.*");
    }

    [Fact]
    public void FromXYWH_CalculatedRightEdgeOverflows_ShouldThrowArgumentException()
    {
        // Arrange
        Action act = () => _ = Rectangle.FromXYWH(
            x: double.MaxValue,
            y: 0,
            width: double.MaxValue,
            height: 10);

        // Act & Assert
        act.Should()
            .Throw<ArgumentException>()
            .WithMessage("*Rectangle coordinates must be finite.*");
    }

    [Fact]
    public void FromXYWH_CalculatedBottomEdgeOverflows_ShouldThrowArgumentException()
    {
        // Arrange
        Action act = () => _ = Rectangle.FromXYWH(
            x: 0,
            y: double.MaxValue,
            width: 10,
            height: double.MaxValue);

        // Act & Assert
        act.Should()
            .Throw<ArgumentException>()
            .WithMessage("*Rectangle coordinates must be finite.*");
    }

    [Fact]
    public void ValueEquality_SameCorners_ShouldBeEqual()
    {
        // Arrange
        Rectangle first = new(
            new Point(100, 200),
            new Point(300, 250));

        Rectangle second = new(
            new Point(100, 200),
            new Point(300, 250));

        // Act & Assert
        first.Should().Be(second);
        first.Equals(second).Should().BeTrue();
        (first == second).Should().BeTrue();
        (first != second).Should().BeFalse();
        first.GetHashCode().Should().Be(second.GetHashCode());
    }

    [Fact]
    public void ValueEquality_DifferentCorners_ShouldNotBeEqual()
    {
        // Arrange
        Rectangle first = new(
            new Point(100, 200),
            new Point(300, 250));

        Rectangle second = new(
            new Point(100, 200),
            new Point(301, 250));

        // Act & Assert
        first.Should().NotBe(second);
        (first == second).Should().BeFalse();
        (first != second).Should().BeTrue();
    }

    [Fact]
    public void ValueType_Copy_ShouldNotModifyOriginalRectangle()
    {
        // Arrange
        Rectangle original = new(
            new Point(100, 200),
            new Point(300, 250));

        // Act
        Rectangle copy = original;

        // Assert
        copy.Should().Be(original);
        original.TopLeft.Should().Be(new Point(100, 200));
        original.BottomRight.Should().Be(new Point(300, 250));
    }
}