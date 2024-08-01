using Common.Core;

using Game.Model.Locations;

using Unity.Entities;

using UnityEngine;

namespace Game.Views
{
    public class SetupConfigComponent : MonoBehaviour, IViewComponent
#if UNITY_EDITOR
        , ISerializationCallbackReceiver
#endif
    {
#if UNITY_EDITOR
        [SerializeField] private PointConfig configPoint;
#endif
        [SerializeField, HideInInspector] private ObjectID configId;

        [SerializeField, SerializeReference] 
        private ILocationItem m_PrepareData;

        public ObjectID ConfigId => configId;

        public void Initialization(IView view)
        {
            //throw new System.NotImplementedException();
        }

        #region ISerializationCallbackReceiver

#if UNITY_EDITOR
        public void OnBeforeSerialize()
        {
            if (!configPoint) return;
            configId = configPoint.ID;
            var type = ComponentType.FromTypeIndex(configPoint.Value.GetTypeIndexDefinable()).GetManagedType();
            if (m_PrepareData == null || m_PrepareData.GetType() != type)
            {
                m_PrepareData = System.Activator.CreateInstance(type) as ILocationItem;
            }
                 
        }

        public void OnAfterDeserialize() { }
#endif

        #endregion
    }
}

