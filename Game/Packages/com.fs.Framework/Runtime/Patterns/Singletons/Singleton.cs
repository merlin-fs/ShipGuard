using System;

namespace Common.Singletons
{
    public class Singleton<T>
        where T: ISingleton
    {
        private static readonly T s_Instance = (T)Activator.CreateInstance(typeof(T), true);
        protected static T Inst { get => s_Instance; }
    }
}
