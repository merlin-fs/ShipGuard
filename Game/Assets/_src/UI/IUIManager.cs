using Common.UI;

using UnityEngine;

namespace Game.UI
{
    public enum UILayer
    {
        Loading,
        Windows,
        Main,
    }
    
    public interface IUIManager : IUIManager<UILayer>
    {
    }
}
