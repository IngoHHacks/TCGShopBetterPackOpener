namespace TCGShopBetterPackOpener.Utils;

public static class CardUtils
{
    public static string CardsToString(List<CardData> cardDataList)
    {
        return string.Join(", ", cardDataList.Select(c => $"{CPlayerData.GetFullCardTypeName(c)} {InventoryBase.GetMonsterData(c.monsterType).GetName()}"));
    }

    public static string CardsToShortString(List<CardData> cardDataList)
    {
        return string.Join(" ", cardDataList.Select(GetCardAbbreviatedKeywords));
    }
    
    public static List<string> GetCardKeywords(CardData card)
    {
        var keywords = new List<string>();
        if (card.isFoil) keywords.Add("foil");

        var alt = false;
        switch (card.expansionType)
        {
            case ECardExpansionType.Tetramon:
                keywords.Add("tetramon");
                break;
            case ECardExpansionType.Destiny:
                keywords.Add("destiny");
                break;
            case ECardExpansionType.Ascension:
                keywords.Add("ascension");
                alt = true;
                break;
            case ECardExpansionType.Ghost:
                keywords.Add("ghost");
                break;
            default:
                keywords.Add(card.expansionType.ToString().ToLower());
                break;
        }

        keywords.Add("basic+");
        if (alt)
        {
            switch (card.borderType)
            {
                case ECardBorderType.Base:
                    AddBorderAndLower(keywords, "basic");
                    break;
                case ECardBorderType.FirstEdition:
                    AddBorderAndLower(keywords, "silver");
                    break;
                case ECardBorderType.Silver:
                    AddBorderAndLower(keywords, "gold");
                    break;
                case ECardBorderType.Gold:
                    AddBorderAndLower(keywords, "silver");
                    keywords.Add("fullart");
                    keywords.Add("fullart+");
                    break;
                case ECardBorderType.EX:
                    AddBorderAndLower(keywords, "ex");
                    keywords.Add("fullart");
                    keywords.Add("fullart+");
                    break;
                case ECardBorderType.FullArt:
                    AddBorderAndLower(keywords, "borderless");
                    break;
            }
        }
        else
        {
            switch (card.borderType)
            {
                case ECardBorderType.Base:
                    AddBorderAndLower(keywords, "basic");
                    break;
                case ECardBorderType.FirstEdition:
                    AddBorderAndLower(keywords, "firstedition");
                    break;
                case ECardBorderType.Silver:
                    AddBorderAndLower(keywords, "silver");
                    break;
                case ECardBorderType.Gold:
                    AddBorderAndLower(keywords, "gold");
                    break;
                case ECardBorderType.EX:
                    AddBorderAndLower(keywords, "ex");
                    break;
                case ECardBorderType.FullArt:
                    AddBorderAndLower(keywords, "fullart");
                    break;
            }
        }
        return keywords;
    }

    public static string GetCardAbbreviatedKeywords(CardData card)
    {
        var keywords = "";
        if (card.isNew) keywords += "N";
        if (card.isFoil) keywords += "F";
        if (card.expansionType == ECardExpansionType.Ghost)
        {
            return keywords + "G";
        }
        return keywords + (int)card.borderType;
    }

    private static List<string> BorderOrder = new()
    {
        "basic", "firstedition", "silver", "gold", "ex", "fullart", "borderless"
    };
    
    private static void AddBorderAndLower(List<string> list, string border)
    {
        list.Add(border);
        list.Add(border + "+");
        for (int i = BorderOrder.IndexOf(border) - 1; i >= 0; i--)
        {
            list.Add(BorderOrder[i] + "+");
        }
    }
    
}