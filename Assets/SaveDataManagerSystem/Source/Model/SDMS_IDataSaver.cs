
namespace SaveDataManagerSystem.Model
{
    /// <summary>
    /// データのセーブを行うサービス
    /// </summary>
    public interface SDMS_IDataSaver
    {
        /// <summary>
        /// <paramref name="jsonData"/> のセーブを行う
        /// </summary>
        /// <param name="jsonData">実際の保存対象のjson</param>
        public void Save(string jsonData);
    }
}