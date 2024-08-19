using Labirint.Core.Interfaces;

namespace Labirint.Core.Items;

public record WorldItemParameters(IRandom Random, int Width, int Height, int Density);
