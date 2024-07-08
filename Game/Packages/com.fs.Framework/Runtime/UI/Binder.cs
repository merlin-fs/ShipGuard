using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine.Pool;

namespace Common.UI
{
    public sealed class Binder<T>
    {
        public T Element { get; internal set; }
        private List<T> m_Elements;
        private List<CallbackHandler> m_CallbackHandlers;

        public void AddAlternativeShowElement<TC>(TC element, Action<Binder<TC>> bind = null)
            where TC : T
        {
            m_Elements ??= ListPool<T>.Get();
            m_Elements.Add(element);
            var child = Binder<TC>.GetNewBinder(element);
            bind?.Invoke(child);
            
            if (child.m_CallbackHandlers != null)
            {
                m_CallbackHandlers ??= ListPool<CallbackHandler>.Get();
                m_CallbackHandlers.AddRange(child.m_CallbackHandlers);
            }

            if (child.m_Elements != null)
            {
                m_Elements.AddRange(child.m_Elements.Cast<T>());
            }
            child.Release();
        }

        internal static Binder<T> GetNewBinder(T element)
        {
            var result = GenericPool<Binder<T>>.Get();
            result.Element = element;
            return result;
        }

        internal void Release()
        {
            if (m_Elements != null)
                ListPool<T>.Release(m_Elements);
            if (m_CallbackHandlers != null)
                ListPool<CallbackHandler>.Release(m_CallbackHandlers);
            m_Elements = null;
            m_CallbackHandlers = null;
            GenericPool<Binder<T>>.Release(this);
        }

        public void Callback<TDelegate>(TDelegate callback, Action<Binder<T>, TDelegate> addHandler,
            Action<Binder<T>, TDelegate> removeHandler)
        {
            m_CallbackHandlers ??= ListPool<CallbackHandler>.Get();
            CallbackHandler handler = default;
            handler.AddHandler = AddHandler(this, addHandler, callback);
            handler.RemoveHandler = AddHandler(this, removeHandler, callback);
            m_CallbackHandlers.Add(handler);
        }

        private static Action AddHandler<TDelegate>(Binder<T> binder, Action<Binder<T>, TDelegate> @delegate, TDelegate callback)
        {
            return () => @delegate.Invoke(binder, callback);
        }
        
        internal (T[] elements, CallbackHandler[] callbackHandlers) Build()
        {
            (T[] elements, CallbackHandler[] callbackHandlers) result; 
            result.callbackHandlers = m_CallbackHandlers?.ToArray() ?? new CallbackHandler[]{};
            result.elements = m_Elements?.ToArray() ?? new T[]{Element};
            return result;
        }
    }

    internal class ListCallbacks : List<CallbackHandler>{};
    internal struct CallbackHandler
    {
        public Action AddHandler;
        public Action RemoveHandler;
    }
}