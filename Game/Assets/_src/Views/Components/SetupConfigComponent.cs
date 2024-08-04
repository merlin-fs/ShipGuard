using Common.Core;

using Game.Core;
using Game.Core.Defs;
using Game.Core.Repositories;

using Reflex.Attributes;

using Unity.Entities;

using UnityEngine;

namespace Game.Views
{
    public class SetupConfigComponent : MonoBehaviourEditorProperties, IViewComponent
    {
        [Inject] private IContainer m_Container;
        
        #region editor
#if UNITY_EDITOR
        [SerializeField] private GameObjectConfigWithDef config;
        protected override void OnEditorSerialize()
        {
            // ReSharper disable once Unity.NoNullPropagation
            var type = config?.DefType;
            if (type == null)
            {
                m_PrepareData = null;
                return;
            }
            configId = config.ID;
            if (m_PrepareData == null || m_PrepareData.GetType() != type)
            {
                m_PrepareData = System.Activator.CreateInstance(type) as IInitializationComponentData;
            }
        }
#endif
        #endregion
        
        [SerializeField, HideInInspector] private ObjectID configId;

        [SerializeReference] 
        private IInitializationComponentData m_PrepareData;

        public ObjectID ConfigId => configId;
        
        public void SetupPrepareData(EntityCommandBuffer ecb, Entity spawnPointEntity)
        {
            m_PrepareData?.Initialization(m_Container, ecb, spawnPointEntity);
        }
        
        public void Initialization(IView view)
        {
        }
    }
}

