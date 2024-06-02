using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class SubclassSelectorAttribute : PropertyAttribute
{
    public SubclassSelectorAttribute(Type type, bool readOnly = false, bool unique = false)
    {
        FieldType = type;
        Unique = unique;
        ReadOnly = readOnly;
    }

    public Type FieldType { get; }
    public bool Unique { get; }
    public bool ReadOnly { get; }
    
}
