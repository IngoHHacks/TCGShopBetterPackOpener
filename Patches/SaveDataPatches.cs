using Newtonsoft.Json;

namespace TCGShopBetterPackOpener.Patches;

[HarmonyPatch]
internal static class SaveDataPatches
{
    // Save the machine queue data to a separate file when the game saves
    [HarmonyPatch(typeof(CGameManager), nameof(CGameManager.SaveGameData))]
    [HarmonyPostfix]
    public static void CGameManager_SaveGameData(int saveSlotIndex)
    {
        try
        {
            var path = Application.persistentDataPath + $"/modSave_BetterPackOpener{saveSlotIndex}.json";
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            foreach (var machineQueue in StoredPackData.Instance.Machines)
            {
                foreach (var key in machineQueue.Value.QueuedPacks.Keys.ToList().Where(key => machineQueue.Value.QueuedPacks[key].Count <= 0))
                {
                    machineQueue.Value.QueuedPacks.Remove(key);
                }
            }

            var json = JsonConvert.SerializeObject(
                StoredPackData.Instance.Machines.Where(m => m.Key.Machine != null && !m.Key.Machine.m_IsBoxedUp)
                    .ToDictionary(kvp => kvp.Key.PositionString, kvp => kvp.Value), Formatting.None);
            File.WriteAllText(path, json);
        }
        catch (Exception e)
        {
            LogError($"Failed to save machine queue data: {e}");
        }
    }

    // Load the machine queue data from the separate file when the game loads
    [HarmonyPatch(typeof(CGameManager), nameof(CGameManager.LoadData))]
    [HarmonyPostfix]
    public static void CGameManager_LoadData(CGameManager __instance)
    {
        try
        {
            var slotIndex = __instance.m_CurrentSaveLoadSlotSelectedIndex;
            var path = Application.persistentDataPath + $"/modSave_BetterPackOpener{slotIndex}.json";
            StoredPackData.Instance.Machines.Clear();
            PackProcessing.CurrentMachine = null;
            PackProcessing.PackToReturn.Clear();
            PackProcessing.ReturnEligible = false;
            if (!File.Exists(path))
            {
                return;
            }

            for (var i = 0; i < ShelfManager.Instance.m_SpawnedAutoPackOpenerList.Count; i++)
            {
                var packOpener = ShelfManager.Instance.m_SpawnedAutoPackOpenerList[i];
                if (!ShelfManager.Instance.m_AutoPackOpenerSaveDataList[i].isBoxed) // m_IsBoxedUp isn't applied until half a second after the game loads, so we check the save data instead to see if the machine is boxed up.
                {
                    StoredPackData.Instance.Machines[new MachineRef(packOpener)] = new MachineQueue();
                }
            }

            var json = File.ReadAllText(path);
            var machines = JsonConvert.DeserializeObject<Dictionary<string, MachineQueue>>(json);
            foreach (var machine in machines)
            {
                var pos = machine.Key.StringToVector();
                var existingMachine = StoredPackData.Instance.GetMachineAtPos(pos);
                if (existingMachine != null)
                {
                    StoredPackData.Instance.Machines[existingMachine] = machine.Value;
                }
                else
                {
                    LogWarning($"Failed to load machine at position {pos}: No matching machine found.");
                }
            }
        }
        catch (Exception e)
        {
            LogError($"Failed to load machine queue data: {e}");
        }
    }
}