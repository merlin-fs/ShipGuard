using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using System.Reflection.Ext;

namespace System.Reflection
{
    public class TypeList
	{
		public TypeList(Type baseType)
		{
			BaseType = baseType;
		}

		public Type BaseType { get; private set; }

		public List<Type> Types;
		public List<string> Paths;
	}

	public static class TypeHelper
	{
        private static Dictionary<string, TypeList> _derivedTypeLists = new Dictionary<string, TypeList>();

		#region Listing

		public static IEnumerable<Type> GetDerivedTypes<BaseType>(bool includeAbstract)
		{
			return typeof(BaseType).EditorGetDerivedTypes(includeAbstract);
		}

		public static IEnumerable<Type> GetTypesWithAttribute<AttributeType>() where AttributeType : Attribute
		{
			return TypeCache.GetTypesWithAttribute<AttributeType>();
		}

		public static IEnumerable<Type> GetTypesWithAttribute(Type attributeType)
		{
			return TypeCache.GetTypesWithAttribute(attributeType);
		}

		public static TypeList GetTypeList<T>(bool includeAbstract, bool includeNull)
		{
			return GetTypeList(typeof(T), includeAbstract, includeNull);
		}

		public static TypeList GetTypeList(Type baseType, bool includeAbstract, bool includeNull)
		{
			// include the settings in the name so lists of the same type can be created with different settings
			var listName = string.Format("{0}-{1}", includeAbstract, baseType.AssemblyQualifiedName);

			if (!_derivedTypeLists.TryGetValue(listName, out var typeList))
			{
				typeList = new TypeList(baseType);
				_derivedTypeLists.Add(listName, typeList);
			}

			if (typeList.Types == null)
			{
				var types = baseType.GetDerivedTypes(includeAbstract);

                IEnumerable<PathedType> ordered = types
                    .Select(type => new PathedType(types, baseType, type, includeAbstract))
                    .OrderBy(type => type.Path);
                if (includeNull)
                    ordered = ordered.Prepend(new PathedType());

                typeList.Types = ordered.Select(type => type.Type).ToList();
				typeList.Paths = ordered.Select(type => type.Path).ToList();
			}
			return typeList;
		}

        public static Type GetRealTypeFromTypename(string stringType)
        {
            var names = GetSplitNamesFromTypename(stringType);
            var realType = Type.GetType($"{names.ClassName}, {names.AssemblyName}");
            return realType;
        }

        public static (string AssemblyName, string ClassName) GetSplitNamesFromTypename(string typename)
        {
            if (string.IsNullOrEmpty(typename))
                return ("", "");
            var typeSplitString = typename.Split(char.Parse(" "));
            var typeClassName = typeSplitString[1];
            var typeAssemblyName = typeSplitString[0];
            return (typeAssemblyName, typeClassName);
        }

        private class PathedType
		{
			public Type Type;
			public string Path;

			public PathedType(IEnumerable<Type> types, Type rootType, Type type, bool includeAbstract)
			{
                Type = type;
                Path = Type.ToGenericTypeString();

                // repeat the name for types that have derivations so they appear in their own submenu (otherwise they wouldn't be selectable)
                if (type != rootType)
				{
					if (types.Any(t => t.BaseType == type))
						Path += "/" + Type.ToGenericTypeString();

                    type = type.BaseType;
				}

				// prepend all parent type names up to but not including the root type
				while (type != rootType && type != typeof(object)) // check against object in case rootType is an interface
				{
                    if (type.IsAbstract && !includeAbstract)
                        break;
					Path = type.ToGenericTypeString() + "/" + Path;
					type = type.BaseType;
				}
			}
            
            public PathedType()
            {
                Type = null;
                Path = "(null)";
            }
        }

		#endregion

		#region Utility

		public static Type FindType(string name)
		{
			// search with normal rules
			var type = Type.GetType(name);

			// search in default runtime assembly
			if (type == null)
				type = Type.GetType($"{name}, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");

			// search in default editor assembly
			if (type == null)
				type = Type.GetType($"{name}, Assembly-CSharp-Editor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");

			// search in Unity
			if (type == null)
				type = typeof(Object).Assembly.GetType(name);

			return type;
		}

        #endregion
    }
}
