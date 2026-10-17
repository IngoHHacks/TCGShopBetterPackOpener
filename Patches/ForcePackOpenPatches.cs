namespace TCGShopBetterPackOpener.Patches;

[HarmonyPatch]
internal static class ForcePackOpenPatches
{
    // Force the pack opening sequence to start if the player received packs from the pack opening machine.
    [HarmonyPatch(typeof(InteractionPlayerController), nameof(InteractionPlayerController.Update))]
    [HarmonyPrefix]
    public static void InteractionPlayerController_Update(InteractionPlayerController __instance)
    {
        if (PackProcessing.CurrentMachine != null)
        {
            if (__instance.m_IsInUIMode || CSingleton<CardOpeningSequence>.Instance.m_IsReadyingToOpen)
            {
                return;
            }
            var queue = StoredPackData.Instance.GetOrAddMachine(PackProcessing.CurrentMachine).Queue;
            if (queue.QueuedPacks.Sum(p => p.Value.Count) == 0)
            {
                PackProcessing.CurrentMachine = null;
                return;
            }
            if (!__instance.CanOpenPack())
            {
                PackProcessing.CurrentMachine = null;
                return;
            }
            if (!CSingleton<CardOpeningSequence>.Instance.m_IsScreenActive)
            {
                __instance.EvaluateOpenCardPack();
            }
        }
    }
    
    // Prevent the player from canceling the pack opening sequence if it was initiated by the machine.
    [HarmonyPatch(typeof(InputManager), nameof(InputManager.GetKeyDownAction))]
    [HarmonyPrefix]
    public static bool InputManager_GetKeyDownAction(EGameAction action, ref bool __result)
    {
        if (action == EGameAction.CancelOpenPack && PackProcessing.CurrentMachine != null)
        {
            __result = false;
            return false;
        }
        return true;
    }
    
    // Hide the cancel tooltip if the pack opening sequence was initiated by the machine.
    [HarmonyPatch(typeof(CardOpeningSequence), nameof(CardOpeningSequence.ReadyingCardPack))]
    [HarmonyPostfix]
    public static void CardOpeningSequence_ReadyingCardPack()
    {
        if (PackProcessing.CurrentMachine != null)
        {
            InteractionPlayerController.RemoveToolTip(EGameAction.CancelOpenPack);
        }
    }
}