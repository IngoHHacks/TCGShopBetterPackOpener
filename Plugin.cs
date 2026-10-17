using TCGShopBetterPackOpener.Config;

namespace TCGShopBetterPackOpener;

[BepInPlugin(PluginGuid, PluginName, PluginVer)]
[HarmonyPatch]
public class Plugin : BaseUnityPlugin
{
    public const string PluginGuid = "net.ingoh.tcgcardshopsimulator.betterpackopener";
    public const string PluginName = "Better Pack Opener";
    public const string PluginVer = "1.0.0";

    public static Plugin Instance { get; private set; }

    internal static ManualLogSource Log;
    internal readonly static Harmony Harmony = new(PluginGuid);

    internal static string PluginPath;
    
    public static readonly List<string> ProcessedLog = new();
    
    internal static bool Debug = false;


    private void Awake()
    {
        Plugin.Log = base.Logger;
        PluginPath = Path.GetDirectoryName(Info.Location);

        Instance = this;

        Settings.Instance.Init(Config);
    }

    private void OnEnable()
    {
        Harmony.PatchAll();
        Logger.LogInfo($"Loaded {PluginName}!");
    }

    private void OnDisable()
    {
        Harmony.UnpatchSelf();
        Logger.LogInfo($"Unloaded {PluginName}!");
    }
    
    private void Update()
    {
        if (Settings.Instance.DebugKeybind.Value.IsDown())
        {
            Debug = !Debug;
            Log.LogInfo($"Debug mode {(Debug ? "enabled" : "disabled")}.");
        }
    }

    private void OnGUI()
    {
        if (!Debug) return;
        GUI.Label(new Rect(10, 10, 10000, 20), $"Better Pack Opener Debug Info");
        GUI.Label(new Rect(10, 30, 10000, 20), $"Machines: {StoredPackData.Instance.Machines.Count}");
        int y = 50;
        foreach (var machine in StoredPackData.Instance.Machines)
        {
            if (machine.Key.Machine == null)
            {
                continue;
            }
            GUI.Label(new Rect(10, y, 10000, 20), $"Machine at {machine.Key.PositionString}: {machine.Value.QueuedItems.Count} queued packs");
            y += 20;
            foreach (var pack in machine.Value.QueuedPacks)
            {
                GUI.Label(new Rect(30, y, 10000, 20), $"> Pack type {pack.Key}: {pack.Value.Count} queued packs");
                y += 20;
            }
        }
        if (PackProcessing.CurrentMachine != null)
        {
            GUI.Label(new Rect(10, y, 10000, 20), "Current packs:");
            y += 20;
            foreach (var pack in StoredPackData.Instance.GetOrAddMachine(PackProcessing.CurrentMachine).Queue.QueuedPacks)
            {
                foreach (var queuedPack in pack.Value)
                {
                    GUI.Label(new Rect(30, y, 10000, 20), $"> Pack type {pack.Key}, cards: {CardUtils.CardsToString(queuedPack)} value: {queuedPack.Sum(CPlayerData.GetCardMarketPrice)}");
                    y += 20;
                }
            }
        }

        if (ProcessedLog.Count > 100)
        {
            ProcessedLog.RemoveRange(0, ProcessedLog.Count - 100);
        }
        foreach (var log in ProcessedLog.TakeLast((Screen.height - y) / 20 - 1))
        {
            GUI.Label(new Rect(10, y, 10000, 20), log);
            y += 20;
        }
    }
}