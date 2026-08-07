using System.ComponentModel;
using System.Reflection;

namespace Domain.Models.Fbs;

public enum SizeType
{
    #region Оверсайз

    #region Лимон
    [Description("Оверсайз лимон XXS")]
    XXS_Lemon = 309756817,
    [Description("Оверсайз лимон XS")]
    XS_Lemon = 309756818,
    [Description("Оверсайз лимон S")]
    S_Lemon = 309756819,
    [Description("Оверсайз лимон М")]
    M_Lemon = 309756820,
    [Description("Оверсайз лимон L")]
    L_Lemon = 309756821,
    [Description("Оверсайз лимон XL")]
    XL_Lemon = 309756822,
    #endregion

    #region Оливка
    [Description("Оверсайз оливка XXS")]
    XXS_Olive = 312578970,
    [Description("Оверсайз оливка XS")]
    XS_Olive = 312578971,
    [Description("Оверсайз оливка S")]
    S_Olive = 312578973,
    [Description("Оверсайз оливка М")]
    M_Olive = 312578974,
    [Description("Оверсайз оливка L")]
    L_Olive = 312578975,
    [Description("Оверсайз оливка XL")]
    XL_Olive = 312578972,
    #endregion

    #region пыльно-оливковый, светло-оливковый, оливковый, зеленая оливка, оливково-зеленый
    [Description("Оверсайз пыльно-оливковый XXS")]
    XXS_DustyOlive = 315899810,
    [Description("Оверсайз пыльно-оливковый XS")]
    XS_DustyOlive = 315899811,
    [Description("Оверсайз пыльно-оливковый S")]
    S_DustyOlive = 315899812,
    [Description("Оверсайз пыльно-оливковый М")]
    M_DustyOlive = 315899813,
    [Description("Оверсайз пыльно-оливковый L")]
    L_DustyOlive = 315899814,
    [Description("Оверсайз пыльно-оливковый XL")]
    XL_DustyOlive = 315899815,
    #endregion

    #region Белый с мерчем Телеграм
    [Description("Оверсайз белый мерч телеграм XXS")]
    XXS_WhileMerchedTelegram = 332842905,
    [Description("Оверсайз белый мерч телеграм XS")]
    XS_WhileMerchedTelegram = 332842904,
    [Description("Оверсайз белый мерч телеграм S")]
    S_WhileMerchedTelegram = 332842902,
    [Description("Оверсайз белый мерч телеграм М")]
    M_WhileMerchedTelegram = 332842901,
    [Description("Оверсайз белый мерч телеграм L")]
    L_WhileMerchedTelegram = 332842900,
    [Description("Оверсайз белый мерч телеграм XL")]
    XL_WhileMerchedTelegram = 332842903,
    #endregion

    #region Белый с мерчем Корона
    [Description("Оверсайз с мерчем Корона XXS")]
    XXS_WhiteMerchedCrown = 337879602,
    [Description("Оверсайз с мерчем Корона XS")]
    XS_WhiteMerchedCrown = 337879601,
    [Description("Оверсайз с мерчем Корона S")]
    S_WhiteMerchedCrown = 337879599,
    [Description("Оверсайз с мерчем Корона М")]
    M_WhiteMerchedCrown = 337879598,
    [Description("Оверсайз с мерчем Корона L")]
    L_WhiteMerchedCrown = 337879597,
    [Description("Оверсайз с мерчем Корона XL")]
    XL_WhiteMerchedCrown = 337879600,
    #endregion

    #region Белый с мерчем Love
    [Description("Оверсайз с мерчем Love XXS")]
    XXS_WhiteMerchedLove = 332842899,
    [Description("Оверсайз с мерчем Love XS")]
    XS_WhiteMerchedLove = 332842898,
    [Description("Оверсайз с мерчем Love S")]
    S_WhiteMerchedLove = 332842896,
    [Description("Оверсайз с мерчем Love М")]
    M_WhiteMerchedLove = 332842895,
    [Description("Оверсайз с мерчем Love L")]
    L_WhiteMerchedLove = 332842894,
    [Description("Оверсайз с мерчем Love XL")]
    XL_WhiteMerchedLove = 332842897,
    #endregion

    #region Белый с мерчем Самолет
    [Description("Оверсайз с мерчем Самолет XXS")]
    XXS_WhiteMerchedPlane = 338103203,
    [Description("Оверсайз с мерчем Самолет XS")]
    XS_WhiteMerchedPlane = 338103202,
    [Description("Оверсайз с мерчем Самолет S")]
    S_WhiteMerchedPlane = 338103200,
    [Description("Оверсайз с мерчем Самолет М")]
    M_WhiteMerchedPlane = 338103199,
    [Description("Оверсайз с мерчем Самолет L")]
    L_WhiteMerchedPlane = 338103198,
    [Description("Оверсайз с мерчем Самолет XL")]
    XL_WhiteMerchedPlane = 338103201,
    #endregion

    #region мятный, белый, зеленый
    [Description("Оверсайз зеленый в полоску XXS")]
    XXS_GreenStripped = 321028914,
    [Description("Оверсайз зеленый в полоску XS")]
    XS_GreenStripped = 321028913,
    [Description("Оверсайз зеленый в полоску S")]
    S_GreenStripped = 321028911,
    [Description("Оверсайз зеленый в полоску М")]
    M_GreenStripped = 321028910,
    [Description("Оверсайз зеленый в полоску L")]
    L_GreenStripped = 321028909,
    [Description("Оверсайз зеленый в полоску XL")]
    XL_GreenStripped = 321028912,
    #endregion

    #region Полосатик
    [Description("Оверсайз полосатик XXS")]
    XXS_Polosatik = 303395415,
    [Description("Оверсайз полосатик XS")]
    XS_Polosatik = 299928964,
    [Description("Оверсайз полосатик S")]
    S_Polosatic = 299928965,
    [Description("Оверсайз полосатик M")]
    M_Polosatik = 299928966,
    [Description("Оверсайз полосатик L")]
    L_Polosatic = 299928967,
    #endregion
    #region Мята
    [Description("Оверсайз мята XXS")]
    XXS_Mint = 312578964,
    [Description("Оверсайз мята XS")]
    XS_Mint = 312578965,
    [Description("Оверсайз мята S")]
    S_Mint = 312578967,
    [Description("Оверсайз мята М")]
    M_Mint = 312578968,
    [Description("Оверсайз мята L")]
    L_Mint = 312578969,
    [Description("Оверсайз мята XL")]
    XL_Mint = 312578966,
    #endregion

    #region Черный
    [Description("Оверсайз черный XXS")]
    XXS_Black = 297493697,
    [Description("Оверсайз черный XS")]
    XS_Black = 297493696,
    [Description("Оверсайз черный S")]
    S_Black = 297493692,
    [Description("Оверсайз черный М")]
    M_Black = 297493693,
    [Description("Оверсайз черный L")]
    L_Black = 297493694,
    [Description("Оверсайз черный XL")]
    XL_Black = 297493695,
    #endregion

    #region Белый
    [Description("Оверсайз белый XXS")]
    XXS_White = 291860118,
    [Description("Оверсайз белый XS")]
    XS_White = 291719424,
    [Description("Оверсайз белый S")]
    S_Whitte = 291719425,
    [Description("Оверсайз белый M")]
    M_White = 291719426,
    [Description("Оверсайз белый L")]
    L_White = 291719427,
    [Description("Оверсайз белый XL")]
    XL_White = 291719428,
    #endregion

    #region Полосатик синий
    [Description("Оверсайз полосатик синий XXS")]
    XXS_Polosatik_Blue = 312939561,
    [Description("Оверсайз полосатик синий XS")]
    XS_Polosatik_Blue = 312939562,
    [Description("Оверсайз полосатик синий S")]
    S_Polosatik_Blue = 312939563,
    [Description("Оверсайз полосатик синий М")]
    M_Polosatik_Blue = 312939564,
    [Description("Оверсайз полосатик синий L")]
    L_Polosatik_Blue = 312939565,
    [Description("Оверсайз полосатик синий XL")]
    XL_Polosatik_Blue = 312939566,
    #endregion

    #endregion

    #region Лонгслив

    #region Хлопковый
    [Description("Лонгслив хлопковый 38")]
    ThirtyEight_Cotton = 312716864,
    [Description("Лонгслив хлопковый 40")]
    Fourty_Cotton = 312716865,
    [Description("Лонгслив хлопковый 42")]
    FortyTwo_Cotton = 312716866,
    [Description("Лонгслив хлопковый 44")]
    FortyFour_Cotton = 312716867,
    [Description("Лонгслив хлопковый 46")]
    FortySix_Cotton = 312716869,
    [Description("Лонгслив хлопковый 48")]
    FourtyEight_Cotton = 312716871,
    [Description("Лонгслив хлопковый 50")]
    Fifty_Cotton = 312716872,
    [Description("Лонгслив хлопковый 52")]
    FiftyTwo_Cotton = 312716873,
    [Description("Лонгслив хлопковый 54")]
    FiftyFour_Cotton = 312716874,
    #endregion

    #region Пудровый
    [Description("Лонгслив пудровый 38")]
    ThirtyEight_Powdery = 285096456,
    [Description("Лонгслив пудровый 40")]
    Fourty_Powdery = 275064622,
    [Description("Лонгслив пудровый 42")]
    FortyTwo_Powdery = 275064570,
    [Description("Лонгслив пудровый 44")]
    FortyFour_Powdery = 275064571,
    [Description("Лонгслив пудровый 46")]
    FortySix_Powdery = 275064572,
    [Description("Лонгслив пудровый 48")]
    FourtyEight_Powdery = 275064573,
    [Description("Лонгслив пудровый 50")]
    Fifty_Powdery = 275064574,
    [Description("Лонгслив пудровый 52")]
    FiftyTwo_Powdery = 280072154,
    [Description("Лонгслив пудровый 54")]
    FiftyFour_Powdery = 285096457,
    #endregion

    #region Черный
    [Description("Лонгслив черный 38")]
    ThirtyEight_Black = 284303108,
    [Description("Лонгслив черный 40")]
    Fourty_Black = 275064247,
    [Description("Лонгслив черный 42")]
    FortyTwo_Black = 275064172,
    [Description("Лонгслив черный 44")]
    FortyFour_Black = 275064173,
    [Description("Лонгслив черный 46")]
    FortySix_Black = 275064174,
    [Description("Лонгслив черный 48")]
    FourtyEight_Black = 275064175,
    [Description("Лонгслив черный 50")]
    Fifty_Black = 275064176,
    [Description("Лонгслив черный 52")]
    FiftyTwo_Black = 280072156,
    [Description("Лонгслив черный 54")]
    FiftyFour_Black = 284303410,
    #endregion

    #region Белый
    [Description("Лонгслив белый 38")]
    ThirtyEight_White = 284157822,
    [Description("Лонгслив белый 40")]
    Fourty_White = 275059690,
    [Description("Лонгслив белый 42")]
    FortyTwo_White = 273628129,
    [Description("Лонгслив белый 44")]
    FortyFour_White = 273628130,
    [Description("Лонгслив белый 46")]
    FortySix_White = 275062127,
    [Description("Лонгслив белый 48")]
    FourtyEight_White = 275062128,
    [Description("Лонгслив белый 50")]
    Fifty_White = 275062129,
    [Description("Лонгслив белый 52")]
    FiftyTwo_White = 280072155,
    [Description("Лонгслив белый 54")]
    FiftyFour_White = 284157823,
    #endregion

    #region Бежевый
    [Description("Лонгслив бежевый 38")]
    ThirtyEight_Biege = 285096393,
    [Description("Лонгслив бежевый 40")]
    Fourty_Biege = 275063817,
    [Description("Лонгслив бежевый 42")]
    FortyTwo_Biege = 275063774,
    [Description("Лонгслив бежевый 44")]
    FortyFour_Biege = 275063775,
    [Description("Лонгслив бежевый 46")]
    FortySix_Biege = 275063776,
    [Description("Лонгслив бежевый 48")]
    FourtyEight_Biege = 275063777,
    [Description("Лонгслив бежевый 50")]
    Fifty_Biege = 275063778,
    [Description("Лонгслив бежевый 52")]
    FiftyTwo_Biege = 280072152,
    [Description("Лонгслив бежевый 54")]
    FiftyFour_Biege = 285096394,
    #endregion

    #region Для девочки
    [Description("Лонгслив для девочки 38")]
    ThirtyEight_ColdWhite = 318851218,
    [Description("Лонгслив для девочки 40")]
    Fourty_ColdWhite = 318851219,
    [Description("Лонгслив для девочки 42")]
    FortyTwo_ColdWhite = 318851220,
    [Description("Лонгслив для девочки 44")]
    FortyFour_ColdWhite = 318851221,
    [Description("Лонгслив для девочки 46")]
    FortySix_ColdWhite = 318851222,
    [Description("Лонгслив для девочки 48")]
    FourtyEight_ColdWhite = 318851223,
    [Description("Лонгслив для девочки 50")]
    Fifty_ColdWhite = 318851224,
    [Description("Лонгслив для девочки 52")]
    FiftyTwo_ColdWhite = 318851225,
    [Description("Лонгслив для девочки 54")]
    FiftyFour_ColdWhite = 319093284,
    #endregion

    #region сиреневый, лавандово-розовый, лавандовый, светло-сиреневый, лиловый
    [Description("Лонгслив лавандовый 38")]
    ThirtyEight_Lavander = 318845717,
    [Description("Лонгслив лавандовый 40")]
    Fourty_Lavander = 318845718,
    [Description("Лонгслив лавандовый 42")]
    FortyTwo_Lavander = 318845719,
    [Description("Лонгслив лавандовый 44")]
    FortyFour_Lavander = 318845720,
    [Description("Лонгслив лавандовый 46")]
    FortySix_Lavander = 318845721,
    [Description("Лонгслив лавандовый 48")]
    FourtyEight_Lavander = 318845722,
    [Description("Лонгслив лавандовый 50")]
    Fifty_Lavander = 318845723,
    [Description("Лонгслив лавандовый 52")]
    FiftyTwo_Lavander = 318845724,
    [Description("Лонгслив лавандовый 54")]
    FiftyFour_Lavander = 319093283,
    #endregion

    #endregion

    #region Брюки палаццо широкие

    #region Черный
    [Description("Брюки палаццо широкие черные XS")]
    XS_WidePalazzoTrousers_black = 321181183,
    [Description("Брюки палаццо широкие черные S")]
    S_WidePalazzoTrousers_black = 321181184,
    [Description("Брюки палаццо широкие черные M")]
    M_WidePalazzoTrousers_black = 321181185,
    [Description("Брюки палаццо широкие черные L")]
    L_WidePalazzoTrousers_black = 321181186,
    [Description("Брюки палаццо широкие черные XL")]
    XL_WidePalazzoTrousers_black = 326294616,
    [Description("Брюки палаццо широкие черные 2XL")]
    TwoXL_WidePalazzoTrousers_black = 321181187,
    [Description("Брюки палаццо широкие черные 3XL")]
    ThreeXL_WidePalazzoTrousers_black = 326294617
    #endregion

    #endregion
}

public static class SizeTypeHelper
{
    /// <summary>
    /// Получает значение атрибута Description
    /// </summary>
    /// <param name="sizeType">Указатель на значение перечисления</param>
    /// <returns></returns>
    public static string GetDescription(this SizeType sizeType)
    {
        FieldInfo? field = sizeType.GetType().GetField(sizeType.ToString());
        if (field != null)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes != null && attributes.Length > 0)
            {
                return attributes[0].Description;
            }
        }
        return sizeType.ToString();

    }
}
