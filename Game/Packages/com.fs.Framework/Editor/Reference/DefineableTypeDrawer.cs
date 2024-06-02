using System;
using System.Reflection.Ext;
using UnityEngine;
using Common.Defs;

namespace UnityEditor.Inspector
{
    //[CustomPropertyDrawer(typeof(IDef), true)]
    class DefineableTypeDrawer : BaseReferenceDrawer
    {
        public DefineableTypeDrawer(): base()
        {
            m_CanPing = false;
        }

        protected override Color GetColor()
        {
            return Color.cyan;
        }

        protected override void GetDisplayValue(object value, ref string display)
        {
            var def = value;//(DefineableType)
            display = def.GetType()?.ToGenericTypeString() ?? "(null)";
        }

        protected override void OnSelect(SerializedProperty property, Type type)
        {
            //DefineableType value = (DefineableType)property.GetValue();
            /*
            value.SetType(type);
            property.SetValue(value);
            EditorUtility.SetDirty(property.serializedObject.targetObject);
            property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.Update();
            */
        }

        protected override Type GetBaseType(SerializedProperty property)
        {
            //DefineableType value = (DefineableType)property.GetValue();
            //return value.BaseType;
            return typeof(DefineableAttribute);
        }

        protected override Rect GetRect(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = EditorGUIUtility.singleLineHeight;
            return EditorGUI.PrefixLabel(position, label); 
        }

        protected override bool IsReadOnly(SerializedProperty property, Type type)
        {
            //DefineableType value = (DefineableType)property.GetValue();
            //var isDefined = value.OwnerType.IsDefined(typeof(DefineableAttribute));
            //return isDefined;
            return false;
        }
    }
}
