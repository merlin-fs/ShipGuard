using System;

using Unity.Entities;

namespace Game.Core
{
    public static class TypeIndexHelper
    {
        public static TypeIndex GetTypeIndex(this string value)
        {
            return string.IsNullOrEmpty(value) 
                ? TypeIndex.Null
                : ((ComponentType)Type.GetType(value)).TypeIndex;
        }

        public static Type GetManagedType(this TypeIndex value)
        {
            return value == TypeIndex.Null
                ? null
                : ComponentType.FromTypeIndex(value).GetManagedType();
        }
        
        public static Type GetManagedType(this int value)
        {
            return GetManagedType((TypeIndex)value);
        }
    }
}
