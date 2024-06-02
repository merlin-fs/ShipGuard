using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Common.UI
{
    public class RootClicked : UIWidget
    {
        private string m_RootName;
        public VisualElement VisualElement { get; private set; }

        public RootClicked(string rootName)
        {
            m_RootName = rootName;
        }
        
        private void OnClick()
        {
            using var cancel = NavigationCancelEvent.GetPooled();
            VisualElement.panel.visualTree.SendEvent(cancel);
        }

        protected override void Bind()
        {
            var root = Document.rootVisualElement.Q(m_RootName);
            VisualElement = new Button(OnClick);
            VisualElement.style.position = Position.Absolute;
            VisualElement.style.top = 0;
            VisualElement.style.left = 0;
            VisualElement.style.flexGrow = new StyleFloat(1);
            VisualElement.style.height = new StyleLength(new Length(100, LengthUnit.Percent));
            VisualElement.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            VisualElement.style.opacity = new StyleFloat(0f);
            root.Insert(0, VisualElement);
        }

        public override IEnumerable<VisualElement> GetElements()
        {
            yield return VisualElement;
        }
    }
}