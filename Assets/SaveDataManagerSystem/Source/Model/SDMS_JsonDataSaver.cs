using System.IO;
using UnityEngine;

namespace SaveDataManagerSystem.Model
{
    /// <inheritdoc cref="SDMS_IDataSaver"/>
    public sealed class SDMS_JsonDataSaver : MonoBehaviour, SDMS_IDataSaver
    {
        /// <inheritdoc cref=" SDMS_IDataSaver.Save"/>
        public void Save(string filePath, string jsonData)
        {
            // シリアル化してファイル保存
            File.WriteAllText(filePath, jsonData);
        }
    }
}