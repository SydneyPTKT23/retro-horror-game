using System.Collections.Generic;
using UnityEngine;

namespace SLC.RetroHorror.DataPersistence
{
    [System.Serializable]
    public class SaveData
    {
        public string uniqueId;
        public bool active;
        public Vector3 localPosition;
        public Quaternion localRotation;
        public Dictionary<string, bool> bools = new();
        public Dictionary<string, int> ints = new();
        public Dictionary<string, float> floats = new();
        public Dictionary<string, string> strings = new();
    }

    [System.Serializable]
    public class SceneSaveData
    {
        public string SceneName;
        public List<SaveData> SaveDatas = new();

        public SceneSaveData(string _sceneName)
        {
            SceneName = _sceneName;
        }
    }
}