using System;

namespace Common.UI
{
    public enum ShowStyle
    {
        Normal,
        Popup,
    }

    public partial interface IUIManager<TLayer, T, TManifest>
        where TManifest : WidgetShowManifest<TLayer, T>
    {
        TManifest Show<TWidget>() where TWidget : IWidget;
        void Hide<TWidget>() where TWidget : IWidget;
    }
}