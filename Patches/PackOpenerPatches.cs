using TCGShopBetterPackOpener.Config;
using TMPro;

namespace TCGShopBetterPackOpener.Patches;

[HarmonyPatch]
internal static class PackOpenerPatches
{
    // Mark the pack as being opened by the machine and eligible (if applicable) for the pack opening sequence to process.
    [HarmonyPatch(typeof(InteractableAutoPackOpener), nameof(InteractableAutoPackOpener.OpenPack))]
    [HarmonyPrefix]
    public static void InteractableAutoPackOpener_OpenPack(InteractableAutoPackOpener __instance)
    {
        PackProcessing.PackFromMachine = true;
        if (__instance.m_StoredItemList.Count > 1 ||
            __instance.m_CompactCardDataAmountList.Count > 0) // If exactly one pack is left and no packs have been opened yet, we force the last one to be opened by the machine regardless of its value so that the list is not empty, since the machine can only be interacted with if at least one card is in the list. This is to prevent the machine from being stuck in a state where it cannot be interacted with because the list is empty after processing the last pack.
        {
            PackProcessing.ReturnEligible = true;
            PackProcessing.PackToReturn.Clear();
        }
    }
    
    // Reset the flags after the pack opening machine has finished opening the pack and queue the pack for return if the pack was marked as such.
    [HarmonyPatch(typeof(InteractableAutoPackOpener), nameof(InteractableAutoPackOpener.OpenPack))]
    [HarmonyPostfix]
    public static void InteractableAutoPackOpener_OpenPackPost(InteractableAutoPackOpener __instance)
    {
        PackProcessing.PackFromMachine = false;
        PackProcessing.ReturnEligible = false;
        if (PackProcessing.PackToReturn.Count <= 0) return;
        LogDebug($"Returning pack of value {PackProcessing.PackToReturn.Sum(CPlayerData.GetCardMarketPrice)} [Type: {InventoryBase.ItemTypeToCollectionPackType(__instance.GetStoredItemList().First().m_ItemType)}]");
        __instance.m_PackOpenedCount -= 1;
        var machine = StoredPackData.Instance.GetOrAddMachine(__instance);
        var queue = machine.Queue;
        var type = __instance.GetStoredItemList().First().m_ItemType;
        var packType = InventoryBase.ItemTypeToCollectionPackType(type);
        if (!queue.QueuedPacks.ContainsKey(packType))
        {
            queue.QueuedPacks[packType] = new List<List<CardData>>();
        }
        queue.QueuedPacks[packType].Add(PackProcessing.PackToReturn.ToList());
        queue.QueuedItems.Add(type);
        PackProcessing.PackToReturn.Clear();
    }
    
    // Return the queued packs to the player when they interact with the machine.
    [HarmonyPatch(typeof(InteractableAutoPackOpener), nameof(InteractableAutoPackOpener.OnMouseButtonUp))]
    [HarmonyPrefix]
    public static void InteractableAutoPackOpener_OnMouseButtonUp(InteractableAutoPackOpener __instance)
    {
        if (PackProcessing.CurrentMachine != null)
        {
            return;
        }
        PackProcessing.CurrentMachine = __instance;
        var machine = StoredPackData.Instance.GetOrAddMachine(__instance);
        var queue = machine.Queue;
        if (queue.QueuedItems.Count <= 0)
        {
            PackProcessing.CurrentMachine = null;
            return;
        }
        foreach (var type in queue.QueuedItems)
        {
            var offset = __instance.transform.forward * 0.4f + Vector3.up * 0.5f;
            ItemUtils.GiveItemToPlayer(type, __instance.transform, offset);
        }
        queue.QueuedItems.Clear();
    }
    
    // Adds the number of queued packs to the UI
    [HarmonyPatch(typeof(InteractableAutoPackOpener), nameof(InteractableAutoPackOpener.Update))]
    [HarmonyPostfix]
    public static void InteractableAutoPackOpener_Update(InteractableAutoPackOpener __instance)
    {
        if (Settings.Instance.DisplayReturnedAmount.Value)
        {
            if (__instance.m_IsBoxedUp)
            {
                return;
            }

            var queue = StoredPackData.Instance.GetOrAddMachine(__instance).Queue;
            var textMesh = __instance.m_AutoCardOpenerUI.m_DoneGrp.GetComponentInChildren<TextMeshProUGUI>();
            var textMesh2 = __instance.m_AutoCardOpenerUI.m_ProcessingTimeLeft;
            if (!textMesh.text.EndsWith($"\n[{queue.QueuedItems.Count} returned]"))
            {
                if (textMesh.text.EndsWith("]"))
                {
                    textMesh.text = textMesh.text.Substring(0, textMesh.text.LastIndexOf('[') - 1).TrimEnd();
                }

                textMesh.text += $"\n[{queue.QueuedItems.Count} returned]";
            }

            if (!textMesh2.text.EndsWith($"\n[{queue.QueuedItems.Count} returned]"))
            {
                if (textMesh2.text.EndsWith("]"))
                {
                    textMesh2.text = textMesh2.text.Substring(0, textMesh2.text.LastIndexOf('[') - 1).TrimEnd();
                }

                textMesh2.text += $"\n[{queue.QueuedItems.Count} returned]";
            }
        }
        else
        {
            var textMesh = __instance.m_AutoCardOpenerUI.m_DoneGrp.GetComponentInChildren<TextMeshProUGUI>();
            var textMesh2 = __instance.m_AutoCardOpenerUI.m_ProcessingTimeLeft;
            if (textMesh.text.EndsWith("]"))
            {
                textMesh.text = textMesh.text.Substring(0, textMesh.text.LastIndexOf('[') - 1).TrimEnd();
            }
            if (textMesh2.text.EndsWith("]"))
            {
                textMesh2.text = textMesh2.text.Substring(0, textMesh2.text.LastIndexOf('[') - 1).TrimEnd();
            }
        }
    }
}