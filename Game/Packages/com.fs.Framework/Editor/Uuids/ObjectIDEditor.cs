using System.Linq;
using System.Reflection.Ext;

using Common.Core;

using UnityEditor;
using UnityEngine;

using UnityEditor.UIElements;

using UnityEngine.UIElements;


namespace UnityEditor.Inspector
{
    [CustomPropertyDrawer(typeof(SelectObjectIDAttribute), true)]
    public class ObjectIDEditor : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var attr = (SelectObjectIDAttribute)attribute;
            var field = new ObjectField 
            {
                objectType = attr.SelectType,
            };
            var value = property.GetValue().ToString();
            if (!string.IsNullOrEmpty(value))
            {
                var selected = Resources.FindObjectsOfTypeAll(attr.SelectType)
                    .FirstOrDefault(iter => iter.name == value);
                field.value = selected;
            }
            field.RegisterValueChangedCallback(evt =>
            {
                property.SetValue((ObjectID)evt.newValue.name);
                property.serializedObject.ApplyModifiedProperties();
                property.serializedObject.Update();
                EditorUtility.SetDirty(property.serializedObject.targetObject);
            });
            return field;
        }
    }
}