using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Serialization;

using UnityEngine;

using Debug = UnityEngine.Debug;

namespace Game
{
    public struct StoreModel : IComponentData, IStorageData
    {
        
    }

    public struct StoreModelItem : IBufferElementData, IStorageData
    {
        public int Id;
    }
    
    
    public struct TestStorageData: IComponentData, IStorageData 
    {
        public int Value;
    }
    
    public class _test_SaveManager : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            
            var world = DefaultWorldInitialization.Initialize("Test save manager", false);
            
            var entityManager = world.EntityManager;
            var ecb = world.EntityManager.World.GetOrCreateSystemManaged<GameSpawnSystemCommandBufferSystem>()
                .CreateCommandBuffer();

            var type = entityManager.CreateArchetype(
                new ComponentType(typeof(TestStorageData)),
                new ComponentType(typeof(Unity.Transforms.LocalToWorld)),
                new ComponentType(typeof(Game.Core.Spawns.Spawn.ViewTag))
                );

            var serializeWorld = new World("Serialization World", WorldFlags.Staging | WorldFlags.Streaming);// WorldFlags.Streaming

            var entitiesCount = 1000;

            var stopwatch = Stopwatch.StartNew();
            var entities = new NativeArray<Entity>(entitiesCount, Allocator.Temp);
            Debug.Log($"Create array {entitiesCount} {stopwatch.Elapsed}");

            stopwatch.Restart();
            entityManager.CreateEntity(type, entities);
            Debug.Log($"CreateEntity {entitiesCount} {stopwatch.Elapsed}");
            entities.Dispose();

            var types = TypeManager.AllTypes.Where(iter =>
                iter.Category is TypeManager.TypeCategory.BufferData or TypeManager.TypeCategory.ComponentData
                    or TypeManager.TypeCategory.ISharedComponentData)
                .Select(iter => ComponentType.FromTypeIndex(iter.TypeIndex));

            types = types.Where(iter => !typeof(IStorageData).IsAssignableFrom(iter.GetManagedType()));
            
            IEnumerable<ComponentType> enumerable = types as ComponentType[] ?? types.ToArray();
            var count = enumerable.Count();
            var capacity = new FixedList64Bytes<TypeIndex>().Capacity;

            var listSets = new List<ComponentTypeSet>();
            for (int i = 0; i <= count / capacity; i++)
            {
                var componentTypes = enumerable
                    .Skip(i * capacity)
                    .Take(capacity).ToArray();
                var componentSet = new ComponentTypeSet(componentTypes);
                listSets.Add(componentSet);
            }
            
            EntityQuery query = entityManager.CreateEntityQuery(
                new EntityQueryDesc
                {
                    All = new ComponentType[] {ComponentType.ReadWrite<IStorageData>()}, 
                    Options = EntityQueryOptions.Default
                }
            );

            for (int i = 0; i < 10; i++)
            {

                var transaction = entityManager.BeginExclusiveEntityTransaction();
                var transactionSerialization = serializeWorld.EntityManager.BeginExclusiveEntityTransaction();
                //using (var serializeWorld = new World("Serialization World"))
                {
                    transactionSerialization.EntityManager.DestroyEntity(transactionSerialization.EntityManager.UniversalQuery);
                    stopwatch.Restart();
                    entities = query.ToEntityArray(Allocator.Temp);
                    Debug.Log($"query.ToEntityArray {entitiesCount} {stopwatch.Elapsed}");


                    stopwatch.Restart();
                    //transactionSerialization.EntityManager.MoveEntitiesFrom(transaction.EntityManager, query);
                    transactionSerialization.EntityManager.CopyEntitiesFrom(transaction.EntityManager, entities, CopyArchetype.Storage,
                        entities);
                    Debug.Log($"CopyEntitiesFrom {entitiesCount} {stopwatch.Elapsed}");
                    entityManager.EndExclusiveEntityTransaction();

                    stopwatch.Restart();
                    foreach (var componentSet in listSets)
                    {
                        transactionSerialization.EntityManager.RemoveComponent(entities, componentSet);
                    }

                    Debug.Log($"RemoveComponent {entitiesCount} {stopwatch.Elapsed}");
                    entities.Dispose();

                    stopwatch.Restart();
                    using (var writer = new StreamBinaryWriter($"{Application.persistentDataPath}/saveData.test"))
                    {
                        //SerializeUtility.DeserializeWorld();
                        SerializeUtility.SerializeWorld(transactionSerialization.EntityManager, writer);
                    }

                    serializeWorld.EntityManager.EndExclusiveEntityTransaction();

                    Debug.Log($"Serialization World {entitiesCount} {stopwatch.Elapsed}");
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
