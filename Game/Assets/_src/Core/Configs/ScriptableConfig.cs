using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Common.Core;
using Common.Defs;

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

    public abstract class ScriptableConfig : ScriptableIdentifiable, IConfig, IViewPrefab
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
        
        void IConfig.Configure(Entity root, IDefinableContext context)
        {
            m_Prefab = root;
            Configure(root, context);
        }

        protected abstract void Configure(Entity entity, IDefinableContext context);
    }
}
