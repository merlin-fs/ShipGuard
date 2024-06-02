using System;

namespace Common.UI
{
    public enum ShowStyle
    {
        Normal,
        Popup,
    }

    public interface IUIManager
    {
        void Show<T>(bool show) where T : IWidget;

        void ShowCancelButton(bool show);
    }
}