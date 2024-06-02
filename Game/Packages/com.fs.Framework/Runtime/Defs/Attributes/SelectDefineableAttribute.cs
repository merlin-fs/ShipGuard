using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
public class SelectTypeAttribute : PropertyAttribute
{
    public Type SelectType { get; }
    public SelectTypeAttribute(Type selectType)
    {
        SelectType = selectType;
    }
}
