using Newtonsoft.Json;
using SaveDataManagerSystem;
using System.Collections.Generic;
using System.Linq;
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

            if (Input.GetKeyUp(KeyCode.S))
            {
                SaveTest();
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

    #region Data Save
    /// <summary>
    /// セーブを行うクラスの動作確認
    /// </summary>
    public void SaveTest()
    {
        AddSlotData();

        if (!SaveDataUtility.TrySave<Rootobject>(_saveData))
        {
            // 失敗した場合このScopeに入る
        }
    }

    /// <summary>
    /// SlotDataの追加テスト.
    /// ゲーム側で加工される想定
    /// </summary>
    private void AddSlotData()
    {
        var slotDataList = _saveData.GameData.SlotData.ToList();

        var newSlot = new Slotdata();        
        newSlot.SlotID = "-1";

        var playerData = new Playerdata();
        playerData.Turn = "-1";
        playerData.Money = "-1";
        newSlot.PlayerData = playerData;

        var heroineList = new List<Heroinedata>();

        var heroineData = new Heroinedata();
        heroineData.Name = "Hanako";
        heroineList.Add(heroineData);

        newSlot.HeroineData = heroineList.ToArray();

        slotDataList.Add(newSlot);
        _saveData.GameData.SlotData = slotDataList.ToArray();
    }

    #endregion

}
