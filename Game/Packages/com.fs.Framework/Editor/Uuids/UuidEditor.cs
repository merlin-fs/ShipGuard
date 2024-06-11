using System.Reflection.Ext;

using Common.Core;

using UnityEditor.UIElements;

using UnityEngine.UIElements;


namespace UnityEditor.Inspector
{

    [CustomPropertyDrawer(typeof(Uuid), true)]
    public class UuidDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var field = new Label();
            field.TrackPropertyValue(property, prop => UpdateDrawer(field, prop));
            UpdateDrawer(field, property);
            return field;
        }

        private void UpdateDrawer(Label field, SerializedProperty property)
        {
            field.text = $"UID : {((Uuid)property.GetValue()).Name}";
        }
    }
}