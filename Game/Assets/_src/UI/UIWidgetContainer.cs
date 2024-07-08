using System;
using System.Collections.Generic;

using Common.UI;

namespace Game.UI
{
    public abstract class UIWidgetContainer: UIVisualElementWidget, IWidgetContainer
    {
        private readonly HashSet<Type> m_SubItems = new();
        
        public UIWidgetContainer()
        {
            RegistrySubItems(new RegistryContainer(this));
        }

        public abstract void RegistrySubItems(RegistryContainer container);
        
        public IEnumerable<Type> GetWidgetTypes() => m_SubItems;

        public class RegistryContainer
        {
            private UIWidgetContainer m_Owner;
            public RegistryContainer(UIWidgetContainer owner) => m_Owner = owner; 
            public void Add<T>()
                where T : IWidget
            {
                m_Owner.m_SubItems.Add(typeof(T));
            }
        }
    }
}
