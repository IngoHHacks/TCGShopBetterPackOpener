namespace TCGShopBetterPackOpener;

internal class PackProcessing
{
    public static bool ReturnEligible = false;
    public static bool PackFromMachine = false;
    public static List<CardData> PackToReturn = new();
    public static InteractableAutoPackOpener CurrentMachine = null;
}