using SaveDataManagerSystem.View;
using System;
using UnityEditor;
using UnityEngine;

public class DeprecatedTreeSettingsDialog : EditorWindow
{
    static string[] s_insertOptions = { "選択肢1", "選択肢2", "選択肢3" };
    static Action<string,int> s_deprecateAction;
    int _insetIndex = 0;
    ScriptableObject _recordMaster;
    //static JsonTreeViewEditor s_jsonTreeViewEditor;

    // メニューからウィンドウを開く
    [MenuItem("Window/My Custom Dialog")]
    public static void ShowWindow(string[] options, Action<string,int> _DeprecateAction)
    {
        s_insertOptions = options;
        s_deprecateAction = _DeprecateAction;
        GetWindow<DeprecatedTreeSettingsDialog>("設定ダイアログ");
    }

    void OnGUI()
    {
        GUILayout.Label("設定を選んでください", EditorStyles.boldLabel);

        // ドロップダウン（Popup）を表示
        _insetIndex = EditorGUILayout.Popup("Insert At", _insetIndex, s_insertOptions);

        // ObjectField が入力ボックスの実体です
        _recordMaster = EditorGUILayout.ObjectField(
            "Insert Source",                 // ラベル名
            _recordMaster,              // 現在の値
            typeof(ScriptableObject),       // 許可する型（特定の型に制限すると便利）
            false                           // シーン上のオブジェクトを許可するか（SOなら通常false）
        ) as ScriptableObject;


        if (GUILayout.Button("決定"))
        {
            if (_recordMaster is IMasterData recordMaster)
            {
                s_deprecateAction.Invoke(s_insertOptions[_insetIndex], recordMaster.GetDataCount());
                Debug.Log("選択されたのは: " + s_insertOptions[_insetIndex]);
            }
            this.Close(); // ウィンドウを閉じる
        }
    }
}
