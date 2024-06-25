using System.Collections.Generic;
using System.Linq;

using Game.Core.Spawns;

using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Serialization;

namespace Game
{
    public class SerializeManager
    {
        private List<ComponentTypeSet> m_AddedComponents;
        private EntityQuery? m_Query;
        private World m_SerializationWorld;
            
        public SerializeManager()
        {
            m_SerializationWorld = new World("Serialization World", WorldFlags.Staging | WorldFlags.Streaming);// WorldFlags.Streaming

            m_AddedComponents = new List<ComponentTypeSet>();
            m_AddedComponents.Add(new ComponentTypeSet(typeof(Spawn), typeof(Spawn.WithDataTag)));
        }

        private void NeedQuery()
        {
            if (m_Query.HasValue) return;

            m_Query = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(
                new EntityQueryDesc {
                    All = new ComponentType[] {ComponentType.ReadWrite<TUserStorageDataTag>()},
                    Options = EntityQueryOptions.Default
                });
        }
        
        public void SerializeWorld(World world, BinaryWriter writer)
        {
            NeedQuery();
            
            var entities = m_Query!.Value.ToEntityArray(Allocator.Temp);

            m_SerializationWorld.EntityManager.DestroyEntity(m_SerializationWorld.EntityManager.UniversalQuery);
            m_SerializationWorld.EntityManager.CopyEntitiesFrom(world.EntityManager, 
                entities, CopyArchetype.Storage, entities);

            foreach (var componentSet in m_AddedComponents)
            {
                m_SerializationWorld.EntityManager.AddComponent(entities, componentSet);
            }
            
            SerializeUtility.SerializeWorld(m_SerializationWorld.EntityManager, writer);
        }

        public void DeserializeWorld(World world, BinaryReader reader)
        {
            NeedQuery();
            var transactionSerialization = m_SerializationWorld.EntityManager.BeginExclusiveEntityTransaction();
            try
            {
                transactionSerialization.EntityManager.DestroyEntity(transactionSerialization.EntityManager.UniversalQuery);
                SerializeUtility.DeserializeWorld(transactionSerialization, reader);
            }
            finally
            {
                m_SerializationWorld.EntityManager.EndExclusiveEntityTransaction();
            }
            world.EntityManager.DestroyEntity(m_Query!.Value);
            world.EntityManager.MoveEntitiesFrom(m_SerializationWorld.EntityManager);
        }
    }
}
