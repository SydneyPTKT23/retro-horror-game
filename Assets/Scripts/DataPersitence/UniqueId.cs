using UnityEngine;

namespace SLC.RetroHorror.DataPersistence
{
    public class UniqueIdentifierAttribute : PropertyAttribute { }

    public class UniqueId : MonoBehaviour
    {
        [UniqueIdentifier]
        public string uniqueId;
    }
}