using Game.Core.HybridTransforms;
using Game.Core.Repositories;
using Game.Model;

using Unity.Entities;

using Reflex.Attributes;

using Unity.Transforms;

namespace Game.Core.Spawns
{
    public partial struct Spawn
    {
        [UpdateInGroup(typeof(GameSpawnSystemGroup))]
        public partial struct DestroySystem : ISystem
        {
            private static string m_PrefabType;
            
            [Inject] private static ConfigRepository m_Repository;
            [Inject] private static GameUniqueEntityRepository m_GameUniqueEntityRepository;
            
            private EntityQuery m_Query; 
            
            public void OnCreate(ref SystemState state)
            {
                m_Query = SystemAPI.QueryBuilder()
                    .WithAll<DestroyTag>()
                    .Build();
            }

            public void OnUpdate(ref SystemState state)
            {
                if (m_Query.IsEmpty) return;
                
                var ecb = SystemAPI.GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>()
                    .CreateCommandBuffer(state.WorldUnmanaged);

                //Destroy view
                foreach (var viewReference in SystemAPI.Query<HybridTransform.ReferenceView>()
                             .WithAll<DestroyTag>())
                {
                    UnityEngine.Object.Destroy(viewReference.Value.Transform.gameObject);
                }

                // Destroy children
                foreach (var children in SystemAPI.Query<DynamicBuffer<Child>>()
                             .WithAll<DestroyTag>())
                {
                    ecb.AddComponent<DestroyTag>(children.AsNativeArray().Reinterpret<Entity>());
                }

                //Remove uniqueEntity
                foreach (var (uniqueEntity, entity) in SystemAPI.Query<UniqueEntity>()
                             .WithAll<DestroyTag>()
                             .WithEntityAccess())
                {
                    m_GameUniqueEntityRepository.Remove(uniqueEntity.ID);
                    ecb.DestroyEntity(entity);
                }

                //Destroy Entity
                foreach (var (_, entity) in SystemAPI.Query<DestroyTag>()
                             .WithEntityAccess())
                {
                    ecb.DestroyEntity(entity);
                }
            }
        }
    }
}