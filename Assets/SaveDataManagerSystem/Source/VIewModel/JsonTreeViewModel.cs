using SaveDataManagerSystem.View;
using SaveDataManagerSystem.Model;
using UnityEngine;

namespace SaveDataManagerSystem.ViewModel
{
    /// <summary>
    /// JsonTreeView のViewModel
    /// </summary>
    public class JsonTreeViewModel : MonoBehaviour
    {
        /// <summary>
        /// バインディング対象のView
        /// </summary>
        [SerializeField]
        private JsonTreeView jsonTreeView;

        /// <summary>
        /// ロードしたJsonデータ
        /// </summary>
        public string JsonInput => _dataLoader.JsonRawData;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private JsonTreeViewModel()
        {
            _dataLoader = new SDMS_JsonSaveDataLoader();
            _dataSaver = new SDMS_JsonDataSaver();
        }

        /// <summary>
        /// リロードのコールバック
        /// </summary>
        public void OnReload()
        {
            object jsonData = new();
            _dataLoader.LoadFromFilePath(JsonTreeView.FilePath, ref jsonData);
        }

        /// <summary>
        /// セーブのコールバック
        /// </summary>
        /// <param name="jsonData">保存対象のjsonのデータ</param>
        public void OnSave(string jsonData)
        {
            _dataSaver.Save(jsonData);
        }


        private readonly SDMS_ISaveDataLoader _dataLoader;
        private readonly SDMS_IDataSaver _dataSaver;
    }
}