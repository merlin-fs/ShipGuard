using System;
using System.Collections.Generic;
using Common.UI;
using UnityEngine.UIElements;

namespace Common.UI
{
    public abstract class UIWidget: IWidget
    {
        protected UIDocument Document { get; private set; }
        
        public void Bind(UIDocument document)
        {
            Document = document;
            Bind();
        }
        protected abstract void Bind();
        public abstract IEnumerable<VisualElement> GetElements();
        
    }
}
