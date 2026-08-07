using System.ComponentModel;

namespace Domain.Models.Fbs;

/// <summary>
/// Enum: 1, 2, 3. Тип товара: 1 - обычный, 2 - СГТ (Сверхгабаритный товар), 3 - КГТ (Крупногабаритный товар). Не используется на данный момент.
/// </summary>
public enum CargoType
{
    [Description("Обычный")]
    Usual = 1,
    [Description("СГТ (Сверхгабаритный товар)")]
    SGT = 2,
    [Description("КГТ (Крупногабаритный товар). Не используется на данный момент.")]
    KGT = 3,
}
