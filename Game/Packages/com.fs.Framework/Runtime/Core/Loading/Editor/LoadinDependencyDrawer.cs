using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR

namespace UnityEditor.Inspector
{
    using Common.Core.Loading;
    //TODO: зробити UI редактування команд

    /*
    //[CustomPropertyDrawer(typeof(LoadingManager.Dependency))]
    public class LoadinDependencyDrawer : PropertyDrawer
    {
        private float LINE_HEIGHT = EditorGUIUtility.singleLineHeight;
        private const float SPACING = 4;
        private readonly GUIStyle m_St = new GUIStyle(EditorStyles.label);

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty valueItems = property.FindPropertyRelative("m_CommandsIndex");
            string path = property.propertyPath.Split('.')
                .Where((iter, idx) => idx < property.depth - 1)
                .Aggregate((current, next) => current + "." + next);

            SerializedProperty arrayItems = property.serializedObject.FindProperty(path);

            string[] names = BuildArray();

            float x = position.x;
            float y = position.y;
            string title = $"Dependency";
            m_St.normal.textColor = new Color(0.7f, 0.1f, 0.7f, 1);

            valueItems.isExpanded = EditorGUI.Foldout(new Rect(x, y, position.width - 6, LINE_HEIGHT), valueItems.isExpanded, title, true);

            var indentedRect = EditorGUI.IndentedRect(position);
            x = indentedRect.x;
            #region Draw array
            if (valueItems.isExpanded)
            {
                if (valueItems.arraySize > 0)
                    for (int i = 0; i < valueItems.arraySize; ++i)
                    {
                        SerializedProperty valueItem = valueItems.GetArrayElementAtIndex(i);
                        y += LINE_HEIGHT + SPACING;

                        int currentTypeIndex = valueItem.intValue;
                    
                        int selectedTypeIndex = EditorGUI.Popup(new Rect(x, y, indentedRect.width, LINE_HEIGHT + SPACING), currentTypeIndex, names);

                        if (selectedTypeIndex >= 0 && selectedTypeIndex < names.Length)
                        {
                            valueItem.intValue = selectedTypeIndex;
                        }
                    }
                EditorGUI.indentLevel++;
                indentedRect = EditorGUI.IndentedRect(position);
                x = indentedRect.x;
                y += LINE_HEIGHT + SPACING;
                if (GUI.Button(new Rect(x, y, 60, LINE_HEIGHT + (SPACING / 2)), "Add") && valueItems.arraySize < arrayItems.arraySize - 1)
                {
                    valueItems.arraySize++;
                    valueItems.isExpanded = true;
                }
                if (GUI.Button(new Rect((x + 60 + SPACING), y, 60, LINE_HEIGHT + (SPACING / 2)), "Del") && valueItems.arraySize > 0)
                {
                    valueItems.arraySize--;
                    valueItems.isExpanded = true;
                }
                y += LINE_HEIGHT + SPACING;
                EditorGUI.indentLevel--;
            }
            #endregion
            string[] BuildArray()
            {
                List<string> list = new List<string>();
                //arrayItems.arr
                for (int i = 0; i < arrayItems.arraySize; i++)
                {
                    string name = arrayItems.GetArrayElementAtIndex(i)
                        .FindPropertyRelative(nameof(LoadingManager.CommandItem.Command)).managedReferenceFullTypename;
                    //name = name.Remove(0, name.IndexOf(" "));
                    list.Add(name);
                }
                return list.ToArray();
            }

        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float lineAndSpace = LINE_HEIGHT + SPACING;
            SerializedProperty valueItems = property.FindPropertyRelative("m_CommandsIndex");

            return !valueItems.isExpanded 
                ? lineAndSpace 
                : (valueItems.arraySize + 2) * lineAndSpace;
        }

    }
    */
}
#endif