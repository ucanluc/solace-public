using System.Runtime.CompilerServices;
using Godot;

namespace Solace.addons.solace_core_plugin.lib.utilities;

public static class DimensionalUtilities
{
    /// <summary>
    /// Mathematical modulus operation; indexing into segments of given length repeating from zero.
    /// Handles negative indexes and lengths.
    /// The default % operation in C# is not mod, but remainder.
    /// </summary>
    /// <param name="index">Value that may be outside 0~length.</param>
    /// <param name="length">Non-zero length of the segments to index into.</param>
    /// <returns>Index into the segment of 0~length </returns>
    public static int ModWrap(this int index, int length)
    {
        // index % length is the remainder of the division;
        // adding the length again indexes from 'the end of the length'
        // second remainder puts us within 0~length.
        return ((index % length) + length) % length;
    }

    /// <summary>
    /// Mathematical modulus operation that handles length of 0 by returning zero.
    /// See <see cref="ModWrap"/> for the faster implementation for non-zero lengths.
    /// </summary>
    /// <param name="index">Value that may be outside 0~length.</param>
    /// <param name="length">Length of the segments to index into</param>
    /// <returns>Index into the segment of 0~length </returns>
    public static int ModWrapZero(this int index, int length)
    {
        // handle the zero case by 
        return length == 0 ? 0 : ModWrap(index, length);
    }


    /// <summary>
    /// Convert given index in a flat dimensional array to coordinates.
    /// Dimension lengths are expected to be nonzero, and positive.
    /// Index is expected to be nonzero and positive.
    /// </summary>
    /// <param name="index">index of the item in flat array</param>
    /// <param name="dimensionSizes">length of the dimensions</param>
    /// <returns>Coordinates within dimension bounds.</returns>
    public static Vector4I ToDimensionalCoordinates(this int index, Vector4I dimensionSizes)
    {
        dimensionSizes.Deconstruct(out var dX, out var dY, out var dZ, out var dW);

        // x = index % dimensions.x
        var xOffset = index % dX;
        var yOffset = (index / (dX)) % dY;
        var zOffset = (index / (dX * dY)) % dZ;
        var wOffset = (index / (dX * dY * dZ)) % dW;

        return new Vector4I(xOffset, yOffset, zOffset, wOffset);
    }

    /// <summary>
    /// Convert given index in a flat dimensional array to coordinates.
    /// Dimension lengths are expected to be nonzero, and positive.
    /// Index is expected to be nonzero and positive.
    /// </summary>
    /// <param name="index">index of the item in flat array</param>
    /// <param name="dimensionSizes">length of the dimensions</param>
    /// <returns>Coordinates within dimension bounds.</returns>
    public static Vector3I ToDimensionalCoordinates(this int index, Vector3I dimensionSizes)
    {
        dimensionSizes.Deconstruct(out var dX, out var dY, out var dZ);

        var xOffset = index % dX;
        var yOffset = (index / (dX)) % dY;
        var zOffset = (index / (dX * dY)) % dZ;

        return new Vector3I(xOffset, yOffset, zOffset);
    }

    /// <summary>
    /// Convert given index in a flat dimensional array to coordinates.
    /// Dimension lengths are expected to be nonzero, and positive.
    /// Index is expected to be nonzero and positive.
    /// </summary>
    /// <param name="index">index of the item in flat array</param>
    /// <param name="dimensionSizes">length of the dimensions</param>
    /// <returns>Coordinates within dimension bounds.</returns>
    public static Vector2I ToDimensionalCoordinates(this int index, Vector2I dimensionSizes)
    {
        dimensionSizes.Deconstruct(out var dX, out var dY);

        var xOffset = index % dX;
        var yOffset = (index / (dX)) % dY;

        return new Vector2I(xOffset, yOffset);
    }

    /// <summary>
    /// Convert given coordinates to a flat dimensional array index.
    /// Negative coordinates 'wrap around' into the given dimension sizes.
    /// Dimension sizes are expected to be non-zero and positive.
    /// </summary>
    /// <param name="coordinates">coordinates in the dimensional array</param>
    /// <param name="dimensionSizes">length of the dimensions</param>
    /// <returns>Index in the dimensional array</returns>
    public static int ToDimensionalIndex(this Vector4I coordinates, Vector4I dimensionSizes)
    {
        dimensionSizes.Deconstruct(out var dX, out var dY, out var dZ, out var dW);
        coordinates.Deconstruct(out var x, out var y, out var z, out var w);

        var xOffset = x.ModWrap(dX);
        var yOffset = y.ModWrap(dY) * dX;
        var zOffset = z.ModWrap(dZ) * dX * dY;
        var wOffset = w.ModWrap(dW) * dX * dY * dZ;

        var index = xOffset + yOffset + zOffset + wOffset;

        return index;
    }


    /// <summary>
    /// Convert given coordinates to a flat dimensional array index.
    /// Negative coordinates 'wrap around' into the given dimension sizes.
    /// Dimension sizes are expected to be non-zero and positive.
    /// </summary>
    /// <param name="coordinates">coordinates in the dimensional array</param>
    /// <param name="dimensionSizes">length of the dimensions</param>
    /// <returns>Index in the dimensional array</returns>
    public static int ToDimensionalIndex(this Vector3I coordinates, Vector3I dimensionSizes)
    {
        dimensionSizes.Deconstruct(out var dX, out var dY, out var dZ);
        coordinates.Deconstruct(out var x, out var y, out var z);

        var xOffset = x.ModWrap(dX);
        var yOffset = y.ModWrap(dY) * dX;
        var zOffset = z.ModWrap(dZ) * dX * dY;

        var index = xOffset + yOffset + zOffset;

        return index;
    }


    /// <summary>
    /// Convert given coordinates to a flat dimensional array index.
    /// Negative coordinates 'wrap around' into the given dimension sizes.
    /// Dimension sizes are expected to be non-zero and positive.
    /// </summary>
    /// <param name="coordinates">coordinates in the dimensional array</param>
    /// <param name="dimensionSizes">length of the dimensions</param>
    /// <returns>Index in the dimensional array</returns>
    public static int ToDimensionalIndex(this Vector2I coordinates, Vector2I dimensionSizes)
    {
        dimensionSizes.Deconstruct(out var dX, out var dY);
        coordinates.Deconstruct(out var x, out var y);

        var xOffset = x.ModWrap(dX);
        var yOffset = y.ModWrap(dY) * dX;

        var index = xOffset + yOffset;

        return index;
    }

    /// <summary>
    /// Convert given coordinates to coordinates within the dimension.
    /// Dimension sizes are expected to be non-zero and positive.
    /// </summary>
    /// <param name="coordinates">coordinates in the dimensional array</param>
    /// <param name="dimensionSizes">length of the dimensions</param>
    /// <returns>Coordinates within dimension bounds.</returns>
    public static Vector4I WrapInDimension(this Vector4I coordinates, Vector4I dimensionSizes)
    {
        dimensionSizes.Deconstruct(out var dX, out var dY, out var dZ, out var dW);
        coordinates.Deconstruct(out var x, out var y, out var z, out var w);

        // x = index % dimensions.x
        var xOffset = x.ModWrap(dX);
        var yOffset = y.ModWrap(dY);
        var zOffset = z.ModWrap(dZ);
        var wOffset = w.ModWrap(dW);

        return new Vector4I(xOffset, yOffset, zOffset, wOffset);
    }

    /// <summary>
    /// Convert given coordinates to coordinates within the dimension.
    /// Dimension sizes are expected to be non-zero and positive.
    /// </summary>
    /// <param name="coordinates">coordinates in the dimensional array</param>
    /// <param name="dimensionSizes">length of the dimensions</param>
    /// <returns>Coordinates within dimension bounds.</returns>
    public static Vector3I WrapInDimension(this Vector3I coordinates, Vector3I dimensionSizes)
    {
        dimensionSizes.Deconstruct(out var dX, out var dY, out var dZ);
        coordinates.Deconstruct(out var x, out var y, out var z);

        // x = index % dimensions.x
        var xOffset = x.ModWrap(dX);
        var yOffset = y.ModWrap(dY);
        var zOffset = z.ModWrap(dZ);

        return new Vector3I(xOffset, yOffset, zOffset);
    }

    /// <summary>
    /// Convert given coordinates to coordinates within the dimension.
    /// Dimension sizes are expected to be non-zero and positive.
    /// </summary>
    /// <param name="coordinates">coordinates in the dimensional array</param>
    /// <param name="dimensionSizes">length of the dimensions</param>
    /// <returns>Coordinates within dimension bounds.</returns>
    public static Vector2I WrapInDimension(this Vector2I coordinates, Vector2I dimensionSizes)
    {
        dimensionSizes.Deconstruct(out var dX, out var dY);
        coordinates.Deconstruct(out var x, out var y);

        // x = index % dimensions.x
        var xOffset = x.ModWrap(dX);
        var yOffset = y.ModWrap(dY);

        return new Vector2I(xOffset, yOffset);
    }
}