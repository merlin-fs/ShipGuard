using System;

namespace Common.UI
{
    public enum ShowStyle
    {
        Normal,
        Popup,
    }

    public partial interface IUIManager<TLayer>
        where TLayer : struct, IComparable, IConvertible, IFormattable
    {
        WidgetShowBinder Show<T>() where T : IWidget;
        //WidgetShowBinder Show(Type uiType);
        void Hide<T>() where T : IWidget;
        //void Hide(Type uiType);
    }
}