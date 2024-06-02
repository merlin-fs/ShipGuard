using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace System.Reflection.Ext
{
    public static class ReflectionUtil
    {
        #region ReflectionUtil Methods
        public static bool SetValue(this object obj, string sprop, object value)
        {
            if (obj == null) return false;
            return SetValueDirect(obj, sprop, value, (object[])null);
        }

        public static bool SetValue(this object obj, string sprop, object value, params object[] index)
        {
            if (obj == null) return false;
            return SetValueDirect(obj, sprop, value, index);
        }

        public static bool SetValue(this object obj, MemberInfo member, object value)
        {
            if (obj == null) return false;
            return SetValueDirect(obj, member, value, (object[])null);
        }

        public static bool SetValue(this object obj, MemberInfo member, object value, params object[] index)
        {
            if (obj == null) return false;
            return SetValueDirect(obj, member, value, index);
        }

        public static object GetValue(this object obj, string sprop, params object[] args)
        {
            if (obj == null) return null;
            return GetValueDirect(obj, sprop, args);
        }

        public static object GetValue(this object obj, MemberInfo member, params object[] args)
        {
            if (obj == null) return null;
            return GetValueDirect(obj, member, args);
        }

        public static bool TryGetValue(this object obj, string sMemberName, out object result, params object[] args)
        {
            if (obj == null)
            {
                result = null;
                return false;
            }
            return TryGetValueDirect(obj, sMemberName, out result, args);
        }

        public static bool HasMember(object obj, string name, bool includeNonPublic)
        {
            if (obj == null) return false;
            return TypeHasMember(obj.GetType(), name, includeNonPublic);
        }

        public static IEnumerable<MemberInfo> GetMembers(object obj, bool includeNonPublic)
        {
            if (obj == null) return Enumerable.Empty<MemberInfo>();
            return GetMembersFromType(obj.GetType(), includeNonPublic);
        }

        public static IEnumerable<MemberInfo> GetMembers(object obj, bool includeNonPublic, MemberTypes mask)
        {
            if (obj == null) return Enumerable.Empty<MemberInfo>();
            return GetMembersFromType(obj.GetType(), includeNonPublic, mask);
        }

        public static IEnumerable<string> GetMemberNames(object obj, bool includeNonPublic)
        {
            if (obj == null) return Enumerable.Empty<string>();
            return GetMemberNamesFromType(obj.GetType(), includeNonPublic);
        }

        public static IEnumerable<string> GetMemberNames(object obj, bool includeNonPublic, MemberTypes mask)
        {
            if (obj == null) return Enumerable.Empty<string>();
            return GetMemberNamesFromType(obj.GetType(), includeNonPublic, mask);
        }

        public static MemberInfo GetMember(object obj, string sMemberName, bool includeNonPublic)
        {
            if (obj == null) return null;
            return GetMemberFromType(obj.GetType(), sMemberName, includeNonPublic);
        }


        public static object GetValueRecursively(this object obj, string sprop)
        {
            if (sprop.Contains('.')) obj = ReduceSubObject(obj, sprop, out sprop);
            return GetValue(obj, sprop);
        }

        #endregion
        #region Direct Reflection
        public static bool SetValueDirect(object obj, string sprop, object value)
        {
            return SetValueDirect(obj, sprop, value, (object[])null);
        }

        public static bool SetValueDirect(object obj, string sprop, object value, params object[] index)
        {
            if (string.IsNullOrEmpty(sprop)) return false;

            if (obj == null) return false;

            try
            {
                var vtp = (value != null) ? value.GetType() : null;
                var member = GetValueSetterMemberFromType(obj.GetType(), sprop, vtp, true);
                if (member != null)
                {
                    switch (member.MemberType)
                    {
                        case MemberTypes.Field:
                            (member as FieldInfo).SetValue(obj, value);
                            return true;
                        case MemberTypes.Property:
                            (member as PropertyInfo).SetValue(obj, value, index);
                            return true;
                        case MemberTypes.Method:
                            (member as MethodInfo).Invoke(obj, new object[] { value });
                            return true;
                    }
                }

                if (vtp != null)
                {
                    member = GetValueSetterMemberFromType(obj.GetType(), sprop, null, true);
                    if (member != null)
                    {
                        var rtp = GetReturnType(member);
                        switch (member.MemberType)
                        {
                            case MemberTypes.Field:
                                (member as FieldInfo).SetValue(obj, value);
                                return true;
                            case MemberTypes.Property:
                                (member as PropertyInfo).SetValue(obj, value, index);
                                return true;
                            case MemberTypes.Method:
                                (member as MethodInfo).Invoke(obj, new object[] { value });
                                return true;
                        }
                    }
                }
            }
            catch
            {

            }

            return false;
        }

        public static bool SetValueDirect(object obj, MemberInfo member, object value)
        {
            return SetValueDirect(obj, member, value, (object[])null);
        }

        public static bool SetValueDirect(object obj, MemberInfo member, object value, params object[] index)
        {
            if (obj == null) return false;

            if (member == null) return false;

            try
            {
                switch (member.MemberType)
                {
                    case MemberTypes.Field:
                        (member as FieldInfo).SetValue(obj, value);
                        return true;
                    case MemberTypes.Property:
                        if ((member as PropertyInfo).CanWrite)
                        {
                            (member as PropertyInfo).SetValue(obj, value, index);
                            return true;
                        }
                        else
                            return false;
                    case MemberTypes.Method:
                        (member as MethodInfo).Invoke(obj, new object[] { value });
                        return true;
                }
            }
            catch
            {

            }

            return false;
        }

        public static object GetValueDirect(object obj, string sprop, params object[] args)
        {
            const BindingFlags BINDING = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            if (string.IsNullOrEmpty(sprop)) return null;

            if (obj == null) return null;

            try
            {
                var tp = obj.GetType();

                while (tp != null)
                {
                    var members = tp.GetMember(sprop, BINDING);
                    if (members == null || members.Length == 0) return null;

                    foreach (var member in members)
                    {
                        switch (member.MemberType)
                        {
                            case System.Reflection.MemberTypes.Field:
                                var field = member as System.Reflection.FieldInfo;
                                return field.GetValue(obj);

                            case System.Reflection.MemberTypes.Property:
                            {
                                var prop = member as System.Reflection.PropertyInfo;
                                var paramInfos = prop.GetIndexParameters();
                                if (prop.CanRead && ParameterSignatureMatches(args, paramInfos, false))
                                {
                                    return prop.GetValue(obj, args);
                                }
                                break;
                            }
                            case System.Reflection.MemberTypes.Method:
                            {
                                var meth = member as System.Reflection.MethodInfo;
                                var paramInfos = meth.GetParameters();
                                if (ParameterSignatureMatches(args, paramInfos, false))
                                {
                                    return meth.Invoke(obj, args);
                                }
                                break;
                            }
                        }
                    }

                    tp = tp.BaseType;
                }
            }
            catch
            {

            }
            return null;
        }

        public static object GetValueDirect(this object obj, MemberInfo member, params object[] args)
        {
            switch (member.MemberType)
            {
                case System.Reflection.MemberTypes.Field:
                    var field = member as System.Reflection.FieldInfo;
                    return field.GetValue(obj);

                case System.Reflection.MemberTypes.Property:
                {
                    var prop = member as System.Reflection.PropertyInfo;
                    var paramInfos = prop.GetIndexParameters();
                    if (prop.CanRead && ParameterSignatureMatches(args, paramInfos, false))
                    {
                        return prop.GetValue(obj, args);
                    }
                    break;
                }
                case System.Reflection.MemberTypes.Method:
                {
                    var meth = member as System.Reflection.MethodInfo;
                    var paramInfos = meth.GetParameters();
                    if (ParameterSignatureMatches(args, paramInfos, false))
                    {
                        return meth.Invoke(obj, args);
                    }
                    break;
                }
            }

            return null;
        }

        public static bool TryGetValueDirect(object obj, string sprop, out object result, params object[] args)
        {
            const BindingFlags BINDING = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            result = null;
            if (string.IsNullOrEmpty(sprop)) return false;

            if (obj == null) return false;

            try
            {
                var tp = obj.GetType();

                while (tp != null)
                {
                    var members = tp.GetMember(sprop, BINDING);
                    if (members == null || members.Length == 0) return false;

                    foreach (var member in members)
                    {
                        switch (member.MemberType)
                        {
                            case System.Reflection.MemberTypes.Field:
                                var field = member as System.Reflection.FieldInfo;
                                result = field.GetValue(obj);
                                return true;

                            case System.Reflection.MemberTypes.Property:
                            {
                                var prop = member as System.Reflection.PropertyInfo;
                                var paramInfos = prop.GetIndexParameters();
                                if (prop.CanRead && ParameterSignatureMatches(args, paramInfos, false))
                                {
                                    result = prop.GetValue(obj, args);
                                    return true;
                                }
                                break;
                            }
                            case System.Reflection.MemberTypes.Method:
                            {
                                var meth = member as System.Reflection.MethodInfo;
                                var paramInfos = meth.GetParameters();
                                if (ParameterSignatureMatches(args, paramInfos, false))
                                {
                                    result = meth.Invoke(obj, args);
                                    return true;
                                }
                                break;
                            }
                        }
                    }

                    tp = tp.BaseType;
                }
            }
            catch
            {

            }
            return false;
        }

        public static bool HasMemberDirect(object obj, string name, bool includeNonPublic)
        {
            if (obj == null) return false;
            if (string.IsNullOrEmpty(name)) return false;

            return TypeHasMember(obj.GetType(), name, includeNonPublic);
        }

        public static IEnumerable<MemberInfo> GetMembersDirect(object obj, bool includeNonPublic, MemberTypes mask = MemberTypes.Field | MemberTypes.Property | MemberTypes.Method)
        {
            if (obj == null) return Enumerable.Empty<MemberInfo>();

            return GetMembersFromType(obj.GetType(), includeNonPublic, mask);
        }

        public static IEnumerable<MemberInfo> GetMemberNamesDirect(object obj, bool includeNonPublic, MemberTypes mask = MemberTypes.Field | MemberTypes.Property | MemberTypes.Method)
        {
            if (obj == null) return Enumerable.Empty<MemberInfo>();

            return GetMembersFromType(obj.GetType(), includeNonPublic, mask);
        }

        public static bool TypeHasMember(System.Type tp, string name, bool includeNonPublic)
        {
            const BindingFlags BINDING = BindingFlags.Public | BindingFlags.Instance;
            const BindingFlags PRIV_BINDING = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            if (tp == null) return false;

            var member = tp.GetMember(name, BINDING);
            if (member != null && member.Length > 0) return true;

            if (includeNonPublic)
            {
                while (tp != null)
                {
                    member = tp.GetMember(name, PRIV_BINDING);
                    if (member != null && member.Length > 0) return true;
                    tp = tp.BaseType;
                }
            }
            return false;
        }

        public static IEnumerable<MemberInfo> GetMembersFromType(System.Type tp, bool includeNonPublic, MemberTypes mask = MemberTypes.Field | MemberTypes.Property | MemberTypes.Method)
        {
            const BindingFlags BINDING = BindingFlags.Public | BindingFlags.Instance;
            const BindingFlags PRIV_BINDING = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            if (tp == null) yield break;

            foreach (var m in tp.GetMembers(BINDING))
            {
                if ((m.MemberType & mask) != 0)
                {
                    yield return m;
                }
            }

            if (includeNonPublic)
            {
                while (tp != null)
                {
                    foreach (var m in tp.GetMembers(PRIV_BINDING))
                    {
                        if ((m.MemberType & mask) != 0)
                        {
                            yield return m;
                        }
                    }
                    tp = tp.BaseType;
                }
            }
        }

        public static IEnumerable<string> GetMemberNamesFromType(System.Type tp, bool includeNonPublic, MemberTypes mask = MemberTypes.Field | MemberTypes.Property | MemberTypes.Method)
        {
            const BindingFlags BINDING = BindingFlags.Public | BindingFlags.Instance;
            const BindingFlags PRIV_BINDING = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            if (tp == null) yield break;

            foreach (var m in tp.GetMembers(BINDING))
            {
                if ((m.MemberType & mask) != 0)
                {
                    yield return m.Name;
                }
            }

            if (includeNonPublic)
            {
                while (tp != null)
                {
                    foreach (var m in tp.GetMembers(PRIV_BINDING))
                    {
                        if ((m.MemberType & mask) != 0)
                        {
                            yield return m.Name;
                        }
                    }
                    tp = tp.BaseType;
                }
            }
        }

        public static MemberInfo GetMemberFromType(Type tp, string sMemberName, bool includeNonPublic, MemberTypes mask = MemberTypes.Field | MemberTypes.Property | MemberTypes.Method)
        {
            const BindingFlags BINDING_PUBLIC = BindingFlags.Public | BindingFlags.Instance;
            const BindingFlags PRIV_BINDING = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            if (tp == null) throw new ArgumentNullException("tp");
            try
            {
                MemberInfo[] members;

                members = tp.GetMember(sMemberName, BINDING_PUBLIC);
                foreach (var member in members)
                {
                    if ((member.MemberType & mask) != 0) return member;
                }

                while (includeNonPublic && tp != null)
                {
                    members = tp.GetMember(sMemberName, PRIV_BINDING);
                    tp = tp.BaseType;
                    if (members == null || members.Length == 0) continue;

                    foreach (var member in members)
                    {
                        if ((member.MemberType & mask) != 0) return member;
                    }
                }
            }
            catch
            {

            }
            return null;
        }

        public static MemberInfo GetValueMemberFromType(Type tp, string sprop, bool includeNonPublic)
        {
            const BindingFlags BINDING_PUBLIC = BindingFlags.Public | BindingFlags.Instance;
            const BindingFlags PRIV_BINDING = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            if (tp == null) throw new ArgumentNullException("tp");
            try
            {
                MemberInfo[] members;

                members = tp.GetMember(sprop, BINDING_PUBLIC);
                foreach (var member in members)
                {
                    if (IsValidValueMember(member)) return member;
                }

                while (includeNonPublic && tp != null)
                {
                    members = tp.GetMember(sprop, PRIV_BINDING);
                    tp = tp.BaseType;
                    if (members == null || members.Length == 0) continue;

                    foreach (var member in members)
                    {
                        if (IsValidValueMember(member)) return member;
                    }
                }
            }
            catch
            {

            }
            return null;
        }

        public static MemberInfo GetValueSetterMemberFromType(Type tp, string sprop, Type valueType, bool includeNonPublic)
        {
            const BindingFlags BINDING_PUBLIC = BindingFlags.Public | BindingFlags.Instance;
            const BindingFlags PRIV_BINDING = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            if (tp == null) throw new ArgumentNullException("tp");
            try
            {
                System.Type ltp;
                MemberInfo[] members;

                members = tp.GetMember(sprop, BINDING_PUBLIC);
                foreach (var member in members)
                {
                    if (IsValidValueSetterMember(member, valueType)) return member;
                }

                ltp = tp;
                while (includeNonPublic && ltp != null)
                {
                    members = ltp.GetMember(sprop, PRIV_BINDING);
                    ltp = ltp.BaseType;
                    if (members == null || members.Length == 0) continue;

                    foreach (var member in members)
                    {
                        if (IsValidValueSetterMember(member, valueType)) return member;
                    }
                }
            }
            catch
            {

            }

            return null;
        }

        public static Type GetReturnType(MemberInfo info)
        {
            if (info == null) return null;

            switch (info.MemberType)
            {
                case MemberTypes.Field:
                    return (info as FieldInfo).FieldType;
                case MemberTypes.Property:
                    return (info as PropertyInfo).PropertyType;
                case MemberTypes.Method:
                    return (info as MethodInfo).ReturnType;
            }
            return null;
        }

        public static object GetValueWithMember(MemberInfo info, object targObj)
        {
            if (info == null) return null;

            try
            {
                switch (info.MemberType)
                {
                    case MemberTypes.Field:
                        return (info as FieldInfo).GetValue(targObj);
                    case MemberTypes.Property:
                        return (info as PropertyInfo).GetValue(targObj, null);
                    case MemberTypes.Method:
                        return (info as MethodInfo).Invoke(targObj, null);
                }
            }
            catch
            {

            }

            return null;
        }
        #endregion
        #region Helpers

        public static Type NewType(Type prototype, string name)
        {
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(prototype.Assembly.GetName(), AssemblyBuilderAccess.Run);
            ModuleBuilder mb = assemblyBuilder.DefineDynamicModule(prototype.Module.Name);
            TypeBuilder tb = mb.DefineType($"{prototype.Name}.{name}", 
                TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.SequentialLayout | TypeAttributes.Serializable,
                null, new Type[] { prototype });//, parent

            return tb.CreateType();
        }

        public static string ToGenericTypeString(this Type t)
        {
            if (!t.IsGenericType)
                return t.ToString();
            string genericTypeName = t.GetGenericTypeDefinition().ToString();
            genericTypeName = genericTypeName.Substring(0, genericTypeName.IndexOf('`'));
            string genericArgs = string.Join(",", 
                t.GetGenericArguments().Select(ta => ToGenericTypeString(ta)).ToArray());
            return genericTypeName + "<" + genericArgs + ">";
        }

        private static object ReduceSubObject(object obj, string sprop, out string lastProp)
        {
            if (obj == null)
            {
                lastProp = null;
                return null;
            }

            var arr = sprop.Split('.');
            lastProp = arr[arr.Length - 1];
            for (int i = 0; i < arr.Length - 1; i++)
            {
                obj = GetValue(obj, arr[i]);
                if (obj == null) return null;
            }

            return obj;
        }

        private static System.Type ReduceSubType(System.Type tp, string sprop, bool includeNonPublic, out string lastProp)
        {
            if (tp == null)
            {
                lastProp = null;
                return null;
            }

            var arr = sprop.Split('.');
            lastProp = arr[arr.Length - 1];
            for (int i = 0; i < arr.Length - 1; i++)
            {
                var member = GetValueMemberFromType(tp, arr[i], includeNonPublic);
                if (member == null) return null;

                tp = GetReturnType(member);
                if (tp == null) return null;
            }

            return tp;
        }

        private static bool ParameterSignatureMatches(object[] args, ParameterInfo[] paramInfos, bool allowOptional)
        {
            if (args == null) args = new object[] { };
            if (paramInfos == null) paramInfos = new ParameterInfo[] { };

            if (args.Length == 0 && paramInfos.Length == 0) return true;
            if (args.Length > paramInfos.Length) return false;

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == null)
                {
                    continue;
                }
                if (args[i].GetType().IsAssignableFrom(paramInfos[i].ParameterType))
                {
                    continue;
                }

                return false;
            }

            return paramInfos.Length == args.Length || (allowOptional && paramInfos[args.Length].IsOptional);
        }

        private static IEnumerable<MemberInfo> FilterMembers(IEnumerable<MemberInfo> members, MemberTypes mask)
        {
            foreach (var m in members)
            {
                if ((m.MemberType & mask) != 0) yield return m;
            }
        }

        private static bool IsValidValueMember(MemberInfo member)
        {
            switch (member.MemberType)
            {
                case System.Reflection.MemberTypes.Field:
                    return true;

                case System.Reflection.MemberTypes.Property:
                {
                    var prop = member as System.Reflection.PropertyInfo;
                    if (prop.CanRead && prop.GetIndexParameters().Length == 0) return true;
                    break;
                }
                case System.Reflection.MemberTypes.Method:
                {
                    var meth = member as System.Reflection.MethodInfo;
                    if (meth.GetParameters().Length == 0) return true;
                    break;
                }
            }
            return false;
        }

        private static bool IsValidValueSetterMember(MemberInfo member, System.Type valueType)
        {
            switch (member.MemberType)
            {
                case System.Reflection.MemberTypes.Field:
                    var field = member as System.Reflection.FieldInfo;
                    if ((valueType == null && !field.FieldType.IsValueType) || field.FieldType == valueType)
                    {
                        return true;
                    }

                    break;
                case System.Reflection.MemberTypes.Property:
                    var prop = member as System.Reflection.PropertyInfo;
                    if (prop.CanWrite && ((valueType == null && !prop.PropertyType.IsValueType) || prop.PropertyType.IsAssignableFrom(valueType)) && prop.GetIndexParameters().Length == 0)
                    {
                        return true;
                    }
                    break;
                case System.Reflection.MemberTypes.Method:
                {
                    var meth = member as System.Reflection.MethodInfo;
                    var paramInfos = meth.GetParameters();
                    if (paramInfos.Length != 1) return false;
                    if ((valueType == null && !paramInfos[0].ParameterType.IsValueType)
                         || paramInfos[0].ParameterType.IsAssignableFrom(valueType))
                    {
                        return true;
                    }
                }
                break;
            }
            return false;
        }

        #endregion

    }
}