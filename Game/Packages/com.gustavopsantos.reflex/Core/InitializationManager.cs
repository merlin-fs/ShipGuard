using System.Collections.Concurrent;

using Common.Core;

namespace Reflex.Core
{
    internal class InitializationManager: IInitialization
    {
        internal static InitializationManager Instance { get; private set; }
        
        private ConcurrentDictionary<IInitialization, IContainer> m_Queue = new();
        private bool m_Active;

        public InitializationManager()
        {
            Instance = this;
        }
        
        public void Initialization(object instance, IContainer container)
        {
            if (instance is not IInitialization initialization || instance == this) return;
            
            if (!m_Active) 
                Add(initialization, container);
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