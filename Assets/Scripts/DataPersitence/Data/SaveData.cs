using System.Collections.Generic;
using UnityEngine;

namespace SLC.RetroHorror.DataPersistence
{
    [System.Serializable]
    public class SaveData
    {
        public string uniqueId;
        public bool active;
        public SerializableVector3 localPosition;
        public SerializableQuaternion localRotation;
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

    [System.Serializable]
    public struct SerializableVector3
    {
        public float x, y, z;

        public SerializableVector3(float rX, float rY, float rZ)
        {
            x = rX;
            y = rY;
            z = rZ;
        }

        public override readonly string ToString()
        {
            return string.Format("[{0}, {1}, {2}]", x, y, z);
        }

        public static implicit operator Vector3(SerializableVector3 rValue)
        {
            return new Vector3(rValue.x, rValue.y, rValue.z);
        }

        public static implicit operator SerializableVector3(Vector3 rValue)
        {
            return new SerializableVector3(rValue.x, rValue.y, rValue.z);
        }
    }
    public struct SerializableQuaternion
    {
        public float x, y, z, w;


        public SerializableQuaternion(float rX, float rY, float rZ, float rW)
        {
            x = rX;
            y = rY;
            z = rZ;
            w = rW;
        }

        public override readonly string ToString()
        {
            return string.Format("[{0}, {1}, {2}, {3}]", x, y, z, w);
        }

        public static implicit operator Quaternion(SerializableQuaternion rValue)
        {
            return new Quaternion(rValue.x, rValue.y, rValue.z, rValue.w);
        }

        public static implicit operator SerializableQuaternion(Quaternion rValue)
        {
            return new SerializableQuaternion(rValue.x, rValue.y, rValue.z, rValue.w);
        }
    }
}