using System;
using System.Collections.Generic;
using UnityEngine;

namespace SLC.RetroHorror.Core
{
    public class PlayerInventoryManager : MonoBehaviour
    {
        [SerializeField] private Transform inventoryEntryHolder;
        [SerializeField] private GameObject itemEntryPrefab;
        [SerializeField] private Inventory inventory;

        public void UpdateInventoryState()
        {
            ClearInventory();

            foreach (KeyValuePair<string, InventoryEntry> item in inventory.InventoryItems)
            {
                ItemUI itemUI = Instantiate(itemEntryPrefab, inventoryEntryHolder).GetComponent<ItemUI>();
                itemUI.itemNameField.text = item.Value.ItemName;
                itemUI.itemDescField.text = item.Value.ItemDescription;
                itemUI.itemCountField.text = string.Concat("x", item.Value.Amount.ToString());
                itemUI.itemWeightField.text = string.Concat(((float)item.Value.Amount * item.Value.ItemWeight).ToString("0.000"), "kg");
            }
        }

        private void ClearInventory()
        {
            foreach (Transform child in inventoryEntryHolder)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
