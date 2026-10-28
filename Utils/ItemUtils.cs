namespace TCGShopBetterPackOpener.Utils;

public static class ItemUtils
{
    public static Item GiveItemToPlayer(EItemType itemType, Transform parent, Vector3? offset = null,
        Quaternion? rotation = null)
    {
        ItemMeshData meshData = InventoryBase.GetItemMeshData(itemType);
        var item = ItemSpawnManager.GetItem(parent);
        item.SetMesh(meshData.mesh, meshData.material, itemType, meshData.meshSecondary,
            meshData.materialSecondary);
        item.transform.localPosition = offset ?? Vector3.zero;
        item.transform.localRotation = rotation ?? Quaternion.identity;
        item.m_Collider.enabled = false;
        item.GetOrCreateRigidbody().isKinematic = true;
        CSingleton<InteractionPlayerController>.Instance.AddHoldItemToFront(item);
        item.gameObject.SetActive(true);
        return item;
    }
}