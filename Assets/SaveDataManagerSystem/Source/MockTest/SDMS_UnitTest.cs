using Newtonsoft.Json;
using SaveDataManagerSystem;
using SaveDataManagerSystem.Model;
using UnityEngine;

/// <remarks>
/// コメントアウトをすることで、Gameobjectにアタッチ可能
/// </remarks>
//namespace SaveDataManagerSystem.MockTest


/// <summary>
/// 動作確認用の単体テストクラス
/// </summary>
public class SDMS_UnitTest : MonoBehaviour
{
    [SerializeField]
    private bool _isLoadTest = true;

    private Rootobject _saveData;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_isLoadTest)
        {
            LoadTest();
        }
    }

    private void Update()
    {
        if (_isLoadTest)
        {
            if (Input.GetKeyUp(KeyCode.L))
            {
                Debug.Log(JsonConvert.SerializeObject(_saveData));
            }
        }
    }



    #region Data Load
    /// <summary>
    /// ロードを行うクラスの動作確認
    /// </summary>
    public void LoadTest()
    {
        SaveDataUtility.Load<Rootobject>();
        
        if(SaveDataUtility.TryGetSaveData<Rootobject>() is Rootobject rootobject)
        {
            _saveData = rootobject;
        }
    }
    #endregion

}
