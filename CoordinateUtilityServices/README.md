# CoordinateUtilityServices

A lightweight .NET utility library for working with two-dimensional coordinates, screen resolution scaling, padding, and axis-aligned rectangular regions.

The library provides reusable coordinate models for applications such as UI automation, image processing, and screen-based interaction.

## Features

-   `**Point**` — Represents a two-dimensional coordinate using `double` precision. Supports scaling, arithmetic operations, and conversion to rounded integer coordinates.
-   `**Padding**` — Represents the `Left`, `Top`, `Right`, and `Bottom` margins of a region. Supports total horizontal and vertical padding calculations.
-   `**ResolutionScaler**` — Converts points between a reference resolution and an actual resolution, with optional padding for usable-screen offsets.
-   `**Rectangle**` — Represents an axis-aligned rectangular region using its top-left and bottom-right points. Provides width, height, and center-point calculations.

## Target framework

-   .NET 10 (`net10.0`)

## Usage

### Create a point

```
using CoordinateUtilityServices;

Point point = new(10.5, 20.5);

Point scaled = point.Scale(2, 2);
(int x, int y) = point.ToRoundedInt();
```

`ToRoundedInt()` uses `MidpointRounding.AwayFromZero`.

### Define screen padding

```
using CoordinateUtilityServices;

Padding padding = new(
    left: 0,
    top: 100,
    right: 0,
    bottom: 0);

int horizontal = padding.Horizontal;
int vertical = padding.Vertical;
```

`Padding` represents **margins or offsets**, not the absolute coordinates of a rectangular region.

### Scale coordinates between resolutions

```
using CoordinateUtilityServices;

IResolutionScaler scaler = new ResolutionScaler(
    baseWidth: 1920,
    baseHeight: 1080,
    currentWidth: 2340,
    currentHeight: 1080);

Point actualPoint = scaler.Transform(new Point(960, 540));
Point referencePoint = scaler.InverseTransform(actualPoint);
```

Use the optional `Padding` argument when the usable screen area excludes regions such as system bars or other reserved margins.

### Create a rectangle from two corners

```csharp
using CoordinateUtilityServices;

Rectangle rectangle = new(
    topLeft: new Point(100, 200),
    bottomRight: new Point(300, 250));

double width = rectangle.Width;   // 200
double height = rectangle.Height; // 50
Point center = rectangle.Center;  // (200, 225)
```

The rectangle is axis-aligned. Its `TopLeft` and `BottomRight` properties describe **absolute coordinates**, rather than padding values.

### Create a rectangle from an origin and dimensions

```csharp
using CoordinateUtilityServices;

Rectangle rectangle = Rectangle.FromXYWH(
    x: 100,
    y: 200,
    width: 200,
    height: 50);
```

`FromXYWH` creates the same region as the preceding two-corner example.

## Rectangle validation

`Rectangle` validates its input coordinates:

-   Coordinates must be finite; `NaN` and positive or negative infinity are rejected.
-   The bottom-right corner must not precede the top-left corner.
-   `FromXYWH` requires finite, non-negative width and height.
-   Zero width or height is permitted by the geometry model.

A zero-area rectangle may be valid as a geometric value, but consumers that require a clickable or otherwise usable region should validate that requirement separately.

## Rectangle and Padding

Although both types contain information about a region, they serve different purposes:

| Type | Meaning | Example |
| --- | --- | --- |
| `Rectangle` | Absolute bounds of a region | Text detected between `(100, 200)` and `(300, 250)` |
| `Padding` | Margins or offsets applied to a region | A `100`\\-pixel top margin |
| `Point` | A single coordinate | The center of a detected text region |

Do not use `Padding` to store the absolute boundaries of an OCR result. Use `Rectangle` for the detected region and `Point` for its center.

## Testing

The `CoordinateUtilityServices.Tests` project contains unit tests for the coordinate utilities.

`RectangleTests` covers coordinate preservation, dimensions, center-point calculations, fractional coordinates, zero-area rectangles, invalid input, `FromXYWH`, value equality, and value-type copy behavior.

Run the tests from the solution directory:

```
dotnet test .\CoordinateUtilityServices.Tests\CoordinateUtilityServices.Tests.csproj
```

# Version history
## 1.0.0
### Added
+ calculate the point for scaling.

## 2.0.0
+ Rename project name

+ Rename the namespace.

## 3.0.0
+ Add `Rectangle` for detected region of OCR.
