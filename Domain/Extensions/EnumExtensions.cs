using System.ComponentModel;
using System.Reflection;

namespace Domain.Extensions;

public static class EnumExtensions
{
    public static string GetDescription(this Enum value)
    {
        // Получаем тип перечисления
        Type type = value.GetType();
        // Получаем имя элемента перечисления
        string name = Enum.GetName(type, value);
        if (name == null)
            return null;

        // Получаем поле перечисления
        FieldInfo field = type.GetField(name);
        if (field == null)
            return null;

        // Получаем атрибут Description
        DescriptionAttribute attr = field.GetCustomAttribute<DescriptionAttribute>();
        return attr != null ? attr.Description : name;
    }
}