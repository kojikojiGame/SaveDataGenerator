
namespace SaveDataManagerSystem.Model
{
    /// <summary>
    /// データのセーブを行うサービス
    /// </summary>
    public interface SDMS_IDataSaver
    {
        /// <summary>
        /// <paramref name="filePath"/>に <paramref name="jsonData"/> のセーブを行う
        /// </summary>
        /// <param name="filePath">保存先ファイルパス</param>
        /// <param name="jsonData">実際の保存対象のjson</param>
        public void Save(string filePath, string jsonData);
    }
}