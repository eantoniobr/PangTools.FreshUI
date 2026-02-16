# Saeroun (새로운)

This repository contains libraries and tooling to render Pangya's custom
UI markup system (Fresh and Refresh) to images.

"Fresh" is an assumption of the internal name used by Ntreev Soft. There's
a lot of references to this name, and the new UI system introduced in Season 8
is called "Refresh" (very obviously in files and class names).

## Repository Overview

### [`Saeroun.Serialization`](./Saeroun.Serialization)

`Saeroun.Serialization` contains models and utilities to serialize
the XML format into usable data structures.

### [`Saeroun.Renderer`](./Saeroun.Renderer)

`Saeroun.Renderer` contains rendering methods to render the different
UI elements into an `ImageSharp` canvas. Most methods are provided as extension
methods to the regular image processing context.

### [`Saeroun.CLI`](./Saeroun.CLI)

`Saeroun.CLI` contains a CLI application that renders the XML definition
to images.

## License

This project is licensed under [aGPL v3](./LICENSE)