using UnityEngine;

namespace System
{
	public class ReferenceSelectAttribute : PropertyAttribute
    {
        public ReferenceSelectAttribute(Type type = null, bool readOnly = false)
        {
            FieldType = type;
            ReadOnly = readOnly;
        }

        public Type FieldType { get; }
        public bool ReadOnly { get; }
    }
}
