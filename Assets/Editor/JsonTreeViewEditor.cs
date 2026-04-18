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
            return root;
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
                    valueField.RegisterValueChangedCallback(evt =>
                    {
                        if (TryUpdateValue(token, evt.newValue))
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

        // --- 以下、補助メソッド (以前と同様) ---
        VisualElement CreateKeyElement(JToken token, string name)
        {
            if (token.Parent is JProperty prop)
            {
                var f = new TextField { value = name, style = { width = KeyWidth, marginRight = 5 } };
                f.RegisterValueChangedCallback(evt => { RenameProperty(prop, evt.newValue); UpdateOriginalJson(); RefreshTree(); });
                return f;
            }
            return new Label(name) { style = { width = KeyWidth, marginRight = 5, unityFontStyleAndWeight = FontStyle.Bold } };
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
            if (string.IsNullOrEmpty(newName) || property.Name == newName) return;
            JObject parent = (JObject)property.Parent;
            if (parent == null || parent.ContainsKey(newName)) return;
            property.AddAfterSelf(new JProperty(newName, property.Value));
            property.Remove();
        }

        void AddControlButtons(VisualElement row, JToken token, JObject obj = null, JArray arr = null)
        {
            if (obj != null || arr != null)
                row.Add(
                    new Button(
                    () =>
                    {
                        if (obj != null)
                            obj.Add(MakeUniqueName(obj, "NewKey"), "");
                        else
                            arr.Add("");
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
                            p.Remove();
                        else
                            token.Remove();
                        UpdateOriginalJson();
                        RefreshTree();
                    })
                {
                    text = "×",
                    style = { width = 25, color = Color.red }
                });


        }

        private string MakeUniqueName(JObject obj, string candidate, int? count = null)
        {
            var isContainsKey = count == null
                ? obj.ContainsKey($"{candidate}")
                : obj.ContainsKey($"{candidate}{count}");

            var candidateName = count == null ? $"{candidate}" : $"{candidate}{count}";

            if (isContainsKey)
            {
                candidateName = MakeUniqueName(obj, candidateName, count++ ?? 0);
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