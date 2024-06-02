using System;
using System.IO;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace UnityEditor.Inspector
{
    [CustomPropertyDrawer(typeof(ReferenceSelectAttribute))]
    class ReferenceDrawer : BaseReferenceDrawer
    {
        protected override void GetDisplayValue(object value, ref string display)
        {
            display = display.Replace("Def", "");
        }

        protected override void OnSelect(SerializedProperty property, Type type)
        {
            var value = (type != null) ? Activator.CreateInstance(type) : null;
            property.managedReferenceValue = value ?? null;
            property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.Update();
        }

        protected override Type GetBaseType(SerializedProperty property)
        {
            ReferenceSelectAttribute attr = (ReferenceSelectAttribute)attribute;
            Type fieldType = TypeHelper.GetRealTypeFromTypename(property.managedReferenceFieldTypename);
            return attr.FieldType ?? fieldType;
        }

        protected override void FinalizeProperty(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, label, true);
        }

        protected override Rect GetRect(Rect position, SerializedProperty property, GUIContent label)
        {
            position = base.GetRect(position, property, label);
            position.height = EditorGUIUtility.singleLineHeight;
            
            position.x += EditorGUIUtility.labelWidth + EditorGUI.indentLevel * 15 + 3;
            position.width -= EditorGUIUtility.labelWidth + 4;
            return position;
        }
    }
}
