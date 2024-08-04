using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
public class SelectObjectIDAttribute : PropertyAttribute
{
    public Type SelectType { get; }
    public SelectObjectIDAttribute(Type selectType)
    {
        SelectType = selectType;
    }
    
}
