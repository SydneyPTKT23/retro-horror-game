using UnityEngine;

namespace SLC.RetroHorror.DataPersistence
{
    [RequireComponent(typeof(UniqueId))]
    public class SaveableMonoBehaviour : MonoBehaviour, IDataPersistence
    {
        private UniqueId uniqueIdComponent;
        public string ID => uniqueIdComponent.uniqueId;

        protected virtual void Awake()
        {
            uniqueIdComponent = GetComponent<UniqueId>();
        }

        #region Data Persistence

        public virtual SaveData Save()
        {
            return new SaveData
            {
                uniqueId = ID,
                active = gameObject.activeSelf,
                localPosition = transform.localPosition,
                localRotation = transform.localRotation
            };
        }

        public virtual void Load(SaveData data)
        {
            gameObject.SetActive(data.active);
            transform.SetLocalPositionAndRotation(data.localPosition, data.localRotation);
        }

        #endregion
    }
}