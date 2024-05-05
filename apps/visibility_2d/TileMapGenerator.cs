using System;
using System.Collections.Generic;
using Godot;
using Solace.addons.solace_core_plugin.core;

namespace Solace.apps.visibility_2d;

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
            
        }
    }

    private AtlasTile[] GetAtlasTileTypes()
    {
        var cornerTiles = new AtlasTile[16];
        var wallTiles = new AtlasTile[16];
        // create corner & wall spots
        for (var i = 1; i < 16; i++)
        {
            cornerTiles[i].openCornerNW = (i & 1) == 1;
            cornerTiles[i].openCornerSW = (i >> 1 & 1) == 1;
            cornerTiles[i].openCornerNE = (i >> 2 & 1) == 1;
            cornerTiles[i].openCornerSE = (i >> 3 & 1) == 1;

            wallTiles[i].openWallN = (i & 1) == 1;
            wallTiles[i].openWallE = (i >> 1 & 1) == 1;
            wallTiles[i].openWallS = (i >> 2 & 1) == 1;
            wallTiles[i].openWallW = (i >> 3 & 1) == 1;
        }


        var allTiles = new List<AtlasTile>();
        allTiles.AddRange(cornerTiles);
        allTiles.AddRange(wallTiles);

        return allTiles.ToArray();
    }
}