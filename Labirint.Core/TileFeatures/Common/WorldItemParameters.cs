using Labirint.Core.Interfaces;

namespace Labirint.Core.TileFeatures.Common;

public record WorldItemParameters(IRandom Random, int Width, int Height, int Density);
