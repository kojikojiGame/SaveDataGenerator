using System.IO;
using UnityEngine;

namespace SaveDataManagerSystem.Model
{
    /// <inheritdoc cref="SDMS_IDataSaver"/>
    public sealed class SDMS_JsonDataSaver : MonoBehaviour, SDMS_IDataSaver
    {
        /// <inheritdoc cref=" SDMS_IDataSaver.Save"/>
        public void Save(string jsonData)
        {
            SaveDataUtility.Save(jsonData);
        }
    }
}