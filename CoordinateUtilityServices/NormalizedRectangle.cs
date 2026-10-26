using System;

namespace CoordinateUtilityServices;

/// <summary>
/// Represents a rectangle using normalized coordinates between zero and one.
/// </summary>
public readonly record struct NormalizedRectangle
{
    /// <summary>
    /// Gets the normalized top-left point.
    /// </summary>
    public Point TopLeft { get; }

    /// <summary>
    /// Gets the normalized bottom-right point.
    /// </summary>
    public Point BottomRight { get; }

    /// <summary>
    /// Initializes a normalized rectangle from two corner points.
    /// </summary>
    /// <param name="topLeft">The normalized top-left point.</param>
    /// <param name="bottomRight">The normalized bottom-right point.</param>
    public NormalizedRectangle(Point topLeft, Point bottomRight)
    {
        ValidatePoint(topLeft, nameof(topLeft));
        ValidatePoint(bottomRight, nameof(bottomRight));

        if (topLeft.X >= bottomRight.X ||
            topLeft.Y >= bottomRight.Y)
        {
            throw new ArgumentException(
                "The bottom-right point must be greater than the top-left point.");
        }

        TopLeft = topLeft;
        BottomRight = bottomRight;
    }

    /// <summary>
    /// Converts the normalized rectangle into absolute coordinates.
    /// </summary>
    /// <param name="width">The target coordinate space width.</param>
    /// <param name="height">The target coordinate space height.</param>
    /// <returns>The rectangle in absolute coordinates.</returns>
    public Rectangle ToRectangle(int width, int height)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(width);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(height);

        return new Rectangle(
            new Point(TopLeft.X * width, TopLeft.Y * height),
            new Point(BottomRight.X * width, BottomRight.Y * height));
    }

    /// <summary>
    /// Validates that a point contains finite normalized coordinates.
    /// </summary>
    /// <param name="point">The point to validate.</param>
    /// <param name="parameterName">The parameter name for exceptions.</param>
    private static void ValidatePoint(Point point, string parameterName)
    {
        if (!double.IsFinite(point.X) ||
            !double.IsFinite(point.Y) ||
            point.X < 0 ||
            point.X > 1 ||
            point.Y < 0 ||
            point.Y > 1)
        {
            throw new ArgumentOutOfRangeException(
                parameterName,
                "Normalized coordinates must be finite values between zero and one.");
        }
    }
}