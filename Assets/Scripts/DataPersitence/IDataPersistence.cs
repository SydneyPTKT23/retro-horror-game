namespace SLC.RetroHorror.DataPersistence
{
    public interface IDataPersistence
    {
        void Load(SaveData data);
        SaveData Save();
    }
}