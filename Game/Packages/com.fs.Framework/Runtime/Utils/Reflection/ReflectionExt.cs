using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace System.Reflection.Ext
{
	#if UNITY_EDITOR
	public static class ReflectionEditorExt
	{
		// Gets value from SerializedProperty - even if value is nested
		public static object GetValue(this UnityEditor.SerializedProperty property)
		{
            if (property == null) return null;

            var path = property.propertyPath.Replace(".Array.data[", "[");
            object obj = property.serializedObject.targetObject;
            var elements = path.Split('.');
            foreach (var element in elements)
            {
                if (element.Contains("["))
                {
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    obj = GetValue_Imp(obj, elementName, index);
                }
                else
                {
                    obj = GetValue_Imp(obj, element);
                }
            }
            return obj;
        }

		// Sets value from SerializedProperty - even if value is nested
		public static void SetValue(this UnityEditor.SerializedProperty property, object value)
		{
            var path = property.propertyPath.Replace(".Array.data[", "[");
            object obj = property.serializedObject.targetObject;
            var elements = path.Split('.');
            foreach (var element in elements.Take(elements.Length - 1))
            {
                if (element.Contains("["))
                {
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    obj = GetValue_Imp(obj, elementName, index);
                }
                else
                {
                    obj = GetValue_Imp(obj, element);
                }
            }

            if (Object.ReferenceEquals(obj, null)) return;

            try
            {
                var element = elements.Last();

                if (element.Contains("["))
                {
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    var arr = ReflectionUtil.GetValue(obj, elementName) as System.Collections.IList;
                    if (arr != null) arr[index] = value;
                }
                else
                {
                    ReflectionUtil.SetValue(obj, element, value);
                }

            }
            catch
            {
                return;
            }
        }

        private static object GetValue_Imp(object source, string name)
        {
            if (source == null)
                return null;
            var type = source.GetType();

            while (type != null)
            {
                var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (f != null)
                    return f.GetValue(source);

                var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (p != null)
                    return p.GetValue(source, null);

                type = type.BaseType;
            }
            return null;
        }

        private static object GetValue_Imp(object source, string name, int index)
        {
            var enumerable = GetValue_Imp(source, name) as System.Collections.IEnumerable;
            if (enumerable == null) return null;
            var enm = enumerable.GetEnumerator();
            
            for (int i = 0; i <= index; i++)
            {
                if (!enm.MoveNext()) return null;
            }
            return enm.Current;
        }
    }
	#endif

    public static class ReflectionExt
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

        public static void SetProperty(object rObject, string rName, object rValue)
        {
            PropertyInfo[] properties = rObject.GetType().GetProperties();
            if (properties != null && properties.Length != 0)
            {
                for (int i = 0; i < properties.Length; i++)
                {
                    if (properties[i].Name == rName && properties[i].CanWrite)
                    {
                        properties[i].SetValue(rObject, rValue, null);
                        return;
                    }
                }
            }
        }

        public static bool IsTypeValid(string rType)
        {
            bool result;
            try
            {
                result = (Type.GetType(rType) != null);
            }
            catch
            {
                result = false;
            }
            return result;
        }

        public static bool IsPrimitive(Type rType)
        {
            return rType.IsPrimitive;
        }

        public static bool IsValueType(Type rType)
        {
            return rType.IsValueType;
        }

        public static bool IsGenericType(Type rType)
        {
            return rType.IsGenericType;
        }

        public static object GetDefaultValue(Type rType)
        {
            if (rType.IsValueType)
            {
                return Activator.CreateInstance(rType);
            }
            Vector3 vector = default(Vector3);
            return vector.GetType().GetMethod("GetDefaultGeneric").MakeGenericMethod(new Type[]
            {
            rType
            }).Invoke(vector, null);
        }
    }
}