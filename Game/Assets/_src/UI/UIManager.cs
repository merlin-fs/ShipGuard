using Common.Core;
using Common.UI;

using JetBrains.Annotations;

using UnityEngine;
using UnityEngine.UIElements;

namespace Game.UI
{
    public class UIManager : UIManager<UILayer>, IUIManager
    {
        public UIManager(IContainer container, [NotNull] GameObject host, UILayer defaultLayer, params (UILayer layer, PanelSettings panelSettings, VisualTreeAsset rootElement)[] layers) : base(container, host, defaultLayer, layers)
        {
        }
    }
}
