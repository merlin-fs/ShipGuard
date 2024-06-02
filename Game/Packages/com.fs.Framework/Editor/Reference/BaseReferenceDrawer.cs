using System;
using System.IO;
using System.Reflection;
using System.Reflection.Ext;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace UnityEditor.Inspector
{
    abstract class BaseReferenceDrawer : PropertyDrawer
    {
        private static readonly float LINE_HEIGHT = EditorGUIUtility.singleLineHeight;
        private class TypeProvider : PickerProvider<Type> { }
        private static readonly TypeProvider m_TypeProvider = ScriptableObject.CreateInstance<TypeProvider>();
        private static Texture2D m_CaretTexture = null;
        GUIStyle m_Normal;
        protected bool m_CanPing = true;

        protected virtual Color GetColor()
        {
            return Color.green;
        }

        protected virtual void GetDisplayValue(object value, ref string display) { }

        protected abstract Type GetBaseType(SerializedProperty property);

        protected virtual void OnSelect(SerializedProperty property, Type type) { }

        protected virtual void FinalizeProperty(Rect position, SerializedProperty property, GUIContent label) { }

        protected virtual bool IsReadOnly(SerializedProperty property, Type type)
        {
            return false;
        }

        protected virtual Rect GetRect(Rect position, SerializedProperty property, GUIContent label)
        {
            return position;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            m_Normal = new GUIStyle(EditorStyles.objectField);
            var color = GetColor();
            m_Normal.hover.textColor = color;
            m_Normal.focused.textColor = color;

            Color.RGBToHSV(color, out float h, out float s, out float v);
            v -= 0.2f;
            color = Color.HSVToRGB(h, s, v);
            m_Normal.normal.textColor = color;

            var assetDropDownRect = GetRect(position, property, label);
            bool isDragging = Event.current.type == EventType.DragUpdated && assetDropDownRect.Contains(Event.current.mousePosition);
            bool isDropping = Event.current.type == EventType.DragPerform && assetDropDownRect.Contains(Event.current.mousePosition);

            var value = property.boxedValue;
            bool isEmpty = value == null;

            var type = GetBaseType(property);

            string display = value?.GetType().ToGenericTypeString() ?? "(null)";
            GetDisplayValue(value, ref display);
            var isReadOnly = IsReadOnly(property, type);

            if (Event.current.type == EventType.MouseDown)
            {
                var foldoutRect = new Rect(assetDropDownRect.x - 20, assetDropDownRect.y, 25, assetDropDownRect.height);

                bool isFoldoutPressed = Event.current.type == EventType.MouseDown && Event.current.button == 0 && foldoutRect.Contains(Event.current.mousePosition);
                if (isFoldoutPressed)
                    property.isExpanded = !property.isExpanded;
            }

            DrawControl(assetDropDownRect, isDragging, isDropping, display, isEmpty, isReadOnly, value?.GetType(),
                () =>
                {
                    if (type != null)
                    {
                        var types = TypeHelper.GetTypeList(type, false, true);
                        m_TypeProvider.Setup(type.Name, types.Paths, types.Types, GetIcon,
                            (type) =>
                            {
                                OnSelect(property, type);
                            });
                    }
                    var pos = new Vector2(assetDropDownRect.center.x, assetDropDownRect.y + (LINE_HEIGHT * 2));
                    SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(pos), assetDropDownRect.width), m_TypeProvider);
                });
            
            FinalizeProperty(position, property, label);

            property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.Update();
        }

        private void DrawControl(Rect assetDropDownRect, bool isDragging, bool isDropping, string nameToUse, bool isEmpty, bool isReadOnly, Type assetType, Action dropDown)
        {
            float pickerWidth = 20f;
            Rect pickerRect = assetDropDownRect;
            pickerRect.width = pickerWidth;
            pickerRect.x = assetDropDownRect.xMax - pickerWidth;

            bool isPickerPressed = Event.current.type == EventType.MouseDown && Event.current.button == 0 && (pickerRect.Contains(Event.current.mousePosition) || (isEmpty && assetDropDownRect.Contains(Event.current.mousePosition)));
            bool isEnterKeyPressed = Event.current.type == EventType.KeyDown && Event.current.isKey && (Event.current.keyCode == KeyCode.KeypadEnter || Event.current.keyCode == KeyCode.Return);
            if (isPickerPressed || isDragging || isDropping || isEnterKeyPressed)
            {
                // To override ObjectField's default behavior
                Event.current.Use();
            }

            if (!isEmpty)
            {
                GUI.Box(assetDropDownRect, new GUIContent(nameToUse), m_Normal);

                bool isFieldPressed = Event.current.type == EventType.MouseDown && Event.current.button == 0 && assetDropDownRect.Contains(Event.current.mousePosition);

                if (isFieldPressed)
                {
                    if (Event.current.clickCount == 1 && m_CanPing)
                    {
                        var path = GetMonoScriptPathFor(assetType);
                        var obj = AssetDatabase.LoadMainAssetAtPath(path);
                        EditorGUIUtility.PingObject(obj);
                        GUIUtility.ExitGUI();
                    }
                    if (Event.current.clickCount == 2 && m_CanPing)
                    {
                        var path = GetMonoScriptPathFor(assetType);
                        var obj = AssetDatabase.LoadMainAssetAtPath(path);
                        AssetDatabase.OpenAsset(obj);
                        GUIUtility.ExitGUI();
                    }
                }
            }
            else
            {
                GUI.Box(assetDropDownRect, new GUIContent(nameToUse), EditorStyles.objectField);
            }
            if (!isReadOnly)
            {
                DrawCaret(pickerRect);
                if (isPickerPressed)
                    dropDown?.Invoke();
            }
        }

        private void DrawCaret(Rect pickerRect)
        {
            if (m_CaretTexture == null)
            {
                var thisfile = $"{nameof(BaseReferenceDrawer)}.cs";
                string[] res = Directory.GetFiles(Application.dataPath, thisfile, SearchOption.AllDirectories);
                if (res.Length > 0)
                {
                    string caretIconPath = Path.GetDirectoryName(res[0]);
                    caretIconPath = Path.GetFullPath(Path.Combine(caretIconPath, "../Icon/PickerDropArrow-Pro.png"));
                    if (File.Exists(caretIconPath))
                    {
                        caretIconPath = "Assets\\" + Path.GetRelativePath(Application.dataPath, caretIconPath);
                        m_CaretTexture = (Texture2D)AssetDatabase.LoadAssetAtPath(caretIconPath, typeof(Texture2D));
                    }
                }
            }

            if (m_CaretTexture != null)
            {
                GUI.DrawTexture(pickerRect, m_CaretTexture, ScaleMode.ScaleToFit);
            }
        }

        private static string GetMonoScriptPathFor(Type type)
        {
            var asset = "";
            var guids = AssetDatabase.FindAssets(string.Format("{0} t:script", type.Name));
            if (guids.Length > 1)
            {
                foreach (var guid in guids)
                {
                    var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    var filename = Path.GetFileNameWithoutExtension(assetPath);
                    if (filename == type.Name)
                    {
                        asset = guid;
                        break;
                    }
                }
            }
            else if (guids.Length == 1)
            {
                asset = guids[0];
            }
            else
            {
                Debug.LogErrorFormat("Unable to locate {0}", type.Name);
                return null;
            }
            return AssetDatabase.GUIDToAssetPath(asset);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUI.GetPropertyHeight(property);
            return height;
        }

        private Texture GetIcon(Type type)
        {
            return AssetPreview.GetMiniTypeThumbnail(type);
        }
    }
}
