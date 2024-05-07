using System.Collections.Generic;

namespace Solace.addons.solace_core_plugin.lib.tile;

public static class TileCombinations
{
    public static EightWayTile[] GetUniqueEightWayTiles()
    {
        var allTiles = new List<EightWayTile>();

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

                var newCornerVariation = new EightWayTile()
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