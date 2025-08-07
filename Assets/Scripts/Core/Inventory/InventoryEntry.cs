namespace SLC.RetroHorror.Core
{
    /// <summary>
    /// Expanding the Item class to add an amount so inventories can handle multiple items
    /// </summary>
    [System.Serializable]
    public class InventoryEntry : Item
    {
        public int Amount { get; set; }

        public InventoryEntry(Item _item, int _amount = 1)
        {
            ItemId = _item.ItemId;
            ItemName = _item.ItemId;
            ItemDescription = _item.ItemDescription;
            ItemType = _item.ItemType;
            ItemMaxStack = _item.ItemMaxStack;
            UsesAmmo = _item.UsesAmmo;
            AmmoId = _item.AmmoId;
            ItemWeight = _item.ItemWeight;
            ItemMesh = _item.ItemMesh;
            ItemMaterial = _item.ItemMaterial;
            Amount = _amount;
        }
    }
}
