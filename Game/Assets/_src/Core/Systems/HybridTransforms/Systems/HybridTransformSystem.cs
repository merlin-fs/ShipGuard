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
        public struct EntityToViewTag : IComponentData { }

        public class ReferenceView : IComponentData
        {
            public IView Value;
        }

        [UpdateInGroup(typeof(SimulationSystemGroup))]
        [UpdateAfter(typeof(TransformSystemGroup))]
        public partial struct PlacementEntityToView : ISystem
        {
            private EntityQuery m_Query;

            public void OnCreate(ref SystemState state)
            {
                m_Query = SystemAPI.QueryBuilder()
                    .WithAll<LocalToWorld, ReferenceView>()//, EntityToViewTag
                    .Build();
                m_Query.SetChangedVersionFilter(ComponentType.ReadOnly<LocalToWorld>());
                state.RequireForUpdate(m_Query);
            }

            public void OnUpdate(ref SystemState state)
            {
                foreach (var (transform, referenceView) in SystemAPI
                             .Query<RefRO<LocalToWorld>, ReferenceView>()
                             .WithAll<ReferenceView>()
                             .WithNone<ViewToEntityTag>()
                             .WithChangeFilter<LocalToWorld>())
                {
                    if (referenceView == null) continue;
                    
                    referenceView.Value.Transform.localPosition = transform.ValueRO.Position;
                    referenceView.Value.Transform.localRotation = transform.ValueRO.Rotation;
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
                    .WithAll<ViewToEntityTag, LocalTransform, ReferenceView>()
                    .Build();
                m_Query.SetChangedVersionFilter(ComponentType.ReadOnly<LocalTransform>());
                state.RequireForUpdate(m_Query);
            }

            public void OnUpdate(ref SystemState state)
            {
                foreach (var (transform, view) in SystemAPI
                             .Query<RefRW<LocalTransform>, ReferenceView>()
                             .WithAll<ViewToEntityTag>())
                {
                    transform.ValueRW.Position = view.Value.Transform.localPosition;
                    transform.ValueRW.Rotation = view.Value.Transform.localRotation;
                }
            }
        }
    }
}
