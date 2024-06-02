using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using Common.Core;
using Common.Defs;

using UnityEditor.Experimental.GraphView;
using UnityEditor.IMGUI.Controls;

namespace UnityEditor.Inspector
{
 
    public static class Ext
    {
        public static IEnumerable<T> Flatten<T>(this IEnumerable<T> e, Func<T, IEnumerable<T>> f) => e.SelectMany(c => f(c).Flatten(f)).Concat(e);

        public static IEnumerable<T> Traverse<T>(this T item, Func<T, T> childSelector)
        {
            var stack = new Stack<T>(new T[] { item });

            while (stack.Any())
            {
                var next = stack.Pop();
                if (next != null)
                {
                    yield return next;
                    stack.Push(childSelector(next));
                }
            }
        }
    }

    [CustomPropertyDrawer(typeof(SelectChildPrefabAttribute))]
    public class SelectChildPrefabDrawer : PropertyDrawer
    {
        public async override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SelectChildPrefabAttribute attr = (SelectChildPrefabAttribute)attribute;
            if (property.serializedObject.targetObject is not IViewPrefab componentPrefab) return;

            var prefab = await componentPrefab.GetViewPrefab();
            if (prefab == null)
            {
                EditorGUI.PropertyField(position, property, label);//typeof(GameObject)
                return;
            }

            var buttonRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            buttonRect = EditorGUI.PrefixLabel(buttonRect, label);
            var value = property.objectReferenceValue as GameObject;
            var display = value == null ? "Empty" : GetName(value, prefab);

            if (EditorGUI.DropdownButton(buttonRect, new GUIContent(display), FocusType.Passive, EditorStyles.objectField))
            {
                var pos = new Vector2(position.center.x, position.y + (EditorGUIUtility.singleLineHeight * 2));
                SimpleTreeViewWindow.ShowWindow(new SearchWindowContext(GUIUtility.GUIToScreenPoint(pos), position.width), prefab,
                    (value) =>
                    {
                        property.objectReferenceValue = value;
                        property.serializedObject.ApplyModifiedProperties();
                    });
            }
        }

        string GetName(GameObject value, GameObject owner)
        {
            string path = value.name;
            value = value.transform.parent?.gameObject;
            while (value != null) 
            {
                path = value.name + "." + path;
                value = value.transform.parent?.gameObject;
            }

            return path;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, false);
        }

        class SimpleTreeViewWindow : EditorWindow
        {

            private static SimpleTreeViewWindow s_Window;
            private class Styles
            {
                public GUIStyle header = "AC BoldHeader";

                public GUIStyle componentButton = "AC ComponentButton";

                public GUIStyle groupButton = "AC GroupButton";

                public GUIStyle background = "grey_border";

                public GUIStyle rightArrow = "ArrowNavigationRight";

                public GUIStyle leftArrow = "ArrowNavigationLeft";
            }
            private static Styles s_Styles;

            [SerializeField] TreeViewState m_TreeViewState;
            SimpleTreeView m_TreeView;
            SearchField m_SearchField;
            Action<GameObject> m_OnSelected;

            void OnEnable()
            {
                if (m_TreeViewState == null)
                    m_TreeViewState = new TreeViewState();

                m_TreeView = new SimpleTreeView(m_TreeViewState);
                m_SearchField = new SearchField();
                m_SearchField.downOrUpArrowKeyPressed += m_TreeView.SetFocusAndEnsureSelectedItem;
            }

            void OnGUI()
            {
                if (s_Styles == null)
                {
                    s_Styles = new Styles();
                }
                Rect screenRect = base.position;
                screenRect.x = base.position.width * (1f - 0) + 1f;
                screenRect.y = 30f;
                screenRect.height -= 30f;
                screenRect.width -= 2f;

                DoToolbar();
                Rect rect = new Rect(0f, 0f, base.position.width, base.position.height);
                GUI.Label(rect, GUIContent.none, s_Styles.background);
                GUILayout.BeginArea(screenRect);
                GUILayout.EndArea();
                rect.x = 4;
                rect.y = 34;
                rect.width -= rect.x * 2;
                rect.height -= rect.y * 2;
                DoTreeView(rect);
                HandleKeyboard();
            }

            private void HandleKeyboard()
            {
                Event current = Event.current;
                if (current.type != EventType.KeyDown)
                {
                    return;
                }
                if (current.keyCode == KeyCode.Escape)
                {
                    Close();
                    current.Use();
                }
            }

            // Search box 
            void DoToolbar()
            {
                GUILayout.BeginHorizontal(EditorStyles.toolbar);
                GUILayout.Space(100);
                GUILayout.FlexibleSpace();
                m_TreeView.searchString = m_SearchField.OnToolbarGUI(m_TreeView.searchString);
                GUILayout.EndHorizontal();
            }

            // draw TreeView
            void DoTreeView(Rect rect)
            {
                m_TreeView.OnGUI(rect);
            }
            public static void ShowWindow(SearchWindowContext context, GameObject value, Action<GameObject> onSelected)
            {
                UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(typeof(SimpleTreeViewWindow));
                if (array.Length != 0)
                {
                    try
                    {
                        ((EditorWindow)array[0]).Close();
                        return;
                    }
                    catch (Exception)
                    {
                        s_Window = null;
                    }
                }

                if (s_Window == null)
                {
                    s_Window = ScriptableObject.CreateInstance<SimpleTreeViewWindow>();
                    s_Window.hideFlags = HideFlags.HideAndDontSave;
                }

                s_Window.m_TreeView = new SimpleTreeView(s_Window.m_TreeViewState);
                s_Window.m_TreeView.BuildTree(value);
                s_Window.m_TreeView.OnSelect += s_Window.SelectEntry;
                s_Window.m_OnSelected = onSelected;

                s_Window.wantsMouseMove = true;
                float num = Math.Max(context.requestedWidth, 240f);
                float y = Math.Max(context.requestedHeight, 320f);
                Rect buttonRect = new Rect(context.screenMousePosition.x - num / 2f, context.screenMousePosition.y - 16f, num, 1f);
                s_Window.ShowAsDropDown(buttonRect, new Vector2(buttonRect.width, y));
                s_Window.Focus();
            }

            private void SelectEntry(GameObject value)
            {
                m_OnSelected.Invoke(value);
                Close();
            }

        }

        class SimpleTreeView : TreeView
        {
            GameObject m_GameObject;
            public event Action<GameObject> OnSelect;
            public void BuildTree(GameObject value)
            {
                m_GameObject = value;
                Reload();
                ExpandAll();
            }

            protected override void DoubleClickedItem(int id)
            {
                var item = (TreeViewItemData)FindItem(id, rootItem);
                if (item != null)
                    OnSelect?.Invoke(item?.Data);
            }

            public SimpleTreeView(TreeViewState treeViewState): base(treeViewState) { }

            private Texture2D GetIcon(GameObject value)
            {
                return AssetPreview.GetMiniThumbnail(value);
            }

            private class TreeViewItemData : TreeViewItem
            {
                public GameObject Data { get; set; }
            }

            protected override TreeViewItem BuildRoot()
            {
                var root = new TreeViewItemData { id = 0, depth = -1, displayName = "Root" };
                root.AddChild(new TreeViewItemData { id = -1, depth = 0, displayName = "(null)", Data = null, });
                var owner = new TreeViewItemData 
                { 
                    id = m_GameObject.GetInstanceID(), 
                    displayName = m_GameObject.name, 
                    icon = GetIcon(m_GameObject),
                    Data = m_GameObject, 
                };

                root.AddChild(owner);
                BuildTree(owner, m_GameObject);

                void BuildTree(TreeViewItem root, GameObject value)
                {
                    var childs = value.transform
                        .Cast<Transform>()
                        .ToList();

                    foreach (var child in childs)
                    {
                        var node = new TreeViewItemData { id = child.GetInstanceID(), displayName = child.name, icon = GetIcon(child.gameObject), Data = child.gameObject, };
                        root.AddChild(node);
                        BuildTree(node, child.gameObject);
                    }
                }
                SetupDepthsFromParentsAndChildren(root);
                return root;
            }
        }

    }
}
