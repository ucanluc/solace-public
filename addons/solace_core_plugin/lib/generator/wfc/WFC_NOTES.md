- Solver definition:
    - One or more layers to solve over
    - Each layer can have 1~64 possible choices.
    - Choices have sockets for 3d placement, and internal qualities.
    - Sockets have 64 bit definitions
- Solver:
    - Initialise:
        - For every layer;
            - Add the wave possibilities for all possible choices in layer
        - Precull:
            - Map boundaries, center or exact coordinates may get 'preselected' to an individual choice
            - First propagation is started from the preculled cells
        - Entropy initialisation:
            - Get the entropy of all undecided cells
    - Solver Step
        - Pop a wave if no dirty waves exist:
            - Get a random wave from lowest entropy cell
            - Force the wave to a single decision; weighted random
            - Mark the wave as dirty.
        - Propagate the limited constraints:
            - Keep a stack of dirty waves
            - While there are dirty waves:
                - Pop a wave from the dirty stack
                - Get the comparison list
                    - Every neighbouring wave in layer
                    - Every aligned wave in other layers
                - For every comparison:
                    - Get the possibility whitelist for the neighbour
                    - Mask the neighbour's possibilities
                    - If changes were made to the neighbour;
                        - Abort or backtrack if resolved as a contradiction
                        - add the neighbour to the stack if not already in there
        - Solver is complete if no undecided cells exist.
    -

- Map
    - Has an arbitrary number of layers; defines internal values for a given volume of space.
    - Has an arbitrary number of cells; defines 3d space locations.
    - Has a wave for every layer/cell combination.
    - The result is interpreted from the singular choices in each layer.
- Layer
    - Each layer has 'options' defined; one of which must be chosen for the wave to resolve.
    - Max 64 options are allowed per tile layer
    - 3D boolean navigation requires exactly 64 cells to contain all possibilities.
    - Every choice has a mask of allowed choices within the layer.
- Option
    - Options have sockets for every neighbour direction in the layer,
    - Options have sockets to define 'internal' values for usage between layers.
- Sockets
    - Sockets are used to define the compatibility of two options
    - Socket definitions on the same axis faces the same way.
        - Exactly compatible NS / SN sockets, or EW/WE sockets are equal.
    - Sockets can fit 'exactly'; requiring the same socket.
    - Sockets can fit 'inclusively'; The bits of the first socket contains the second socket's bits.
- Cell
    - Defines a given location in space
    - Includes waves for each possible layer
    - A resolved cell has one choice selected for every layer.
- Wave
    - Includes 64 different possibilities
    - Exactly one possibility = wave is decided
    - Exactly zero possibility = wave is contradictory
    - Two or more possibilities = wave is undecided.
-

- Wave entropy tracker operations:
    - Pop a random minimum entropy wave;
        - Weighted random from min. entropy waves
        - The popped cell will not be added back again
    - Remove a wave from tracking
    - Add a wave to tracking
    - Add a list of waves to tracking.

Entropy Integration: with weight sum over a list of choices:

```
weightSum += selection.weight;
weightLogSum += selection.weight * math.log(selection.weight);
```

Entropy finalisation:

```
cell.weight = math.log(weightSum)-(weightLogSum/weightSum);
```

