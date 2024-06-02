using System.Linq;
using System.Reflection;

namespace Reflex.Caching
{
    internal sealed class TypeAttributeInfo
    {
        public readonly FieldInfo[] InjectableFields;
        public readonly PropertyInfo[] InjectableProperties;
        public readonly InjectedMethodInfo[] InjectableMethods;
        public readonly FieldInfo[] InjectableFieldsStatic;

        public TypeAttributeInfo(FieldInfo[] injectableFields, FieldInfo[] injectableFieldsStatic, PropertyInfo[] injectableProperties, MethodInfo[] injectableMethods)
        {
            InjectableFields = injectableFields;
            InjectableProperties = injectableProperties;
            InjectableMethods = injectableMethods.Select(mi => new InjectedMethodInfo(mi)).ToArray();
            InjectableFieldsStatic = injectableFieldsStatic;
        }
    }
}