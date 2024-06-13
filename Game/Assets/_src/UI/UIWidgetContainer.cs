using System;
using System.Collections.Generic;

using Common.UI;

using UnityEngine.UIElements;

namespace Game.UI
{
    public abstract class UIWidgetContainer: UIWidget, IWidgetContainer
    {
        protected override void Bind() {}
        
        public virtual IEnumerable<VisualElement> GetElements()
        {
            yield break;
        }

        public abstract IEnumerable<Type> GetWidgetTypes();
    }
}
