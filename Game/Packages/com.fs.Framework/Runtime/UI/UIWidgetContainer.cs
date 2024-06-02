using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Common.UI
{
    public abstract class UIWidgetContainer: UIWidget, IWidgetContainer
    {
        protected override void Bind() {}
        public override IEnumerable<VisualElement> GetElements()
        {
            yield break;
        }

        public abstract IEnumerable<Type> GetWidgetTypes();
    }
}
