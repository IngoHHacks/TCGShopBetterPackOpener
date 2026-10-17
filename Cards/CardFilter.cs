using TCGShopBetterPackOpener.Config;

namespace TCGShopBetterPackOpener.Cards;

public static class CardFilter
{
    public static bool FilterPack(List<CardData> cardDataList)
    {
        if (cardDataList.Count <= 0) return false;
        var packStr = CardUtils.CardsToString(cardDataList);
        var abbrStr = CardUtils.CardsToShortString(cardDataList);
        var totalValue = cardDataList.Sum(CPlayerData.GetCardMarketPrice);
        if (totalValue >= Settings.Instance.ReturnThreshold.Value)
        {
            LogDebug($"Pack value {totalValue} above {Settings.Instance.ReturnThreshold.Value}, returning pack with {abbrStr}.");
            Plugin.ProcessedLog.Add($"MATCH: {packStr} (value > {Settings.Instance.ReturnThreshold.Value})");
            return true;
        }

        var keywords = cardDataList.Select(CardUtils.GetCardKeywords).ToList();
        if (Match(cardDataList, keywords, Settings.Instance.AlwaysReturnFilter.Value.Split(',').Select(f => f.Trim().ToLower().Replace("-","").Replace("_","")).Where(f => !string.IsNullOrEmpty(f) && IsValid(f)).Select(f => f.Split(" ").ToList()).ToList(), out var match))
        {
            LogDebug($"Pack matches the filter [{match}], returning pack with {abbrStr}.");
            Plugin.ProcessedLog.Add($"MATCH: {packStr} (AlwaysReturnFilter: {match})");
            return true;
        }

        LogDebug($"Pack does not meet criteria: {abbrStr}");

        return false;
    }

    
    private static bool Match(List<CardData> cards, List<List<string>> keywords, List<List<string>> filters, out string match)
    {
        foreach (var filter in filters)
        {
            if (filter.Count == 0) continue;
            var origFilter = filter.ToList();
            if (filter.Contains("god"))
            {
                filter.Remove("god");
                if (filter.Count == 0)
                {
                    if (cards.All(c => c.expansionType == ECardExpansionType.Ghost) ||
                        cards.All(c => c.isFoil) ||
                        cards.All(c =>
                            c.borderType == cards.First().borderType &&
                            cards.First().borderType != ECardBorderType.Base))
                    {
                        match = string.Join(" ", origFilter);
                        return true;
                    }
                }
                else if (keywords.All(k => filter.All(k.Contains)))
                {
                    match = string.Join(" ", origFilter);
                    return true;
                }
            }
            else
            {
                if (keywords.Any(k => filter.All(k.Contains)))
                {
                    match = string.Join(" ", origFilter);
                    return true;
                }
            }
        }
        match = null;
        return false;
    }

    private static readonly List<string> RequiredKeywords = new() { 
        "ghost", "firstedition", "silver", "gold", "ex", "fullart", "borderless", "foil", "god", "firstedition+", "silver+", "gold+", "ex+", "fullart+", "borderless+"
    };

    private static bool IsValid(string filter)
    {
        return filter.Split(' ').Any(f => RequiredKeywords.Contains(f));
    }
}