using JetBrains.Annotations;

using UnityEngine.UIElements;

namespace Common.UI
{
    public interface IWidget
    {
        [CanBeNull]
        public VisualTreeAsset GetTemplate() => null;
    }

    public interface IWidget<in TObj, T> : IWidget
    {
        void Bind(Binder<T> binder, TObj bindObject);
    }

    public interface IWidgetNoData<T> : IWidget
    {
        void Bind(Binder<T> binder);
    }
}
