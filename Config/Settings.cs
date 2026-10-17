using BepInEx.Configuration;

namespace TCGShopBetterPackOpener.Config;

public class Settings
{
    public static Settings Instance { get; private set; } = new();

    public ConfigEntry<bool> Enabled;
    public ConfigEntry<float> ReturnThreshold;
    public ConfigEntry<string> AlwaysReturnFilter;
    public ConfigEntry<bool> DisplayReturnedAmount;
    public ConfigEntry<KeyboardShortcut> DebugKeybind;

    public void Init(ConfigFile config)
    {
        Enabled = config.Bind("General", "Enabled", true, "Whether this mod is enabled or not.");
        ReturnThreshold = config.Bind("Filters", "ReturnThreshold", 100f,
            new ConfigDescription(
                "If the total market value of a pack is equal to or above this value, the pack opening machines will return it. This value is in dollars. Refer to the README for conversion rates for other currencies.",
                new AcceptableValueRange<float>(10f, 100000f)));
        AlwaysReturnFilter = config.Bind("Filters", "AlwaysReturnFilter", "",
            new ConfigDescription(
                "A comma-separated list of card keyword groups that will always have packs containing its cards returned by the pack opening machines, regardless of their value. See the README for a list of keywords. For example, 'ghost,foil fullart,god ex+' will return any pack that contains any Ghost expansion card, a Foil Full Art border card, or a God Pack with all EX or higher border cards. Leave empty to disable this filter."));
        DisplayReturnedAmount = config.Bind("General", "DisplayReturnedAmount", true,
            "Whether pack opening machines will display the amount of packs queued to be returned to the player.");
        DebugKeybind = config.Bind("Debug", "DebugKeybind", new KeyboardShortcut(KeyCode.F2), "The keybind to toggle the debug GUI.");
    }
}