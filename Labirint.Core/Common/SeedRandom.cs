using Labirint.Core.Interfaces;

namespace Labirint.Core.Common;

/// <summary>
/// Источник случайных чисел с фиксированным зерном.
/// </summary>
/// <param name="seed">Зерно генератора</param>
public sealed class SeedRandom(int seed) : IRandom
{
    /// <summary>
    /// Генератор случайных чисел.
    /// </summary>
    public Random Generator { get; } = new(seed);
}
