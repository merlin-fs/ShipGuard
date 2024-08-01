using System;
using System.Collections.Generic;

using Game.Core.Spawns;

using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Serialization;

namespace Game.Storages
{
    public class Serializer: IDisposable
    {
        private readonly List<ComponentTypeSet> m_AddedComponents;
        private EntityQuery m_Query;
        private readonly World m_SerializationWorld;

        public static Serializer Setup<T>()
            where T : IStorageContainerType
        {
            return new Serializer(typeof(T));
        }

        public static Serializer Setup(Type storageContainerType)
        {
            return new Serializer(storageContainerType);
        }
        
        private Serializer(Type storageContainerType)
        {
            m_SerializationWorld = new World(storageContainerType.Name, WorldFlags.Staging | WorldFlags.Streaming);

            m_AddedComponents = new List<ComponentTypeSet>();
            m_AddedComponents.Add(new ComponentTypeSet(typeof(Spawn), typeof(Spawn.WithDataTag), typeof(Spawn.WaitSpawnTag)));
            m_Query = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(
                new EntityQueryDesc {
                    All = new ComponentType[] { storageContainerType },
                    Options = EntityQueryOptions.Default
                });
        }

        public void SerializeWorld(World world, BinaryWriter writer)
        {
            using var entities = m_Query.ToEntityArray(Allocator.Temp);
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
            var transactionSerialization = m_SerializationWorld.EntityManager.BeginExclusiveEntityTransaction();
            try
            {
                SerializeUtility.DeserializeWorld(transactionSerialization, reader);
            }
            finally
            {
                m_SerializationWorld.EntityManager.EndExclusiveEntityTransaction();
            }
            //world.EntityManager.CopyEntitiesFrom(m_SerializationWorld.EntityManager);
            using var entities = m_SerializationWorld.EntityManager.UniversalQuery.ToEntityArray(Allocator.Temp);

            world.EntityManager.CopyEntitiesFrom(m_SerializationWorld.EntityManager, 
                entities, CopyArchetype.Original, entities);
            
            //!!world.EntityManager.MoveEntitiesFrom(m_SerializationWorld.EntityManager);
        }

        public void Dispose()
        {
            //m_SerializationWorld?.Dispose();
        }
    }
}
