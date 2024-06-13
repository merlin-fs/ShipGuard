using System;

using UnityEngine.UIElements;

namespace Common.UI
{
    public partial interface IUIManager<TLayer>
    {
        public class WidgetShowBinder
        {
            protected Type WidgetType { get; }
            protected bool Show { get; }
            protected TLayer Layer { get; private set;}
            protected VisualTreeAsset VisualTreeAsset { get; set;}
            protected Action<VisualElement> BindingObject { get; private set;}
            protected Func<IWidget> GetWidget { get; set;}

            protected WidgetShowBinder(Type widgetType, bool show)
            {
                BindingObject = visualElement => BindObject<TLayer>(visualElement, GetWidget.Invoke(), default);
                WidgetType = widgetType;
                Show = show;
            }

            public WidgetShowBinder WithConfig(VisualTreeAsset asset)
            {
                VisualTreeAsset = asset;
                return this;
            }

            public WidgetShowBinder WithData<T>(Func<T> getter)
            {
                BindingObject = visualElement => BindObject(visualElement, GetWidget.Invoke(), getter.Invoke());
                return this;
            }

            private static void BindObject<T>(VisualElement root, IWidget widget, T obj)
            {
                switch (widget)
                {
                    case IWidget<T> widgetData:
                        widgetData.Bind(root, obj);
                        break;
                    case IWidgetNoData widgetNoData:
                        widgetNoData.Bind(root);
                        break;
                    default:
                        throw new NotImplementedException($"{widget} not implemented Bind()");
                }
            }
            
            public WidgetShowBinder WithLayer(TLayer layer)
            {
                Layer = layer;
                return this;
            }
        }
    }
}
