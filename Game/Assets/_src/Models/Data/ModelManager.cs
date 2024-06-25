
using Unity.Entities;
using Unity.Entities.Serialization;
using Unity.Scenes;

using UnityEditor.Overlays;

using UnityEngine;

namespace Game.Model.Datas
{
    public class ModelManager
    {
        public void RegistryModel<T>()
            where T : IComponentData, IStorageData
        {
            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            entityManager.World.CreateSystem<ReactiveSystem<T>>();
        }

        public Entity CreateData<T>()
            where T : IComponentData, IStorageData
        {
            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var entity = entityManager.CreateEntity(new ComponentType[] 
            {
                typeof(ModelDataTag<T>),
                typeof(ModelDataChangeTag<T>),
                typeof(ModelDataEvent),
                typeof(StoreModel),
            });
            return entity;
        }
        
        private EntityManager m_EntityManager;
        public void SaveData()
        {
            //m_EntityManager.GetComponentDataRW<TUserStorageDataTag>()
        }
        
        public void save()
        {
            /*
            SerializeUtilityHybrid.Serialize();
            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            SerializeUtility.SerializeWorldIntoYAML
            using (var writer = new Unity.Entities.Serialization.StreamBinaryWriter(Application.dataPath + "/save4/saveNew.dat"))
            {
                SerializeUtility.SerializeWorld(entityManager, writer, out object[] g2);
            }
            */
        }        
    }
}
