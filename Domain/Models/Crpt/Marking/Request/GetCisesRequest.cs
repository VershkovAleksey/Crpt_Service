namespace Domain.Models.Crpt.Marking.Request;

public class GetCisesRequest
{
    /// <inheritdoc cref="CisesFilter"/>
    public required CisesFilter Filter { get; set; } = new();

    /// <inheritdoc cref="Pagination"/>
    public Pagination? Pagination { get; set; } = null;
}

/// <summary>
/// Параметры фильтрации
/// <remarks>Поиск КИ по указанным значениям параметров КИ</remarks>
/// </summary>
public class CisesFilter
{
    /// <summary>
    /// Период эмиссии
    /// <remarks>При наличии параметра и отсутствии всех его вложенных параметров поиск по нему не осуществляется</remarks>
    /// </summary>
    public EmissionDatePeriod? EmissionDatePeriod { get; set; } = null;

    /// <summary>
    /// Период нанесения
    /// <remarks>При наличии параметра и отсутствии всех его вложенных параметров поиск по нему не осуществляется</remarks>
    /// </summary>
    public ApplicationDatePeriod? ApplicationDatePeriod { get; set; } = null;

    /// <summary>
    /// Период производства
    /// <remarks>При наличии параметра и отсутствии всех его вложенных параметров поиск по нему не осуществляется</remarks>
    /// </summary>
    public ProductionDatePeriod? ProductionDatePeriod { get; set; } = null;

    /// <summary>
    /// Период ввода в оборот
    /// <remarks>При наличии параметра и отсутствии всех его вложенных параметров поиск по нему не осуществляется</remarks>
    /// </summary>
    public IntroducedDatePeriod? IntroducedDatePeriod { get; set; } = null;

    /// <summary>
    /// Список кодов товаров
    /// <remarks>Не более 1000 кодов товара в массиве</remarks>
    /// </summary>
    public string[]? Gtins { get; set; } = null;

    /// <summary>
    /// Список ИНН производителей
    /// </summary>
    public string[]? ProducerInns { get; set; } = null;

    /// <summary>
    /// Список видов упаковок
    /// <list type="table">
    ///<item><term>UNIT</term><description>Единица товара (КИ). Потребительская упаковка</description></item>
    ///<item><term>GROUP</term><description>Групповая упаковка (КИГУ). Используется только для товарных групп
    /// «Антисептики и дезинфицирующие средства», «Бакалейная продукция», «Безалкогольное пиво», «Биологически активные добавки к пище»,
    /// «Ветеринарные препараты», «Игры и игрушки для детей», «Консервированная продукция», «Корма для животных», «Медицинские изделия»,
    /// «Молочная продукция», «Морепродукты», «Оптоволокно и оптоволоконная продукция», «Парфюмерные и косметические средства и бытовая химия»,
    /// «Пиво, напитки, изготавливаемые на основе пива, слабоалкогольные напитки», «Пиротехника и огнетушащее оборудование»,
    /// «Радиоэлектронная продукция», «Растительные масла», «Соковая продукция и безалкогольные напитки», «Строительные материалы»,
    /// «Упакованная вода»</description></item>
    ///<item><term>SET</term><description>Набор (КИН). Используется только для товарных групп «Антисептики и дезинфицирующие средства»,
    /// «Безалкогольное пиво», «Биологически активные добавки к пище», «Духи и туалетная вода», «Игры и игрушки для детей»,
    /// «Консервированная продукция», «Корма для животных», «Медицинские изделия», «Молочная продукция», «Морепродукты»,
    /// «Моторные масла», «Парфюмерные и косметические средства и бытовая химия», «Пиротехника и огнетушащее оборудование»,
    /// «Предметы одежды, бельё постельное, столовое, туалетное и кухонное», «Растительные масла», «Соковая продукция и безалкогольные напитки»,
    /// «Фотокамеры (кроме кинокамер), фотовспышки и лампы-вспышки»</description></item>
    ///<item><term>BUNDLE</term><description>Комплект (КИК). Используется только для товарной группы «Предметы одежды, бельё постельное, столовое, туалетное и кухонное»</description></item>
    ///<item><term>BOX</term><description>Транспортная упаковка (КИТУ)</description></item>
    ///<item><term>ATK</term><description>Агрегированный таможенный код (АТК).</description></item>
    /// </list>
    /// </summary>
    public string[]? GeneralPackageTypes { get; set; } = null;

    /// <summary>
    /// Список типов отгрузки
    /// <remarks>Возможные значения:
    /// «SELLING» — «Продажа / передача товара»;
    /// «CONTRACT» — «Передача по АКС»</remarks>
    /// </summary>
    public string[]? TurnoverTypes { get; set; } = null;

    /// <summary>
    /// Список статусов КИ
    /// </summary>
    public States[]? States { get; set; } = null;

    /// <summary>
    /// 4-значный код ТН ВЭД
    /// </summary>
    public string? TnVed { get; set; } = null;

    /// <summary>
    /// 10-значный код ТН ВЭД
    /// </summary>
    public string? TnVed10 { get; set; } = null;

    /// <summary>
    /// Список типов эмиссии
    /// <list type="table">
    ///<item><term>LOCAL</term><description>Производство РФ</description></item>
    ///<item><term>FOREIGN</term><description>Ввезён в РФ</description></item>
    ///<item><term>REMAINS</term><description>Маркировка остатков</description></item>
    ///<item><term>CROSSBORDER</term><description>Ввезён из стран ЕАЭС</description></item>
    ///<item><term>REMARK</term><description>Перемаркировка</description></item>
    ///<item><term>COMMISSION</term><description>Принят на комиссию от физического лица</description></item>
    ///<item><term>REAPPLY</term><description>Маркировка вне производства или импорта</description></item>
    /// </list>
    /// </summary>
    public string[]? EmissionTypes { get; set; } = null;
    
    /// <summary>
    /// Состояние КИ в агрегате
    /// <remarks>Возможные значения:
    /// «true» — КИ упакован (поиск КИ, вложенных в вышестоящую упаковку);
    /// «false» — КИ не упакован (поиск КИ, не вложенных в другие упаковки).
    /// При отсутствии данного параметра выбираются все КИ</remarks>
    /// </summary>
    public bool IsAggregated { get; set; }

    /// <summary>
    /// Список товарных групп
    /// </summary>
    public string[] ProductGroups { get; set; } = ["lp"];

    /// <summary>
    /// Список причин выбытия
    /// <remarks>https://clothes.crpt.ru/help/list#_справочник_причины_выбытия</remarks>
    /// </summary>
    public string[]? EliminationReasons { get; set; } = null;

    /// <summary>
    /// Идентификатор ВСД
    /// </summary>
    public string? PrVetDoc { get; set; } = null;

    /// <summary>
    /// Признак обязательного наличия вложенных КИ
    /// <remarks>Возможные значения:
    /// «true» — поиск КИ с вложенными КИ;
    /// «false» — поиск КИ без вложенных КИ.
    /// При отсутствии данного параметра наличие вложенных КИ не проверяется</remarks>
    /// </summary>
    public bool HaveChildren { get; set; } = false;

    /// <summary>
    /// Признак обязательного наличия регистрационных данных контрагента
    /// <remarks>Возможные значения:«true» — только КИ с заполненными регистрационными данными контрагента «false» — только КИ с незаполненными регистрационными данными контрагента.При отсутствии данного параметра заполнение регистрационных данных контрагента не проверяется</remarks>
    /// </summary>
    public bool ServiceProviderIdPresented { get; set; } = false;

    /// <summary>
    /// Список типов сервис-провайдеров
    /// <remarks>Возможные значения: «CEM» — «Типография»; «CL» — «Логистический склад»; «CM» — «Контрактное производство»; «CA» — «Комиссионная площадка»</remarks>
    /// </summary>
    public string[]? ServiceProviderTypes { get; set; } = null;
    
    /// <summary>
    /// Список идентификаторов заказа на эмиссию КИ в СУЗ
    /// </summary>
    public string[]? OrderIds { get; set; }
}

/// <summary>
/// Период эмиссии
/// <remarks>При наличии параметра и отсутствии всех его вложенных параметров поиск по нему не осуществляется</remarks>
/// </summary>
public class EmissionDatePeriod
{
    /// <summary>
    /// Дата эмиссии, от
    /// <remarks>Дата в формате yyyy-MM-ddTHH:mm:ss.SSSZ для точной фильтрации по товарным группам с массовым производством</remarks>
    /// </summary>
    public string? From { get; set; } = null;

    /// <summary>
    /// Дата эмиссии, до
    /// <remarks>Дата в формате yyyy-MM-ddTHH:mm:ss.SSSZ для точной фильтрации по товарным группам с массовым производством. Значение данного параметра должно быть не меньше, чем значение параметра «from» («Дата эмиссии, до»)</remarks>
    /// </summary>
    public string? To { get; set; } = null;
}

/// <summary>
/// Период нанесения
/// <remarks>При наличии параметра и отсутствии всех его вложенных параметров поиск по нему не осуществляется</remarks>
/// </summary>
public class ApplicationDatePeriod
{
    /// <summary>
    /// Дата нанесения, от
    /// <remarks>Дата в формате yyyy-MM-ddTHH:mm:ss.SSSZ для точной фильтрации по товарным группам с массовым производством</remarks>
    /// </summary>
    public string? From { get; set; } = null;
    /// <summary>
    /// Дата нанесения, до
    /// <remarks>Дата в формате yyyy-MM-ddTHH:mm:ss.SSSZ для точной фильтрации по товарным группам с массовым производством. Значение данного параметра должно быть не меньше, чем значение параметра «from» («Дата нанесения, до»)</remarks>
    /// </summary>
    public string? To { get; set; } = null;
}

public class ProductionDatePeriod
{
    public string? From { get; set; } = null;
    public string? To { get; set; } = null;
}

public class IntroducedDatePeriod
{
    public string?From { get; set; } = null;
    public string? To { get; set; } = null;
}

public class States
{
    public string? Status { get; set; } = null;
    public string? StatusExt { get; set; } = null;
    public bool IsStatusExtNull { get; set; } = false;
}

public class Pagination
{
    public int? PerPage { get; set; } = 1000;
    public required string LastEmissionDate { get; set; }
    public required string Sgtin { get; set; }
    public int? Direction { get; set; } = null;
}