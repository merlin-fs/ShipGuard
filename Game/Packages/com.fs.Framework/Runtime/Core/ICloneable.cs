using System;

namespace Common.Core
{
    public interface IData
    {
    }

    public interface ICloneableData<T>: IData
    {
        T Clone();
    }
}

namespace Common
{
    using System.Reflection;
    using Core;

    public static class DataHelper
    {
        public static IData Clone(this IData self)
        {
            Type generic = typeof(ICloneableData<>).MakeGenericType(self.GetType());
            MethodInfo method = generic.GetMethod("Clone");
            return (IData)method.Invoke(self, null);
        }
    }
}
