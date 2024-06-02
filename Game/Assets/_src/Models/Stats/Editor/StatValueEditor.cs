using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Common.Core;
using Common.Defs;
using Game.Model;
using Game.Model.Stats;

namespace UnityEditor.Inspector
{

    //[CustomPropertyDrawer(typeof(StatValue), true)]
    public class StatValueEdit : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var original = property.FindPropertyRelative("m_Original");
            var value = property.FindPropertyRelative("m_Value");
            var rect = EditorGUI.PrefixLabel(position, label);
            GUIContent[] contents = new GUIContent[] 
            {
                new GUIContent { text = "Min"},
                new GUIContent { text = "Max"},
                new GUIContent { text = "Value"},
                new GUIContent { text = "Normalize"},
            };
            EditorGUI.MultiPropertyField(rect, contents, value.FindPropertyRelative("Min"), EditorGUI.PropertyVisibility.All);
            original.boxedValue = value.boxedValue;
        }
    }
}
