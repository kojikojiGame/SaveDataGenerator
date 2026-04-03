using UnityEngine;
using SaveDataManagerSystem.ViewModel;

namespace SaveDataManagerSystem.View
{
    public class JsonTreeView : MonoBehaviour
    {
        /// <summary>
        /// 保存先ファイルパス
        /// </summary>
        [SerializeField]
        public string _filePath = string.Empty;

        /// <summary>
        /// 実際に編集されるjsonデータ
        /// </summary>
        [TextArea(5, 10)]
        public string jsonInput = "{ \"player\": { \"info\": { \"name\": \"Hero\", \"lv\": 10 }, \"items\": [\"Sword\", \"Shield\"] } }";

        /// <summary>
        /// バインディング先のデータコンテキスト
        /// </summary>
        [SerializeField]
        private JsonTreeViewModel viewModelDataContext;

        /// <summary>
        /// リロードのコールバック
        /// </summary>
        public void OnReload()
        {
            viewModelDataContext.OnReload();
        }

        /// <summary>
        /// セーブのコールバック
        /// </summary>
        public void OnSave()
        {
            viewModelDataContext.OnSave(jsonInput);
        }
    }
}
