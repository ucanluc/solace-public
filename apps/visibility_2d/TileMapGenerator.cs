using System;
using System.Collections.Generic;
using Godot;
using Solace.addons.solace_core_plugin.core;

namespace Solace.apps.visibility_2d;

[Tool]
public partial class TileMapGenerator : TileMap
{
    private const int TileTextureEdgeLengthInPixels = 64;
    private const int MarginInPixels = 1;
    private const int SeparationInPixels = 1;
    private const int TileSubdivision = 8;
    private const int TileSubblockEdgeLengthInPixels = TileTextureEdgeLengthInPixels / TileSubdivision;

    [Export] private ImageTexture? _customAtlas;
    [Export] private bool _recreateTexture = false;

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (_recreateTexture)
        {
            _recreateTexture = false;
            RecreateTexture();
        }
    }

    /// <summary>
    /// Recreate a texture to use as a tileset.
    /// </summary>
    private void RecreateTexture()
    {
        if (!Engine.IsEditorHint())
        {
            SC.PrintErr(nameof(TileMapGenerator), "Texture creation is editor only; aborting.");
        }


        SC.Print(nameof(TileMapGenerator), "Recreating the texture...");

        var tiles = GetAtlasTileTypes();
        var sqrtTileCount = Mathf.CeilToInt(Mathf.Sqrt(tiles.Length));

        var image = CreateEmptyAtlasImage(sqrtTileCount);
        DrawAtlasTiles(tiles, sqrtTileCount, image);

        _customAtlas = ImageTexture.CreateFromImage(image);
    }

    private static Image CreateEmptyAtlasImage(int sqrtTileCount)
    {
        var squareAtlasSideLengthInPixels = (TileTextureEdgeLengthInPixels * sqrtTileCount) +
                                            (sqrtTileCount * SeparationInPixels) +
                                            MarginInPixels;

        var image = Image.Create(squareAtlasSideLengthInPixels,
            squareAtlasSideLengthInPixels, false, Image.Format.Rgba8);
        image.Fill(Colors.Magenta);
        return image;
    }

    private static void DrawAtlasTiles(AtlasTile[] tiles, int sqrtTileCount, Image image)
    {
        var tileSizeInPixels = new Vector2I(TileTextureEdgeLengthInPixels, TileTextureEdgeLengthInPixels);
        const int secondaryCornerStartOffset = TileTextureEdgeLengthInPixels - TileSubblockEdgeLengthInPixels;
        for (var i = 0; i < tiles.Length; i++)
        {
            var tile = tiles[i];
            var column = i % sqrtTileCount;
            // ReSharper disable once PossibleLossOfFraction
            var row = Mathf.FloorToInt((i - column) / sqrtTileCount);

            var tileTextureStartCoordX =
                (column * TileTextureEdgeLengthInPixels) + MarginInPixels + (SeparationInPixels * column);
            var tileTextureStartCoordY =
                (row * TileTextureEdgeLengthInPixels) + MarginInPixels + (SeparationInPixels * row);

            var tileTopLeftPixelCoord = new Vector2I(tileTextureStartCoordX, tileTextureStartCoordY);

            var tileRectOnAtlas = new Rect2I(
                tileTopLeftPixelCoord, tileSizeInPixels
            );
            image.FillRect(tileRectOnAtlas, Colors.Transparent);

            var cornerOutlineColor = Colors.Black;
            var wallFillColor = Colors.Plum;


            if (!tile.clearCornerNE)
            {
                image.FillRect(new Rect2I(
                    tileTopLeftPixelCoord,
                    new Vector2I(TileSubblockEdgeLengthInPixels, TileSubblockEdgeLengthInPixels)), cornerOutlineColor);
                if (!tile.wallE)
                {
                    image.FillRect(new Rect2I(
                            tileTopLeftPixelCoord,
                            new Vector2I(TileSubblockEdgeLengthInPixels, TileSubblockEdgeLengthInPixels)),
                        cornerOutlineColor);
                }
            }

            if (!tile.clearCornerNW)
            {
                image.FillRect(new Rect2I(
                        new Vector2I(
                            tileTextureStartCoordX + secondaryCornerStartOffset,
                            tileTextureStartCoordY
                        ), new Vector2I(TileSubblockEdgeLengthInPixels, TileSubblockEdgeLengthInPixels)),
                    cornerOutlineColor);
            }

            if (!tile.clearCornerSE)
            {
                image.FillRect(new Rect2I(
                        new Vector2I(
                            tileTextureStartCoordX,
                            tileTextureStartCoordY + secondaryCornerStartOffset
                        ), new Vector2I(TileSubblockEdgeLengthInPixels, TileSubblockEdgeLengthInPixels)),
                    cornerOutlineColor);
            }

            if (!tile.clearCornerSW)
            {
                image.FillRect(new Rect2I(
                        new Vector2I(
                            tileTextureStartCoordX + secondaryCornerStartOffset,
                            tileTextureStartCoordY + secondaryCornerStartOffset
                        ), new Vector2I(TileSubblockEdgeLengthInPixels, TileSubblockEdgeLengthInPixels)),
                    cornerOutlineColor);
            }
        }
    }

    private AtlasTile[] GetAtlasTileTypes()
    {
        var cornerTiles = new AtlasTile[16];
        var wallTiles = new AtlasTile[16];
        // create corner & wall spots
        for (var i = 1; i < 16; i++)
        {
            cornerTiles[i].clearCornerNW = (i & 1) == 1;
            cornerTiles[i].clearCornerSW = (i >> 1 & 1) == 1;
            cornerTiles[i].clearCornerNE = (i >> 2 & 1) == 1;
            cornerTiles[i].clearCornerSE = (i >> 3 & 1) == 1;

            wallTiles[i].wallN = (i & 1) == 1;
            wallTiles[i].wallE = (i >> 1 & 1) == 1;
            wallTiles[i].wallS = (i >> 2 & 1) == 1;
            wallTiles[i].wallW = (i >> 3 & 1) == 1;
        }


        var allTiles = new List<AtlasTile>();
        allTiles.AddRange(cornerTiles);
        allTiles.AddRange(wallTiles);

        return allTiles.ToArray();
    }
}