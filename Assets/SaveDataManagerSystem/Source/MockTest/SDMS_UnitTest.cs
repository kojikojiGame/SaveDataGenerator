using UnityEngine;
using SaveDataManagerSystem.Model;

/// <remarks>
/// コメントアウトをすることで、Gameobjectにアタッチ可能
/// </remarks>
namespace SaveDataManagerSystem.MockTest
{

    /// <summary>
    /// 動作確認用の単体テストクラス
    /// </summary>
    public class SDMS_UnitTest : MonoBehaviour
    {
        [SerializeField]
        private bool IsLoadTest = false;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            if (IsLoadTest)
            {
                _dataLoader = this.gameObject.AddComponent<SDMS_JsonSaveDataLoader>();
                LoadTest();
            }
        }


        #region Data Load
        /// <summary>
        /// 保存先ファイルパス
        /// </summary>
        [SerializeField]
        public string _filePath = string.Empty;

        private SDMS_ISaveDataLoader _dataLoader;

        /// <summary>
        /// ロードを行うクラスの動作確認
        /// </summary>
        public void LoadTest()
        {
            object data = new();
            if (_dataLoader.LoadFromFilePath(_filePath, ref data))
            {
                Debug.Log($"{data}");
            }
        }
        #endregion

    }
}