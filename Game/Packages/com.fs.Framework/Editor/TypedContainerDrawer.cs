using System;
using System.Reflection.Ext;
using UnityEngine;

#if UNITY_EDITOR
namespace UnityEditor.Inspector
{
	[CustomPropertyDrawer(typeof(ATypedContainer), true)]
	public class TypedContainerClassDrawer : PropertyDrawer
	{
		const string OBJ_FIELD_NAME = "m_Obj";

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var tc = property.GetValue() as ATypedContainer;
			tc?.Validate();
			string typeName = tc?.GetType().BaseType.GenericTypeArguments[0].Name ?? "";

			var objProp = property.FindPropertyRelative(OBJ_FIELD_NAME);
			if (objProp == null)
				throw new InvalidCastException($"Can't find {OBJ_FIELD_NAME} field in {property.type}");

			label.text = $"{property.displayName} ( {typeName} )";
			EditorGUI.PropertyField(position, objProp, label);
		}
	}
}
#endif
