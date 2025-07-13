using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using UnityEngine.SceneManagement;
using Unity.Plastic.Newtonsoft.Json;

namespace SLC.RetroHorror.DataPersistence
{
    public class DataPersistenceManager : MonoBehaviour
    {
        [Header("File Storage Config")]
        public static DataPersistenceManager Instance { get; private set; }
        private Dictionary<string, IDataPersistence> instanceCache = new();
        private string dataDirPath = "";

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.Log("Found more than one DataPersistenceManager, fixing");
                Destroy(gameObject);
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            dataDirPath = Path.Combine(Application.persistentDataPath, "saves");
        }

        private void Start()
        {
            UpdateInstanceCache();
        }

        public void LoadGame(string _saveName)
        {
            //Add save name to filepath to get full file path
            string path = Path.Combine(dataDirPath, _saveName + ".json");
            Debug.Log(message: $"Loading data from {path}");

            //Error handling in case file is missing
            if (!File.Exists(path))
            {
                Debug.LogError($"Couldn't find data saved at:\n{path}");
                return;
            }

            //Try to load data from file path, then deserialize it to JSON with JsonUtility
            try
            {
                string dataToLoad = "";
                using (FileStream stream = new(path, FileMode.Open))
                {
                    using StreamReader reader = new(stream);
                    dataToLoad = reader.ReadToEnd();
                }
                //deserialize loaded JSON file to string
                SceneSaveData loadedData = JsonConvert.DeserializeObject<SceneSaveData>(dataToLoad);
                Debug.Log($"Loaded data:\n{dataToLoad}");
                SetLoadedSceneSaveData(loadedData);
            }
            //If load couldn't be handled, log error
            catch (System.Exception e)
            {
                Debug.LogError(message: $"Error occured while trying to load data from file: {path}\n{e}");
            }
        }

        public void SaveGame(string _saveName)
        {
            //Add save name to filepath to get full file path
            string path = Path.Combine(dataDirPath, _saveName + ".json");
            Debug.Log(message: $"Saving data to {path}");

            //Try to save data to file path, then serialize it to JSON with JsonUtility
            try
            {
                //Creating directory for saves if it doesn't exist
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                //Serialize GameData object into a JSON file
                string serializedGameState = JsonConvert.SerializeObject(GetSceneSave(), Formatting.Indented);
                //write data to file
                using FileStream stream = new(path, FileMode.Create);
                using StreamWriter writer = new(stream);
                writer.Write(serializedGameState);
            }
            catch (System.Exception e)
            {
                Debug.LogError(message: $"Error occured while trying to save data to file: {path}\n{e}");
            }
        }

        private void UpdateInstanceCache()
        {
            instanceCache = new();
            FindObjectsByType<SaveableMonoBehaviour>(FindObjectsSortMode.None).ToList().
                ForEach((saveable) => instanceCache.Add(saveable.ID, saveable));
        }

        //Create new SceneSaveData instance that holds list of all saveables and can be serialized
        public SceneSaveData GetSceneSave()
        {
            List<SaveData> saveDatas = new();
            instanceCache.Values.ToList().ForEach(obj => saveDatas.Add(obj.Save()));

            //SceneSaveData object if "parent" for all saveables. We would also need something like this, if we had multiple scenes
            SceneSaveData sceneSave = new(SceneManager.GetActiveScene().name)
            {
                SaveDatas = saveDatas
            };

            return sceneSave;
        }

        //Find appropriate objects with their loaded UniqueIDs and restore saved states (by calling each ISaveable's (SaveableMonos are ISaveable) Load method)
        public void SetLoadedSceneSaveData(SceneSaveData _loadedSceneSaveData)
        {
            _loadedSceneSaveData.SaveDatas.ForEach(save =>
            {
                instanceCache[save.uniqueId].Load(save);
                Debug.Log($"Save unique id: {save.uniqueId}");
            });
        }
    }
}