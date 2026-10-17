using TCGShopBetterPackOpener.Cards;
using TCGShopBetterPackOpener.Config;

namespace TCGShopBetterPackOpener.Patches;

[HarmonyPatch]
internal static class CardOpeningSequencePatches
{
    // Check eligible packs if they match the keyword filter and queue them for return if they do.
    [HarmonyPatch(typeof(CardOpeningSequence), nameof(CardOpeningSequence.GetPackContentCardDataList))]
    [HarmonyPostfix]
    public static void CardOpeningSequence_GetPackContentCardDataList(ref List<CardData> __result)
    {
        if (!Settings.Instance.Enabled.Value) return;
        if (PackProcessing.ReturnEligible)
        {
            if (CardFilter.FilterPack(__result))
            {
                PackProcessing.PackToReturn = new List<CardData>();
                foreach (var card in __result)
                {
                    var copy = new CardData();
                    copy.CopyData(card);
                    PackProcessing.PackToReturn.Add(copy);
                }
                __result.Clear();
            }
        }
    }
    
    [HarmonyPatch(typeof(CardOpeningSequence), nameof(CardOpeningSequence.GetPackContent))]
    [HarmonyPrefix]
    public static bool CardOpeningSequence_GetPackContent(ref CardOpeningSequence __instance, bool clearList,
        bool isSecondaryRolledData, ECollectionPackType overrideCollectionPackType)
    {
        var type = overrideCollectionPackType == ECollectionPackType.None
            ? __instance.m_CollectionPackType
            : overrideCollectionPackType;
        if (PackProcessing.PackFromMachine)
        {
            LogDebug($"Pack opening sequence initiated by machine, type {type}");
            return true;
        }
        if (PackProcessing.CurrentMachine == null)
        {
            return true;
        }

        if (isSecondaryRolledData)
        {
            // isSecondaryRolledData is only used for Ghost god packs. These override regular data. We don't want our queued packs to be overridden by these because they're supposed to be final.
            return false;
        }
        var machine = StoredPackData.Instance.GetOrAddMachine(PackProcessing.CurrentMachine);
        var queue = machine.Queue;
        LogDebug($"Opening pack of type {type}");
        if (queue.QueuedPacks.ContainsKey(type) && queue.QueuedPacks[type].Count > 0)
        {
            LogDebug($"Returning queued pack of type {type} with cards {CardUtils.CardsToString(queue.QueuedPacks[type][0])}, value {queue.QueuedPacks[type][0].Sum(CPlayerData.GetCardMarketPrice)} (Remaining: {queue.QueuedPacks[type].Count - 1})");
            if (clearList)
            {
                __instance.m_RolledCardDataList.Clear();
            }
            __instance.m_RolledCardDataList.AddRange(queue.QueuedPacks[type][0]);

            __instance.m_HasFoilCard = queue.QueuedPacks[type][0].Any(card => card.isFoil);
            queue.QueuedPacks[type].RemoveAt(0);
            return false;
        }
        return true;
    }
}