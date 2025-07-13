using System.Collections.Generic;
using System.Linq;
using SLC.RetroHorror.DataPersistence;
using UnityEngine;

namespace SLC.RetroHorror.Core
{
    public class GameManager : MonoBehaviour
    {
        #region Default Methods

        private void Awake()
        {
            CheckForDuplicateIds();
        }

        #endregion

        #region Error Handling

        private void CheckForDuplicateIds()
        {
            List<UniqueId> uniqueIds = FindObjectsByType<UniqueId>(FindObjectsSortMode.None).ToList();
            List<string> ids = new();
            uniqueIds.ForEach(id => ids.Add(id.uniqueId));

            ids.GroupBy(i => i).Where(g => g.Count() > 1).Select(g => g.Key).ToList().ForEach((id) =>
            {
                string error = $"Found duplicate uniqueId: {id}\nOn objects: ";
                uniqueIds.Where(uid => uid.uniqueId == id).ToList().ForEach(uid => error += $"{uid.name} ");
                Debug.LogError(error);
            });
        }

        #endregion
    }
}