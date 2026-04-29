using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class DeprecatedTreeSettingsDialog : EditorWindow
{
    static string s_bindingDataPath;
    static string[] s_insertOptions = { "選択肢1", "選択肢2", "選択肢3" };
    static Action<string,int> s_deprecateAction;
    int _insetIndex = 0;
    int _lastInsetIndex = 0;

    ScriptableObject _recordMaster;

    Dictionary<string, string> _loadedDict;

    // --- JSON保存用の構造体 ---
    [Serializable]
    public class SerializationData
    {
        public List<string> keys = new List<string>();
        public List<string> values = new List<string>();
    }

    // メニューからウィンドウを開く
    [MenuItem("Window/My Custom Dialog")]
    public static void ShowWindow(string[] options, Action<string,int> deprecateAction,string bindingDataPath)
    {
        s_insertOptions = options;
        s_deprecateAction = deprecateAction;
        s_bindingDataPath = bindingDataPath;
        GetWindow<DeprecatedTreeSettingsDialog>("設定ダイアログ");
    }

    // ウィンドウが開かれたときに読み込む
    void OnEnable()
    {
        LoadAndApplySettings();
    }

    void OnGUI()
    {
        //GUILayout.Label("設定を選んでください", EditorStyles.boldLabel);

        // ドロップダウン（Popup）を表示
        _insetIndex = EditorGUILayout.Popup("Insert At", _insetIndex, s_insertOptions);

        // ObjectField が入力ボックスの実体です
        _recordMaster = EditorGUILayout.ObjectField(
            "Insert Source",                 // ラベル名
            _recordMaster,                   // 現在の値
            typeof(ScriptableObject),        // 許可する型（特定の型に制限すると便利）
            false                            // シーン上のオブジェクトを許可するか（SOなら通常false）
        ) as ScriptableObject;

        // 値を設定する
        if(_insetIndex != _lastInsetIndex)
        {
            if (_recordMaster is null)
            {
                // UIの各変数に反映
                if (_loadedDict.TryGetValue(s_insertOptions[_insetIndex], out string option))
                {
                    // パスからScriptableObjectをロード
                    _recordMaster = AssetDatabase.LoadAssetAtPath<ScriptableObject>(option);
                }
            }
            else
            {
                _recordMaster = null;
            }
        }


        if (GUILayout.Button("決定"))
        {

            if(_loadedDict.ContainsKey(s_insertOptions[_insetIndex]))
            {
                _loadedDict[s_insertOptions[_insetIndex]] = AssetDatabase.GetAssetPath(_recordMaster);
            }
            else
            {                
                _loadedDict.Add(s_insertOptions[_insetIndex], AssetDatabase.GetAssetPath(_recordMaster));
            }

            // 2. 保存実行
            SaveDictionary(_loadedDict);

            if (_recordMaster is IMasterData recordMaster)
            {
                s_deprecateAction.Invoke(s_insertOptions[_insetIndex], recordMaster.GetDataCount());
                Debug.Log("選択されたのは: " + s_insertOptions[_insetIndex]);
            }
            this.Close(); // ウィンドウを閉じる
        }

        _lastInsetIndex = _insetIndex;
    }

    private void LoadAndApplySettings()
    {
        if (string.IsNullOrEmpty(s_bindingDataPath) || !File.Exists(s_bindingDataPath))
        {
            Debug.LogError($"バインディングされているMasterDataのファイルパスが不正です : {s_bindingDataPath}");
            return;
        }

        try
        {
            // 1. JSONを文字列として読み込み、List形式に変換
            string json = File.ReadAllText(s_bindingDataPath);
            SerializationData data = JsonUtility.FromJson<SerializationData>(json);

            // 2. Dictionaryを再構築
            _loadedDict = new Dictionary<string, string>();
            for (int i = 0; i < data.keys.Count; i++)
            {
                _loadedDict[data.keys[i]] = data.values[i];
            }

        }
        catch (Exception e)
        {
            Debug.LogError($"設定の読み込みに失敗しました: {e.Message}");
        }
    }

    private void SaveDictionary(Dictionary<string, string> targetDict)
    {
        if (string.IsNullOrEmpty(s_bindingDataPath))
        {
            Debug.LogError($"バインディングされているMasterDataのファイルパスが不正です : {s_bindingDataPath}");
        }

        // DictionaryをList形式に変換
        var serialData = new SerializationData();
        foreach (var kvp in targetDict)
        {
            serialData.keys.Add(kvp.Key);
            serialData.values.Add(kvp.Value);
        }
        string json = JsonUtility.ToJson(serialData, true);
        File.WriteAllText(s_bindingDataPath, json);
        Debug.Log($"Dictionaryを保存しました: {s_bindingDataPath}");
    }
}
