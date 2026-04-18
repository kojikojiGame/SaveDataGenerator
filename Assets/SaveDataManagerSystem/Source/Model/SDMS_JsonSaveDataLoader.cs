using System.IO;
using UnityEngine;
using Newtonsoft.Json.Linq;

namespace SaveDataManagerSystem.Model
{

    /// <inheritdoc cref="SDMS_ISaveDataLoader"/>
    /// <remarks> 
    /// JSON 形式に特化したクラス。
    /// 他の形式はサポートしていない。
    /// </remarks>
    public sealed class SDMS_JsonSaveDataLoader : MonoBehaviour, SDMS_ISaveDataLoader
    {
        /// <inheritdoc cref="SDMS_ISaveDataLoader.FilePath"/>
        public string FilePath { get; set; }

        /// <inheritdoc cref="SDMS_ISaveDataLoader.JsonRawData"/>
        public string JsonRawData { get; private set; }

        /// <inheritdoc cref="SDMS_ISaveDataLoader.JsonObjectData"/>
        public JObject JsonObjectData { get; private set; }

        /// <inheritdoc cref="SDMS_ISaveDataLoader.Load"/>
        public bool Load(ref object saveData) => LoadCore(FilePath, ref saveData);

        /// <inheritdoc cref="SDMS_ISaveDataLoader.LoadFromFilePath(string)"/>
        public bool LoadFromFilePath(string filePath, ref object saveData) => LoadCore(filePath, ref saveData);

        /// <summary>
        /// ロードの内部実装
        /// </summary>
        /// <returns>ロード成功かどうか</returns>
        private bool LoadCore(string filePath, ref object saveData)
        {
            // ファイルが存在するかどうか
            if (!IsFileExists(filePath))
            {
                Debug.LogError($" Not Found Json File. File Path : {filePath}");
                return false;
            }

            // JSONデータとしてデータを読み込む
            JsonRawData = File.ReadAllText(filePath);

            JsonObjectData = JObject.Parse(JsonRawData);
            saveData = JsonObjectData;

            return true;
        }

        /// <summary>
        /// 引数に指定したファイルパスにファイルが存在するかどうか
        /// </summary>
        /// <param name="filePath">対象のファイルパス</param>
        /// <returns> 存在しているかどうか </returns>
        private bool IsFileExists(string filePath)
            => File.Exists(filePath);


    }
} 
