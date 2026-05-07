using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SaveDataManagerSystem.View
{
    /// <summary>
    /// Json から TreeView を構築するクラス
    /// </summary>
    /// <remarks>
    /// AI に機能の確認を行いながら作成したので、注意が必要
    /// </remarks>
    [CustomEditor(typeof(JsonTreeView))]
    public class JsonTreeViewEditor : Editor
    {
        private VisualElement _treeContainer;
        private JToken _rootJToken;
        private int _rowCount = 0;
        private const float IndentSize = 18f;
        private const float KeyWidth = 120f;
        private const float TypeWidth = 100f; // 型表示の幅を少し広めに

        private JsonTreeView jsonTreeView => (JsonTreeView)target;

        // JTokenType の中から主要なものをドロップダウンに表示
        private readonly List<JTokenType> _displayTypes = new List<JTokenType> {
        JTokenType.String, JTokenType.Integer, JTokenType.Float,
        JTokenType.Boolean, JTokenType.Object, JTokenType.Array,
        JTokenType.Null };

        /// <summary>
        /// GUI を構築する
        /// </summary>
        /// <returns></returns>
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            InspectorElement.FillDefaultInspector(root, serializedObject, this);

            // コンテナを作成して参照を保持
            _treeContainer = new VisualElement { name = "JsonTree" };

            root.Add(new Button(jsonTreeView.OnReload) { text = "Reload", style = { height = 30, marginTop = 10 } });
            root.Add(new Button(RefreshTree) { text = "Edit", style = { height = 30, marginTop = 10 } });
            root.Add(_treeContainer);
            root.Add(new Button(jsonTreeView.OnSave) { text = "Save", style = { height = 30, marginTop = 10 } });
            root.Add(new Button(() => OnDeprecate()) { text = "Deprecate", style = { height = 30, marginTop = 10 } });
            return root;
        }

        private void OnDeprecate()
        {
            if (_rootJToken is null)
            {
                RefreshTree();
            }

            if (_rootJToken is JObject jobject)
            {
                // すべてのキーをリストで取得
                var keys = jobject.Descendants()
                    .Where(t => (t.Type == JTokenType.Object || t.Type == JTokenType.Array)
                    && t.Parent is not JArray)
                    .Select(x => x.Path).ToArray();

                DeprecatedTreeSettingsDialog.ShowWindow(keys, Deprecate, jsonTreeView.BindingMasterDataFilePath);
            }
        }

        public void Deprecate(string path, int number)
        {
            if(number == 0)
            {
                Debug.LogError($"要素数が{number}です。");
            }


            if (_rootJToken is JObject jobject)
            {
                var property = jobject.Descendants()
                    .OfType<JProperty>()
                    .FirstOrDefault(t => t.Path == path);

                if (property != null)
                {
                    // JProperty の中身（Value）が配列 (JArray) か確認
                    if (property.Value is JArray jArray)
                    {
                        // 最初の要素をコピー元にする
                        var firstItem = jArray.FirstOrDefault();
                        if (firstItem != null)
                        {
                            if (jArray.Count < number)
                            {
                                for (int i = jArray.Count; i < number; i++)
                                {
                                    // 参照ではなく複製(DeepClone)を追加しないと同じ実体を指してしまう
                                    jArray.Add(firstItem.DeepClone());
                                }
                            }
                            else
                            {
                                var dist = jArray.Count - number;

                                for (int i =  0; i < dist; i++)
                                {
                                    jArray.Last.Remove();
                                }
                            }
                        }
                    }

                    UpdateOriginalJson();
                    RefreshTree();
                }
                else
                {
                    Debug.LogError($"{path}が見つかりません。");
                }
            }

        }

        void RefreshTree()
        {
            var script = jsonTreeView;
            if (string.IsNullOrEmpty(script.jsonInput)) return;
            _treeContainer.Clear();
            _rowCount = 0;
            try
            {
                _rootJToken = JToken.Parse(script.jsonInput);
                RenderNode(_treeContainer, _rootJToken, "Root", 0);
            }
            catch (System.Exception e) { Debug.LogError("JSON Parse Error: " + e.Message); }
        }

        void RenderNode(VisualElement container, JToken token, string name, int depth)
        {
            var row = new VisualElement
            {
                style = {
                flexDirection = FlexDirection.Row, paddingLeft = depth * IndentSize, height = 26, alignItems = Align.Center,
                backgroundColor = (_rowCount++ % 2 == 0) ? new Color(0.25f, 0.25f, 0.25f, 0.3f) : new Color(0.2f, 0.2f, 0.2f, 0.3f)
            }
            };
            container.Add(row);

            // 1. キー
            row.Add(CreateKeyElement(token, name));

            // 2. 型選択 (JTokenType)
            var typePopup = new PopupField<JTokenType>(_displayTypes, token.Type)
            {
                style = { width = TypeWidth, fontSize = 10, marginRight = 5 }
            };
            typePopup.RegisterValueChangedCallback(evt =>
            {
                ChangeTokenType(token, evt.newValue);
                UpdateOriginalJson();
                RefreshTree();
            });
            row.Add(typePopup);

            // 3. 値
            if (token is JObject obj)
            {
                row.Add(new Label("{ Object }") { style = { flexGrow = 1, color = new Color(0.8f, 0.5f, 0.9f) } });
                AddControlButtons(row, token, obj: obj);
                foreach (var p in obj.Properties().ToList()) RenderNode(container, p.Value, p.Name, depth + 1);
            }
            else if (token is JArray arr)
            {
                row.Add(new Label($"[ Array({arr.Count}) ]") { style = { flexGrow = 1, color = new Color(0.4f, 0.7f, 1f) } });
                AddControlButtons(row, token, arr: arr);
                for (int i = 0; i < arr.Count; i++) RenderNode(container, arr[i], $"[{i}]", depth + 1);
            }
            else
            {
                if (token.Type == JTokenType.Null)
                {
                    row.Add(new Label("null") { style = { flexGrow = 1, opacity = 0.5f } });
                }
                else
                {
                    var valueField = new TextField { value = token.ToString(), style = { flexGrow = 1, marginRight = 5 } };
                    valueField.RegisterCallback<FocusOutEvent>(evt =>
                    {
                        if (TryUpdateValue(token, valueField.value))
                            UpdateOriginalJson();
                    });
                    row.Add(valueField);
                }
                AddControlButtons(row, token);
            }
        }

        // 指定された JTokenType に基づいてデフォルト値を生成し置換
        void ChangeTokenType(JToken token, JTokenType newType)
        {
            JToken newValue = newType switch
            {
                JTokenType.Integer => new JValue(0),
                JTokenType.Float => new JValue(0.0),
                JTokenType.Boolean => new JValue(false),
                JTokenType.Object => new JObject(),
                JTokenType.Array => new JArray(),
                JTokenType.Null => JValue.CreateNull(),
                _ => new JValue("")
            };
            token.Replace(newValue);
        }

        VisualElement CreateKeyElement(JToken token, string name)
        {
            // 配列のインデックス（[0]など）は編集不可、JPropertyのキーのみ編集可能にする
            bool isProperty = token.Parent is JProperty;
            var keyField = new TextField { value = name, style = { width = KeyWidth, marginRight = 5 } };
            keyField.SetEnabled(isProperty);

            if (isProperty)
            {
                keyField.RegisterCallback<FocusOutEvent>(evt =>
                {
                    string newName = keyField.value;
                    if (name == newName) return;

                    var property = (JProperty)token.Parent;
                    var parentObj = property.Parent as JObject;

                    if (parentObj != null)
                    {
                        // 1. まず自分自身の名前を変更
                        RenameProperty(property, newName);

                        // 2. 「配列の0番目の要素」のプロパティが変更されたかチェック
                        if (parentObj.Parent is JArray jArray && jArray.First == parentObj)
                        {
                            // 他のすべての要素（1番目以降）に対しても同じ名前変更を適用
                            foreach (var item in jArray.Skip(1).OfType<JObject>())
                            {
                                var targetProp = item.Property(name); // 旧名のプロパティを探す
                                if (targetProp != null)
                                {
                                    RenameProperty(targetProp, newName);
                                }
                            }
                        }

                        UpdateOriginalJson();
                        RefreshTree(); // 構造が変わるためリフレッシュ
                    }
                });
            }
            return keyField;
        }

        bool TryUpdateValue(JToken token, string val)
        {
            // JValue（値を持つトークン）でない場合は更新不可
            if (token is not JValue jValue) return false;

            try
            {
                switch (token.Type)
                {
                    case JTokenType.Integer:
                        jValue.Value = long.Parse(val);
                        break;
                    case JTokenType.Float:
                        jValue.Value = double.Parse(val);
                        break;
                    case JTokenType.Boolean:
                        jValue.Value = bool.Parse(val);
                        break;
                    default:
                        jValue.Value = val; // String等
                        break;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        void RenameProperty(JProperty property, string newName)
        {
            var parent = property.Parent as JObject;
            if (parent == null) return;

            // Newtonsoft.Json では Property の Name は直接書き換えられないため、
            // 新しい名前で作り直して置き換える必要がある
            var newProp = new JProperty(newName, property.Value);
            property.Replace(newProp);
        }

        void AddControlButtons(VisualElement row, JToken token, JObject obj = null, JArray arr = null)
        {
            if (obj != null || arr != null)
                row.Add(
                    new Button(
                    () =>
                    {
                        if (obj != null)
                        {
                            // オブジェクトへの追加（前回の回答通り）
                            string baseName = (token is JProperty p) ? p.Name : "NewKey";
                            AddChild(token, MakeUniqueName(obj, baseName));
                        }
                        else if (arr != null)
                        {
                            // --- 修正箇所：配列への追加 ---
                            if (arr.Count > 0)
                            {
                                // 0番目の要素が存在すれば、それを複製して追加
                                arr.Add(arr[0].DeepClone());
                            }
                            else
                            {
                                // 配列が空ならデフォルトとして空文字（または適切な初期値）を追加
                                arr.Add("");
                            }
                            // ----------------------------
                        }
                        UpdateOriginalJson();
                        RefreshTree();
                    })
                    {
                        text = "+",
                        style = { width = 25 }
                    }
                );

            if (token != _rootJToken)
                row.Add(new Button(
                    () =>
                    {
                        if (token.Parent is JProperty p)
                        {
                            // 1. まず、削除対象の親が「配列の0番目の要素（JObject）」かどうかを確認
                            // path の例: Root.MyArray[0].MyProperty
                            var parentObject = p.Parent as JObject;
                            var grandParentArray = parentObject?.Parent as JArray;

                            // 0番目の要素内のプロパティが削除されようとしている場合
                            if (grandParentArray != null && grandParentArray.IndexOf(parentObject) == 0)
                            {
                                string propertyName = p.Name;

                                // 要素1以降のすべてのJObjectから、同じ名前のプロパティを削除
                                for (int i = 1; i < grandParentArray.Count; i++)
                                {
                                    if (grandParentArray[i] is JObject targetObj)
                                    {
                                        targetObj.Property(propertyName)?.Remove();
                                    }
                                }
                            }

                            // 2. 本人（0番目のプロパティ）を削除
                            p.Remove();
                        }
                        else
                        {
                            // 通常の削除
                            token.Remove();
                        }

                        UpdateOriginalJson();
                        RefreshTree();
                    })
                {
                    text = "×",
                    style = { width = 25, color = Color.red }
                });


        }

        private void AddChild(JToken token, string keyName)
        {
            if (token is JObject obj)
            {
                // 1. 新しいプロパティ（要素）を作成
                var newPropName = keyName;
                var newValue = new JValue("");
                obj.Add(newPropName, newValue);

                // --- 同期ロジック開始 ---
                // このオブジェクトが「JArrayの0番目の要素」であるか確認
                if (obj.Parent is JArray parentArray && parentArray.First == obj)
                {
                    // 1番目以降のすべてのオブジェクトに対して、同じプロパティを追加
                    for (int i = 1; i < parentArray.Count; i++)
                    {
                        if (parentArray[i] is JObject targetObj)
                        {
                            // 既に同名のキーがない場合のみ追加（DeepCloneで実体を分ける）
                            if (targetObj.Property(newPropName) == null)
                            {
                                targetObj.Add(newPropName, newValue.DeepClone());
                            }
                        }
                    }
                }
                // --- 同期ロジック終了 ---

                UpdateOriginalJson();
                RefreshTree();
            }
        }
        private string MakeUniqueName(JObject obj, string candidate, int? count = null)
        {
            string suffix = count == null ? "" : count.ToString();
            string candidateName = $"{candidate}{suffix}";

            if (obj.ContainsKey(candidateName))
            {
                // 次の数値を試す
                return MakeUniqueName(obj, candidate, (count ?? 0) + 1);
            }

            return candidateName;
        }
        void UpdateOriginalJson()
        {
            serializedObject.Update();
            serializedObject.FindProperty("jsonInput").stringValue = _rootJToken.ToString(Newtonsoft.Json.Formatting.Indented);
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        }
    }
}