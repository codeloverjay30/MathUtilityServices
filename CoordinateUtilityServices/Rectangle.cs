
namespace CoordinateUtilityServices;

/// <summary>
/// Represents an axis-aligned rectangle using its top-left
/// and bottom-right coordinates.
/// </summary>
public readonly record struct Rectangle
{
    /// <summary>
    /// Gets the top-left corner of the rectangle.
    /// </summary>
    public Point TopLeft { get; }

    /// <summary>
    /// Gets the bottom-right corner of the rectangle.
    /// </summary>
    public Point BottomRight { get; }

    /// <summary>
    /// Gets the width of the rectangle.
    /// </summary>
    public double Width => BottomRight.X - TopLeft.X;

    /// <summary>
    /// Gets the height of the rectangle.
    /// </summary>
    public double Height => BottomRight.Y - TopLeft.Y;

    /// <summary>
    /// Gets the center point of the rectangle.
    /// </summary>
    public Point Center => new(
        TopLeft.X + Width / 2,
        TopLeft.Y + Height / 2);

    /// <summary>
    /// Initializes a rectangle from its top-left and
    /// bottom-right coordinates.
    /// </summary>
    public Rectangle(Point topLeft, Point bottomRight)
    {
        if (!double.IsFinite(topLeft.X)
            || !double.IsFinite(topLeft.Y)
            || !double.IsFinite(bottomRight.X)
            || !double.IsFinite(bottomRight.Y))
        {
            throw new ArgumentException(
                "Rectangle coordinates must be finite.");
        }

        if (bottomRight.X < topLeft.X
            || bottomRight.Y < topLeft.Y)
        {
            throw new ArgumentException(
                "Bottom-right coordinates must not precede "
                + "top-left coordinates.");
        }

        TopLeft = topLeft;
        BottomRight = bottomRight;
    }

    /// <summary>
    /// Creates a rectangle from an origin and its dimensions.
    /// </summary>
    public static Rectangle FromXYWH(
        double x,
        double y,
        double width,
        double height)
    {
        if (!double.IsFinite(width) || width < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(width),
                "Width must be finite and non-negative.");
        }

        if (!double.IsFinite(height) || height < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(height),
                "Height must be finite and non-negative.");
        }

        return new Rectangle(
            new Point(x, y),
            new Point(x + width, y + height));
    }
}