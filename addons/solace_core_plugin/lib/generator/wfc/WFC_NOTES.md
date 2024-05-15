Initialise a list of cells
    Add the possibility list to each cell.
    May 'draw' the map boundaries / map center with exact cells.
    The initialisation step marks the affected cells as dirty, and propagates.

Solver:
    Pop a cell:
        find lowest entropy:
            set minimum entropy at max value.
            Create a list to track minimum entropy cells.
            for each cell:
                get cell entropy:
                    initialise cell weight sums
                    loop over available selections to sum the weights.
                    ```
                    weightSum += selection.weight;
                    weightLogSum += selection.weight * math.log(selection.weight);
                    ```
                    Set the cell weight:
                    ```
                    cell.weight = math.log(weightSum)-(weightLogSum/weightSum);
                    ```
                Update the minimum entropy index if lower than known minimum
        Select a cell from the minimum entropy cells randomly.
            Select a random possibility from available selections.
            Mark the cell as 'dirty', due to unenforced changes.
    Propagate:
        keep a stack of 'dirty' cells, with unchecked propagation.
        Initialise the stack with the cell popped in the previous step.
        While there are dirty cells remaining:
            pop a cell from the stack,
            for each neighbouring direction:
                get the list of accepted sockets in that direction.
                Remove unfit possibilities from the neighbour.
                If any possibilities were removed, mark the neighbour as dirty, 
                    and add it to the stack if not already in there. 
    Check if all cells have exactly 1 option remaining
        If any cell has 0 options remaining; restart, backtrack, or stop completely.
        If all cells have exactly one option remaining, generation is complete.

Max 64 types allowed per tile; kept as a mask with uint64.
This speeds up most operations considerably,
The generated tile sets do not map to the full variety of assets,

Each given layer has 'options' defined; one of which must be chosen for the wave to resolve.
Options have sockets; which are used to compare whether the options can be paired with each other in a given direction.
Sockets are axis independent; but placement pairs are done alongside the axes
For any tile, there are 4 different sides to place another tile on;

Each 'socket', for every axis; has a whitelist for allowed tiles in the given direction
Each socket checks suitability by exact match; The sockets face in global directions, not local ones.
    - NS / SN sockets are the same; 
    - EW / WE sockets are the same
Every option, for every axis; has a socket; defined as an int.
Every layer; has a list of options; one must be chosen for each cell.
cells define a given volume in space; and may have an arbitrary amount of layers solved at the same time.

layers may have 'internal requirements'; and constrain themselves from other layers. 
Layers cannot explicitly check other layers;
    Layers define one or more 'internal' sockets,
    Other layers can check a different layer for that socket; which is not used elsewhere.

Sockets can be fit exactly, or inclusively.