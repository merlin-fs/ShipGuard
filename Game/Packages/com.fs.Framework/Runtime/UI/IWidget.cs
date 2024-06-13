using JetBrains.Annotations;

using UnityEngine.UIElements;

namespace Common.UI
{
    public interface IWidget
    {
        [CanBeNull]
        public VisualTreeAsset GetTemplate() => null;
    }

    public interface IWidget<in T> : IWidget
    {
        void Bind(VisualElement root, T bindObject);
    }

    public interface IWidgetNoData : IWidget
    {
        void Bind(VisualElement root);
    }
}
