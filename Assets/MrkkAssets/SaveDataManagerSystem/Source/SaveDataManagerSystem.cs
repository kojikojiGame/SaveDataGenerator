using Newtonsoft.Json;
using SaveDataManagerSystem.View;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SaveDataManagerSystem
{

    /// <summary>
    /// セーブデータの管理を行うクラス
    /// </summary>
    /// <remarks>
    /// 本クラスでは、static　のクラスとして、ユーザーがソースコード中から参照できる機能を提供します。
    /// また、動的に扱いい場合は別途サービスクラスを作成して提供します。
    /// </remarks>
    public static class SaveDataUtility
    {
        /// <summary>
        /// セーブ予定のデータ
        /// </summary>
        private static object _saveData { get; set; }

        /// <summary>
        /// TreeViewで指定しているファイルパス
        /// </summary>
        public static string FilePath { get => JsonTreeView.FilePath; }

        /// <summary>
        /// セーブデータを取得する
        /// </summary>
        /// <typeparam name="T">セーブデータのクラスタイプ</typeparam>
        /// <returns>セーブデータ　を返却。失敗した場合 null を返却</returns>
        public static T? TryGetSaveData<T>() where T : class
        {
            if (_saveData is T data)
            {
                return data;
            }

            Debug.LogError("Failed Cast Type. At : SaveDataManagerSystem.SaveDataUtility.TryGetSaveData()");

            return null;
        }

        /// <summary>
        /// セーブデータを保存する
        /// </summary>
        /// <param name="saveData">保存対象のデータ</param>
        /// <typeparam name="T">セーブデータのクラスタイプ</typeparam>
        /// <returns>実行の成功失敗を返却</returns>
        public static bool TrySave<T>(object saveData) where T : class
        {
            if (saveData is T data)
            {
                _saveData = data;
                Save();
                return true;
            }

            Debug.LogError("Failed Cast Type. At : SaveDataManagerSystem.SaveDataUtility.TrySave()");

            return false;
        }

        /// <summary>
        /// 書き込んだデータをセーブ(<see cref="SaveDataUtility.SaveData"/> からセーブ処理を行う)
        /// </summary>
        public static void Save() 
        {
            SaveInternal(JsonConvert.SerializeObject(_saveData));
        }

        /// <summary>
        /// 書き込んだデータをセーブ(セーブ予定のデータからセーブ処理を行う)
        /// </summary>
        
        public static void Load<T>()
        {
            LoadInternal<T>(File.ReadAllText(FilePath));
        }

        private static void LoadInternal<T>(string jsonData)
        {
            _saveData = JsonConvert.DeserializeObject<T>(jsonData);
        }

        /// <summary>
        /// 書き込んだデータをセーブ
        /// </summary>
        /// <param name="jsonData">保存するjsonのデータ</param>
        public static void Save(string jsonData)
        {
            SaveInternal(jsonData);
        }

        private static void SaveInternal(string jsonData)
        {
            // シリアル化してファイル保存
            File.WriteAllText(FilePath, jsonData);
        }
    }
}