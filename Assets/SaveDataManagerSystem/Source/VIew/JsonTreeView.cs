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
        private string _filePath = string.Empty;

        public static string FilePath = string.Empty;
        
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
            jsonInput = viewModelDataContext.JsonInput;
        }

        /// <summary>
        /// セーブのコールバック
        /// </summary>
        public void OnSave()
        {
            viewModelDataContext.OnSave(jsonInput);
        }

        /// <summary>
        /// static プロパティの半影
        /// </summary>
        /// <remarks>
        /// Inspectorで値を変更した瞬間に反映（エディタ実行中も有効）
        /// </remarks>
        private void OnValidate()
        {
            
            FilePath = _filePath;
        }
    }
}
