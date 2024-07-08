using System;
using System.Collections.Generic;
using System.Linq;

using Common.Core;

using Game.Core.Defs;
using Game.Core.Placements;
using Game.Views;

using Reflex.Core;
using Reflex.Injectors;

using Unity.Entities;

using UnityEngine;

using Object = UnityEngine.Object;

namespace Game.Core.Spawns
{
    public class SpawnFactory : MonoBehaviour, Spawn.IViewFactory
    {
        [SerializeField] private List<LayerItem> layerItems;

        private Dictionary<TypeIndex, Transform> m_Layers;
        
        private void Awake()
        {
            m_Layers = layerItems.ToDictionary(
                iter => TypeManager.GetTypeIndex(Type.GetType(iter.layer)),
                iter => iter.transform);
        }

        public IView Instantiate(IConfig config, Entity entity, EntityManager entityManager, Container container)
        {
            if (config is not IViewPrefab viewPrefab) 
                throw new NotImplementedException($"IViewPrefab {config.ID} NotImplemented");
            var prefab = viewPrefab.GetViewPrefab();
            if (!prefab) throw new ArgumentNullException($"ViewPrefab {config.ID} not assigned");
            
            //var placement = manager.GetComponentData<Map.Placement>(entity);
            //var parent = m_Layers[placement.Value.Layer];
            var parent = m_Layers.Values.First();

            var obj = Object.Instantiate<GameObject>(prefab, parent);
            var view = obj.GetComponent<IView>();
            config.Configure(view, entity, entityManager);
            GameObjectInjector.InjectRecursive(obj, container);
            
            return view;
        }

        [Serializable]
        private struct LayerItem
        {
            [SerializeField, SelectType(typeof(Placement.Layers.ILayer))]
            public string layer;

            [SerializeField] public Transform transform;
        }
    }
}
