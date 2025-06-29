using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace SLC.RetroHorror.DataPersistence
{
    public class DataPersistenceManager : MonoBehaviour
    {
        [Header("File Storage Config")]
        [SerializeField] private string fileName;
        public static DataPersistenceManager instance { get; private set; }
        private FileDataHandler dataHandler;
        private List<IDataPersistence> dataPersistences;
        private GameData gameData;

        private void Awake()
        {
            if (instance != null)
            {
                Debug.Log("Found more than one DataPersistenceManager, fixing");
                Destroy(gameObject);
            }
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
            this.dataPersistences = FindAllDataPersistences();
            LoadGame();
        }

        private void NewGame()
        {
            this.gameData = new GameData();
        }

        private void LoadGame()
        {
            //load saved data from file using datahandler, no data returns null
            this.gameData = dataHandler.Load();
            if (this.gameData == null)
            {
                Debug.Log("No saved data was found, initializing with default values");
                NewGame();
            }
            //pass data to other scripts so it can be used
            foreach(IDataPersistence dataPersistence in dataPersistences)
            {
                dataPersistence.LoadData(gameData);
            }
        }

        public void SaveGame()
        {
            //pass data to other scripts so it can be updated
            foreach(IDataPersistence dataPersistence in dataPersistences)
            {
                dataPersistence.SaveData(ref gameData);
            }
            //save data to file using datahandler
            dataHandler.Save(gameData);
        }

        private List<IDataPersistence> FindAllDataPersistences()
        {
            IEnumerable<IDataPersistence> dataPersistences = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IDataPersistence>();

            return new List<IDataPersistence>(dataPersistences);
        }
    }
}