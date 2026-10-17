using Newtonsoft.Json;

namespace TCGShopBetterPackOpener;

public class MachineQueue
{
    [JsonProperty("queuedPacks")]
    public Dictionary<ECollectionPackType, List<List<CardData>>> QueuedPacks = new();
    [JsonProperty("queuedItems")]
    public List<EItemType> QueuedItems = new();
}