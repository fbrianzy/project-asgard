# Quick Start

```bash
dotnet new worker -n ImageResizer.Worker -o image-resizer-worker/src/ImageResizer.Worker
cd image-resizer-worker/src/ImageResizer.Worker
dotnet add package SixLabors.ImageSharp --version 3.1.5
dotnet run
```