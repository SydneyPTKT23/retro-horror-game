using System.Collections.Generic;
using UnityEngine;

namespace SLC.RetroHorror.Core
{
    [CreateAssetMenu(menuName = "ItemCatalogue")]
    public class ItemCatalogue : ScriptableObject
    {
        [SerializeField] private Item[] allItems;
        public Dictionary<string, Item> itemDictionary;

        public void InitializeItemDictionary()
        {
            itemDictionary = new();

            foreach (Item item in allItems)
            {
                itemDictionary.Add(item.ItemId, item);
            }
        }
    }
}
