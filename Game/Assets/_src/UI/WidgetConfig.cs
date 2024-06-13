using System;
using System.Collections.Generic;
using System.Linq;

using Common.UI;

using Game.Core.Contexts;

using UnityEngine;
using UnityEngine.UIElements;

namespace Game.UI
{
    public class WidgetConfig : ScriptableBindConfig
    {
        [SerializeField] private LayerElement[] layers;

        [SerializeField] private WidgetItem[] widgets;

        private Dictionary<Type, VisualTreeAsset> m_Dictionary;

        public (UILayer layer, PanelSettings panelSettings, VisualTreeAsset visualTreeAsset)[] GetLayers => 
            layers.Select(iter => (iter.Layer, iter.PanelSettings, iter.RootElement)).ToArray();

        public bool TryGetTemplate(Type type, out VisualTreeAsset template)
        {
            return m_Dictionary.TryGetValue(type, out template);
        }

        public bool TryGetTemplate<T>(out VisualTreeAsset template)
            where T : IWidget
        {
            return m_Dictionary.TryGetValue(typeof(T), out template);
        }
        
        private void OnValidate()
        {
            m_Dictionary = BuildDictionary();
        }

        private Dictionary<Type, VisualTreeAsset> BuildDictionary()
        {
            return widgets.ToDictionary(
                iter => Type.GetType(iter.Type),
                iter => iter.Template);
        }
        
        [Serializable]
        private class WidgetItem
        {
            [SerializeField, SelectType(typeof(IWidget))]
            public string Type;
            public VisualTreeAsset Template;
        }

        [Serializable]
        private struct LayerElement
        {
            public UILayer Layer;
            public PanelSettings PanelSettings;
            public VisualTreeAsset RootElement;
        }
    }
}
