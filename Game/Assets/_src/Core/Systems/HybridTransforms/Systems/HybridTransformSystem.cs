using Common.Core;

using Game.Core.Defs;
using Game.Views;

using Unity.Entities;
using Unity.Transforms;

namespace Game.Core.HybridTransforms
{
    public partial struct HybridTransform
    {
        public struct ViewToEntityTag : IComponentData { }

        public class ViewReference : IComponentData
        {
            public IView Value;
        }

        [UpdateInGroup(typeof(GameLogicSystemGroup))]
        public partial struct PlacementEntityToView : ISystem
        {
            private EntityQuery m_Query;

            public void OnCreate(ref SystemState state)
            {
                m_Query = SystemAPI.QueryBuilder()
                    .WithAll<LocalTransform, ViewReference>()
                    .WithNone<ViewToEntityTag>()
                    .Build();
                m_Query.SetChangedVersionFilter(ComponentType.ReadOnly<LocalTransform>());
                state.RequireForUpdate(m_Query);
            }

            public void OnUpdate(ref SystemState state)
            {
                foreach (var (transform, view) in SystemAPI
                             .Query<RefRO<LocalTransform>, ViewReference>()
                             .WithChangeFilter<LocalTransform>())
                {
                    view.Value.Transform.localPosition = transform.ValueRO.Position;
                    view.Value.Transform.localRotation = transform.ValueRO.Rotation;
                }
            }
        }

        [UpdateInGroup(typeof(GamePresentationSystemGroup))]
        public partial struct PlacementViewToEntity : ISystem
        {
            private EntityQuery m_Query;

            public void OnCreate(ref SystemState state)
            {
                m_Query = SystemAPI.QueryBuilder()
                    .WithAll<ViewToEntityTag, LocalTransform, ViewReference>()
                    .Build();
                m_Query.SetChangedVersionFilter(ComponentType.ReadOnly<LocalTransform>());
                state.RequireForUpdate(m_Query);
            }

            public void OnUpdate(ref SystemState state)
            {
                foreach (var (transform, view) in SystemAPI
                             .Query<RefRW<LocalTransform>, ViewReference>())
                {
                    transform.ValueRW.Position = view.Value.Transform.localPosition;
                    transform.ValueRW.Rotation = view.Value.Transform.localRotation;
                }
            }
        }
    }
}
