using System.Collections.Concurrent;

using Common.Core;

namespace Reflex.Core
{
    internal class InitializationManager: IInitialization
    {
        private static InitializationManager m_Instance;
        
        private ConcurrentDictionary<IInitialization, IContainer> m_Queue = new();
        private bool m_Active;

        public InitializationManager()
        {
            m_Instance = this;
        }
        
        public static void Initialization(object instance, IContainer container)
        {
            if (m_Instance == null) return;
            
            if (instance is not IInitialization initialization || instance == m_Instance) return;
            
            if (!m_Instance.m_Active) 
                m_Instance.Add(initialization, container);
            else
                initialization.Initialization(container);
        }

        private void Add(IInitialization instance, IContainer container)
        {
            m_Queue.TryAdd(instance, container);
        }
        
        public void Initialization(IContainer dummy)
        {
            m_Active = true;
            foreach ((IInitialization iter, IContainer container) in m_Queue)
            {
                iter.Initialization(container);
            }
            m_Queue.Clear();
        }
    }
}