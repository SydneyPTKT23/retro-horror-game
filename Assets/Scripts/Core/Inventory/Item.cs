using UnityEngine;

namespace SLC.RetroHorror.Core
{
    [System.Serializable]
    public class Item
    {
        public enum Type
        {
            RangeWeapon = 0,
            MeleeWeapon,
            Ammo,
            Consumable,
            Note
        }

        /// <summary>
        /// This should be unique for each different item type!
        /// </summary>
        [field: SerializeField] public string ItemId { get; protected set; }
        [field: SerializeField] public string ItemName { get; protected set; }
        [field: SerializeField, TextArea(2, 10)] public string ItemDescription { get; protected set; }
        [field: SerializeField] public Type ItemType { get; protected set; }
        [field: SerializeField] public bool UsesAmmo { get; protected set; }
        [field: SerializeField] public string AmmoId { get; protected set; }
        [field: SerializeField] public int ItemMaxStack { get; protected set; }
        [field: SerializeField] public float Weight { get; protected set; }
        [field: SerializeField] public Mesh ItemMesh { get; protected set; }
        [field: SerializeField] public Material ItemMaterial { get; protected set; }
        
        //TODO : HIDE USESAMMO AND AMMOID IF NOT RANGEWEAPON
    }
}