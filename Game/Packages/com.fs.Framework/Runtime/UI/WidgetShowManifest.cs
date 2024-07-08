using System;

using Common.Core;

namespace Common.UI
{
    public abstract class WidgetShowManifest<TLayer, T>
    {
        protected bool Show { get; }
        protected Type WidgetType { get; }
        protected TLayer Layer { get; private set;}
        protected Action<Binder<T>> BindingObjectAction { get; private set;}
        protected Func<IWidget> GetWidget { get; set;}
        
        public WidgetShowManifest<TLayer, T> WithData<TObj>(Func<TObj> getter)
        {
            BindingObjectAction = binder => BindObject<TObj>(binder, GetWidget.Invoke(), getter.Invoke());
            return this;
        }

        protected WidgetShowManifest(Type widgetType, bool show)
        {
            BindingObjectAction = binder => BindObject<object>(binder, GetWidget.Invoke(), default);
            WidgetType = widgetType;
            Show = show;
        }

        protected abstract IWidget CreateWidget(IContainer container);
        protected abstract T AttachDocument(IWidget widget, Binder<T> parent);

        private static void BindObject<TObj>(Binder<T> binder, IWidget widget, TObj obj)
        {
            switch (widget)
            {
                case IWidget<TObj, T> widgetData:
                    widgetData.Bind(binder, obj);
                    break;
                case IWidgetNoData<T> widgetNoData:
                    widgetNoData.Bind(binder);
                    break;
                default:
                    throw new NotImplementedException($"{widget} not implemented Bind()");
            }
        }
            
        public WidgetShowManifest<TLayer, T> WithLayer(TLayer layer)
        {
            Layer = layer;
            return this;
        }

        internal interface IAccessManifest
        {
            public IContainer Container { get; }
            bool IsShow { get; }
            Type WidgetType { get; }
            IWidget CreateWidget();
            TLayer Layer { get; }
            T AttachDocument(IWidget widget, Binder<T> parent);
            Action<Binder<T>> BindingObjectAction { get; }
        }

        internal class AccessManifest<TManifest> : IAccessManifest
            where TManifest : WidgetShowManifest<TLayer, T>
        {
            public IContainer Container { get; }
            public TManifest Manifest { get; }

            public Action<Binder<T>> BindingObjectAction => Manifest.BindingObjectAction;
            public bool IsShow => Manifest.Show;
            public Type WidgetType => Manifest.WidgetType;
            public IWidget CreateWidget()
            {
                var widget = Manifest.CreateWidget(Container);
                Manifest.GetWidget = () => widget;
                return widget;
            }

            public TLayer Layer => Manifest.Layer;
            public T AttachDocument(IWidget widget, Binder<T> parent) => Manifest.AttachDocument(widget, parent);

            public AccessManifest(TManifest manifest, IContainer container)
            {
                Container = container; 
                Manifest = manifest;
            }
        }
    }
}
