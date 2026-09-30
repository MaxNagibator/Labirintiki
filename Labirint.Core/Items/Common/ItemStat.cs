namespace Labirint.Core.Items.Common;

/// <summary>
/// Характеристика предмета для витрины.
/// </summary>
/// <param name="Label">Название характеристики.</param>
/// <param name="Value">Значение характеристики.</param>
/// <param name="Suffix">Приписка после значения.</param>
public record ItemStat(string Label, string Value, string? Suffix = null);
