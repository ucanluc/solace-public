# Solace ![](https://github.com/ucanluc/solace-private/actions/workflows/main.yml/badge.svg)
This project-of-projects contains tooling, references, assets and prototypes created while developing the solace framework.

### About the Project(s)
Builds released on this repo are intended to demonstrate framework features. 

Framework features are being collected in [`addons/solace_core_plugin`](addons/solace_core_plugin) during the experimental stage.

This repository can be used as a project template; example apps are in [`apps`](apps), and ready-to-use assets can be found in [`content`](content). 

### About the framework
"Solace" is (intended to someday become) a C# based simulation framework in Godot; made to create 'levels of detail' for gameplay mechanics in the background.

Features are designed to support a hobby project codenamed 'Solace Comes', which benefits from better solutions to the puzzles below.

#### Solutions found so far:
- An SDF approximator for 3D navigation in dynamic scenes, [generalised from the video demonstration here](https://youtu.be/_nqjsfMtO6c)
  - Tracks known surface/air volumes given a moving point in space;
  - Derives ground height, normal, and movement needed to maximise/minimise sky coverage.

#### Active puzzles:
- Level generation specialised from [WFC by Maxim Gumin](https://github.com/mxgmn/WaveFunctionCollapse)
  - to generate urban/interior environments with bounded volumes.
- Tracking approximate 3D and 2D Visibility
  - to limit the data on the player client to 'what the character knows'.
- Concurrent/synchronized 3D, 2D, and abstract worlds
  - to transfer entities between levels of precision.
- An isolated 'simulation runtime' for the framework.
  - to track entities and background events at scale.
- Utility libraries for physics, animations, placeholder asset generation etc.
  - to create diverse behaviour from limited assets using heuristics.

### Third Party / Thanks to:
- [Godot 4](https://godotengine.org): This project is made using _Godot 4 with .NET/C# support_.
- [GdUnit4](https://github.com/MikeSchulze/gdUnit4): for unit testing support.
- [Godot Debug Draw](https://github.com/Zylann/godot_debug_draw): for easy to use 3D visualisations.
- [Game assets by Kenney](https://www.kenney.nl/): for CC0 game assets.

