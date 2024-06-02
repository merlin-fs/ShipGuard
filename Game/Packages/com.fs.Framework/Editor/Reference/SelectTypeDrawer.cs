using System;
using UnityEngine;

namespace UnityEditor.Inspector
{
    [CustomPropertyDrawer(typeof(SelectTypeAttribute), true)]
    class SelectTypeDrawer : BaseReferenceDrawer
    {
        protected override void GetDisplayValue(object value, ref string display)
        {
            display = (string)value ?? "(null)";
        }

        protected override void OnSelect(SerializedProperty property, Type type)
        {
            string value = type?.FullName;
            property.stringValue = value ?? null;
            property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.Update();
        }

        protected override Type GetBaseType(SerializedProperty property)
        {
            SelectTypeAttribute attr = (SelectTypeAttribute)attribute;
            var type = attr.SelectType;
            return type;
        }

        protected override Rect GetRect(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = EditorGUIUtility.singleLineHeight;
            return EditorGUI.PrefixLabel(position, label); 
        }
    }
}
