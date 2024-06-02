using System;
using System.Collections.Generic;

namespace Common.UI
{
    public class WidgetBinder
    {
        protected readonly HashSet<Type> m_WidgetTypes = new();
            
        public void Bind<T>()
            where T : IWidget
        {
            m_WidgetTypes.Add(typeof(T));
        }
    }
}
