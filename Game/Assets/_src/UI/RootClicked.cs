using Common.UI;

using UnityEngine.UIElements;

namespace Game.UI
{
    public class RootClicked : UIVisualElementWidget
    {
        private readonly VisualElement m_Root;
        public VisualElement VisualElement { get; private set; }

        public RootClicked(VisualElement root)
        {
            m_Root = root;
        }
        
        private void OnClick()
        {
            using var cancel = NavigationCancelEvent.GetPooled();
            VisualElement.panel.visualTree.SendEvent(cancel);
        }

        public override void Bind(Binder<VisualElement> binder)
        {
            VisualElement = new Button(OnClick);
            VisualElement.style.position = Position.Absolute;
            VisualElement.style.top = 0;
            VisualElement.style.left = 0;
            VisualElement.style.flexGrow = new StyleFloat(1);
            VisualElement.style.height = new StyleLength(new Length(100, LengthUnit.Percent));
            VisualElement.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            VisualElement.style.opacity = new StyleFloat(0f);
            m_Root.Insert(0, VisualElement);
        }
    }
}