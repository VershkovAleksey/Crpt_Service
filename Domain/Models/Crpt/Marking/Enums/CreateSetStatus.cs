using System.ComponentModel;

namespace Domain.Models.Crpt.Marking.Enums;

public enum CreateSetStatus
{
    [Description("Сформирован")]
    Created = 1,
    
    [Description("Ожидает формирования")]
    Proccessed = 2,
    
    [Description("Ошибка")]
    Error = 3
}