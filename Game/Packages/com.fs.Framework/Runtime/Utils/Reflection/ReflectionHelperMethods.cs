using System;

namespace System.Reflection
{
    public static partial class ReflectionHelper
    {
        public const BindingFlags STATIC = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

        public static T GetDelegate<T> (this Type self, string methodName, 
            BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            where T : Delegate
        {
            var method = self.GetMethod(methodName, bindingAttr);
            return (T)method?.CreateDelegate(typeof(T));
        }
    }
}