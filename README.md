# Solace
## About the Project(s)
This project-of-projects contains tooling, references, assets and prototypes created while developing the 'Solace' framework.

### Puzzles solved so far:
- An SDF approximator for dynamic 3D navigation [generalised from here](https://youtu.be/_nqjsfMtO6c);
  - Tracks known surface/air volumes given a moving point in space;
  - For getting ground height, normal, and movement to maximise/minimise sky coverage.

### Active puzzles:
- Syncing between concurrent 3D, 2D, and abstracted worlds
  - for large scale, low precision simulations in the background.
- Hierarchical level generation specialised from (WFC)[https://github.com/mxgmn/WaveFunctionCollapse]
  - for level generation during runtime with pre-generated assets, at different scales;
- Tracking approximate 3D and 2D Visibility;
  - for allowing access to only 'what the character knows' for the player.
- An isolated 'simulation' runtime for the framework.
- and many others.

### About the Framework (Core Addon)
Solace is (intended to someday become) a C# based simulation framework in Godot; designed to ease the development of games with 'multiple levels of precision' in game mechanics.
Framework features are being collected in [`addons/solace-core-plugin`](https://github.com/ucanluc/solace-public/tree/a88e6104568c389d9f8678e173bd7b41f11eb8ac/addons/solace_core_plugin) during the experimental stage.

### Third Party Addons in Use
- [Godot Debug Draw](https://github.com/Zylann/godot_debug_draw)
- [GdUnit4](https://github.com/MikeSchulze/gdUnit4)
- [Game assets by Kenney](https://www.kenney.nl/)
