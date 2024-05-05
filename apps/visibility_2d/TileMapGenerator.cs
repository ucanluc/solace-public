using System;
using System.Collections.Generic;
using Godot;
using Solace.addons.solace_core_plugin.core;

namespace Solace.apps.visibility_2d;

/// <summary>
/// Creates a tilemap image for wall/corner variations.
/// Intended as an editor tool for a quick & dirty map generation.
/// </summary>
[Tool]
public partial class TileMapGenerator : TileMap
{
    private const int TileTextureEdgeLengthInPixels = 32;
    private const int MarginInPixels = 1;
    private const int SeparationInPixels = 1;
    private const int TileSubdivision = 8;
    private const int TileSubblockEdgeLengthInPixels = TileTextureEdgeLengthInPixels / TileSubdivision;
    private const int OutlineWidth = TileSubblockEdgeLengthInPixels / 2;
    private const int OutlineMargin = TileSubblockEdgeLengthInPixels - OutlineWidth;

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

        image.SavePng("res://test_tile_atlas.png");
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


            var outlineColor = Colors.Black;
            var wallFillColor = Color.FromHtml("57007F");
            var airFillColor = Colors.Transparent;


            image.FillRect(new Rect2I(
                tileTopLeftPixelCoord,
                tileSizeInPixels
            ), wallFillColor);

            var outlineMarginRect = new Vector2I(OutlineMargin, OutlineMargin);
            var subblockRect = new Vector2I(TileSubblockEdgeLengthInPixels, TileSubblockEdgeLengthInPixels);

            image.FillRect(new Rect2I(
                tileTopLeftPixelCoord + outlineMarginRect,
                tileSizeInPixels - (outlineMarginRect * 2)
            ), outlineColor);

            image.FillRect(new Rect2I(
                tileTopLeftPixelCoord + subblockRect,
                tileSizeInPixels - subblockRect * 2
            ), airFillColor);

            // draw tile by creating a 'blocked tile' that allows for no additional features;
            // Remove walls and corners if the tile demands it; first doing a pass with the outline color


            var horizontalWallRect = new Vector2I(
                TileTextureEdgeLengthInPixels - (TileSubblockEdgeLengthInPixels * 2),
                TileSubblockEdgeLengthInPixels
            );

            var horizontalWallOutlineRect = new Vector2I(
                TileTextureEdgeLengthInPixels - (TileSubblockEdgeLengthInPixels * 2) + (OutlineMargin * 2),
                TileSubblockEdgeLengthInPixels
            );

            var verticalWallRect = new Vector2I(
                TileSubblockEdgeLengthInPixels,
                TileTextureEdgeLengthInPixels - (TileSubblockEdgeLengthInPixels * 2)
            );

            var verticalWallOutlineRect = new Vector2I(
                TileSubblockEdgeLengthInPixels,
                TileTextureEdgeLengthInPixels - (TileSubblockEdgeLengthInPixels * 2) + (OutlineMargin * 2)
            );

            if (tile.openWallN)
            {
                image.FillRect(new Rect2I(
                    new Vector2I(
                        tileTextureStartCoordX + TileSubblockEdgeLengthInPixels - OutlineMargin,
                        tileTextureStartCoordY
                    ), horizontalWallOutlineRect), outlineColor);

                image.FillRect(new Rect2I(
                    new Vector2I(
                        tileTextureStartCoordX + TileSubblockEdgeLengthInPixels,
                        tileTextureStartCoordY
                    ), horizontalWallRect), airFillColor);
            }


            if (tile.openWallW)
            {
                image.FillRect(new Rect2I(
                    new Vector2I(
                        tileTextureStartCoordX,
                        tileTextureStartCoordY + TileSubblockEdgeLengthInPixels - OutlineMargin
                    ), verticalWallOutlineRect), outlineColor);

                image.FillRect(new Rect2I(
                    new Vector2I(
                        tileTextureStartCoordX,
                        tileTextureStartCoordY + TileSubblockEdgeLengthInPixels
                    ), verticalWallRect), airFillColor);
            }

            if (tile.openWallS)
            {
                image.FillRect(new Rect2I(
                    new Vector2I(
                        tileTextureStartCoordX + TileSubblockEdgeLengthInPixels - OutlineMargin,
                        tileTextureStartCoordY + TileTextureEdgeLengthInPixels - TileSubblockEdgeLengthInPixels
                    ), horizontalWallOutlineRect), outlineColor);

                image.FillRect(new Rect2I(
                    new Vector2I(
                        tileTextureStartCoordX + TileSubblockEdgeLengthInPixels,
                        tileTextureStartCoordY + TileTextureEdgeLengthInPixels - TileSubblockEdgeLengthInPixels
                    ), horizontalWallRect), airFillColor);
            }

            if (tile.openWallE)
            {
                image.FillRect(new Rect2I(
                    new Vector2I(
                        tileTextureStartCoordX + TileTextureEdgeLengthInPixels - TileSubblockEdgeLengthInPixels,
                        tileTextureStartCoordY + TileSubblockEdgeLengthInPixels - OutlineMargin
                    ), verticalWallOutlineRect), outlineColor);

                image.FillRect(new Rect2I(
                    new Vector2I(
                        tileTextureStartCoordX + TileTextureEdgeLengthInPixels - TileSubblockEdgeLengthInPixels,
                        tileTextureStartCoordY + TileSubblockEdgeLengthInPixels
                    ), verticalWallRect), airFillColor);
            }

            if (tile.openCornerNW)
            {
                image.FillRect(new Rect2I(
                    new Vector2I(
                        tileTextureStartCoordX,
                        tileTextureStartCoordY
                    ), subblockRect), airFillColor);
            }

            if (tile.openCornerNE)
            {
                image.FillRect(new Rect2I(
                    new Vector2I(
                        tileTextureStartCoordX + TileTextureEdgeLengthInPixels - TileSubblockEdgeLengthInPixels,
                        tileTextureStartCoordY
                    ), subblockRect), airFillColor);
            }

            if (tile.openCornerSW)
            {
                image.FillRect(new Rect2I(
                    new Vector2I(
                        tileTextureStartCoordX,
                        tileTextureStartCoordY + TileTextureEdgeLengthInPixels - TileSubblockEdgeLengthInPixels
                    ), subblockRect), airFillColor);
            }

            if (tile.openCornerSE)
            {
                image.FillRect(new Rect2I(
                    new Vector2I(
                        tileTextureStartCoordX + TileTextureEdgeLengthInPixels - TileSubblockEdgeLengthInPixels,
                        tileTextureStartCoordY + TileTextureEdgeLengthInPixels - TileSubblockEdgeLengthInPixels
                    ), subblockRect), airFillColor);
            }
        }
    }

    /// <summary>
    /// Creates the edge/corner variations for atlas tiles.
    /// </summary>
    /// <returns> an array of all unique 'atlas tiles', which lists which edges/corners of the tile are open.</returns>
    private AtlasTile[] GetAtlasTileTypes()
    {
        var allTiles = new List<AtlasTile>();

        for (var i = 0; i < 16; i++)
        {
            // get all wall variations by doing a binary count
            var openWallN = (i & 1) == 1;
            var openWallE = (i >> 1 & 1) == 1;
            var openWallS = (i >> 2 & 1) == 1;
            var openWallW = (i >> 3 & 1) == 1;

            // check combinations of corners that are 'on their own', without a wall attachment here.
            // starting from 0 also adds the 'default' corners.
            // going up to 16 also adds the 'fully empty tile' also.
            for (var j = 0; j < 16; j++)
            {
                // get which variation we are on
                var varyNE = (j & 1) == 1;
                var varyNW = (j >> 1 & 1) == 1;
                var varySE = (j >> 2 & 1) == 1;
                var varySW = (j >> 3 & 1) == 1;

                // get which corners are isolated, therefore can be varied.
                var isolatedNE = openWallN && openWallE;
                var isolatedNW = openWallN && openWallW;
                var isolatedSE = openWallS && openWallE;
                var isolatedSW = openWallS && openWallW;

                if ((varyNE && !isolatedNE)
                    || (varyNW && !isolatedNW)
                    || (varySE && !isolatedSE)
                    || (varySW && !isolatedSW)) continue;

                var newCornerVariation = new AtlasTile()
                {
                    openWallN = openWallN,
                    openWallE = openWallE,
                    openWallS = openWallS,
                    openWallW = openWallW,
                    openCornerNE = varyNE,
                    openCornerNW = varyNW,
                    openCornerSE = varySE,
                    openCornerSW = varySW
                };
                allTiles.Add(newCornerVariation);
            }
        }

        return allTiles.ToArray();
    }
}