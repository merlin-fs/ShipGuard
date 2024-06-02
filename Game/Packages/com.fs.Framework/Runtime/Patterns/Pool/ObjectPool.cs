using System;
using System.Collections.Concurrent;

namespace System.Pool
{
    public class ObjectPool<T> : IDisposable 
        where T : class
    {
        private readonly ConcurrentBag<T> m_Objects;

        private readonly Func<object, T> m_CreateFunc;

        private readonly Action<T, object> m_ActionOnGet;

        private readonly Action<T> m_ActionOnRelease;

        private readonly Action<T> m_ActionOnDestroy;

        private readonly int m_MaxSize;

        public ObjectPool(
            Func<object, T> createFunc,
            Action<T, object> actionOnGet = null, 
            Action<T> actionOnRelease = null, 
            Action<T> actionOnDestroy = null, 
            int maxSize = 10000)
        {
            if (createFunc == null)
            {
                throw new ArgumentNullException("createFunc");
            }

            if (maxSize <= 0)
            {
                throw new ArgumentException("Max Size must be greater than 0", "maxSize");
            }

            m_Objects = new ConcurrentBag<T>();
            m_CreateFunc = createFunc;
            m_MaxSize = maxSize;
            m_ActionOnGet = actionOnGet;
            m_ActionOnRelease = actionOnRelease;
            m_ActionOnDestroy = actionOnDestroy;
        }

        public T Get(object value = null)
        {
            var obj = m_Objects.TryTake(out T item) ? item : m_CreateFunc(value);
            m_ActionOnGet?.Invoke(obj, value);
            return obj;
        }

        public void Release(T element)
        {
            m_ActionOnRelease?.Invoke(element);
            if (m_Objects.Count < m_MaxSize)
            {
                m_Objects.Add(element);
            }
            else
            {
                m_ActionOnDestroy?.Invoke(element);
            }
        }

        public void Clear()
        {
            if (m_ActionOnDestroy != null)
            {
                foreach (T item in m_Objects)
                {
                    m_ActionOnDestroy(item);
                }
            }
            m_Objects.Clear();
        }

        public void Dispose()
        {
            Clear();
        }
    }
}