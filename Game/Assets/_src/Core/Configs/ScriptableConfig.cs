using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Common.Core;
using Common.Defs;

using Game.Views;

using UnityEngine;
using Unity.Entities;

using UnityEngine.AddressableAssets;

namespace Game.Core.Defs
{
    [Serializable]
    public class ChildConfig
    {
        public ScriptableConfig Child;
//#if UNITY_EDITOR
        [SelectChildPrefab]
        public GameObject PrefabObject;
        public bool Enabled = true;
//#endif
    }

    public interface IConfigContainer
    {
        IEnumerable<ChildConfig> Childs { get; }
    }

    public abstract class ScriptableConfig : ScriptableIdentifiable, IConfig, IViewPrefab, IConfigWritable
    {
        [field: SerializeField]
        public AssetReferenceGameObject ReferencePrefab { get; private set; }

        private GameObject m_ViewPrefab;
        
        public GameObject GetViewPrefab()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                m_ViewPrefab = ReferencePrefab.editorAsset;
#endif
            return m_ViewPrefab;
        }
        private Entity m_Prefab;

        public Entity EntityPrefab => m_Prefab;

        public async Task PreloadPrefab()
        {
            if (!ReferencePrefab.RuntimeKeyIsValid()) return;
            m_ViewPrefab = await ReferencePrefab.LoadAssetAsync().Task;
        }
        
        void IConfig.Configure(Entity root, EntityManager manager, IDefinableContext context)
        {
            Configure(root, manager, context);
        }

        public abstract void Configure(Entity entity, EntityManager manager, IDefinableContext context);
        public abstract ComponentTypeSet GetComponentTypeSet();
        public virtual void Configure(IView view, Entity entity, EntityManager manager) { }
        public void SetEntityPrefab(Entity entity)
        {
            m_Prefab = entity;
        }
    }

    public interface IConfigWritable 
    {
        void SetEntityPrefab(Entity entity);
    }
    
}
