using SLC.RetroHorror.DataPersistence;
using UnityEngine;

namespace SLC.RetroHorror.Core
{
    public class PickupTemplate : InteractableBase
    {
        [SerializeField] private Item itemToAdd;
        [SerializeField] private int amountToAdd;
        public bool ItemWasCollected { get; private set; }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        protected override void Start()
        {
            base.Start();
        }

        #region interaction

        public override void OnInteract(InteractionController controller)
        {
            base.OnInteract(controller);
        }

        #endregion

        #region data persistence

        private const string collectedKey = "pickupWasCollected";

        public override SaveData Save()
        {
            SaveData data = base.Save();
            data.bools.Add(collectedKey, ItemWasCollected);

            return data;
        }

        public override void Load(SaveData data)
        {
            base.Load(data);
            ItemWasCollected = data.bools[collectedKey];
        }

        #endregion
    }
}
