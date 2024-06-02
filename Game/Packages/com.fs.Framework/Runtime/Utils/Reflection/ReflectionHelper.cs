using System.Linq;
using System.Collections.Generic;

namespace System.Reflection
{
    public static partial class ReflectionHelper
    {
        public static IEnumerable<Type> GetDerivedTypes(this Type baseType, bool includeAbstract)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => baseType.IsAssignableFrom(p));
            return includeAbstract ? types : types.Where(type => !type.IsAbstract);
        }

        public static bool IsSubclassOf(Type rType, Type rBaseType)
        {
            return rType == rBaseType || rType.IsSubclassOf(rBaseType);
        }

        public static bool IsAssignableFrom(Type rType, Type rDerivedType)
        {
            return rType == rDerivedType || rType.IsAssignableFrom(rDerivedType);
        }

        public static T GetAttribute<T>(Type rObjectType)
        {
            object[] customAttributes = rObjectType.GetCustomAttributes(typeof(T), true);
            if (customAttributes == null || customAttributes.Length == 0)
            {
                return default(T);
            }
            return (T)((object)customAttributes[0]);
        }

        public static bool IsDefined(Type rObjectType, Type rType)
        {
            object[] customAttributes = rObjectType.GetCustomAttributes(rType, true);
            return customAttributes != null && customAttributes.Length != 0;
        }

        public static bool IsDefined(FieldInfo rFieldInfo, Type rType)
        {
            object[] customAttributes = rFieldInfo.GetCustomAttributes(rType, true);
            return customAttributes != null && customAttributes.Length != 0;
        }

        public static bool IsDefined(MemberInfo rMemberInfo, Type rType)
        {
            object[] customAttributes = rMemberInfo.GetCustomAttributes(rType, true);
            return customAttributes != null && customAttributes.Length != 0;
        }

        public static bool IsDefined(PropertyInfo rPropertyInfo, Type rType)
        {
            object[] customAttributes = rPropertyInfo.GetCustomAttributes(rType, true);
            return customAttributes != null && customAttributes.Length != 0;
        }
    }
}