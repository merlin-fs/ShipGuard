using System.Collections.Concurrent;

namespace Reflex.Core
{
    internal class InitializationManager: IInitialization
    {
        internal static InitializationManager Instance { get; } = new();
        
        private ConcurrentDictionary<IInitialization, bool> m_Queue = new();
        private bool m_Active;
        
        public void Initialization(object instance)
        {
            if (instance is not IInitialization initialization || instance == this) return;
            
            if (!m_Active) 
                Add(initialization);
            else
                initialization.Initialization();
        }

        private void Add(IInitialization instance)
        {
            m_Queue.TryAdd(instance, true);
        }
        
        public void Initialization()
        {
            m_Active = true;
            foreach (var iter in m_Queue.Keys)
            {
                iter.Initialization();
            }
            m_Queue.Clear();
        }
    }
}