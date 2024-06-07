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

        public class ContainerReference : IComponentData
        {
            public IContainer Value;
        }

        [UpdateInGroup(typeof(GameLogicSystemGroup))]
        public partial struct PlacementEntityToView : ISystem
        {
            private EntityQuery m_Query;

            public void OnCreate(ref SystemState state)
            {
                m_Query = SystemAPI.QueryBuilder()
                    .WithAll<LocalTransform, ContainerReference>()
                    .WithNone<ViewToEntityTag>()
                    .Build();
                m_Query.SetChangedVersionFilter(ComponentType.ReadOnly<LocalTransform>());
                state.RequireForUpdate(m_Query);
            }

            public void OnUpdate(ref SystemState state)
            {
                foreach (var (transform, context) in SystemAPI
                             .Query<RefRO<LocalTransform>, ContainerReference>()
                             .WithChangeFilter<LocalTransform>())
                {
                    var view = context.Value.Resolve<IView>();
                    view.Transform.localPosition = transform.ValueRO.Position;
                    view.Transform.localRotation = transform.ValueRO.Rotation;
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
                    .WithAll<ViewToEntityTag, LocalTransform, ContainerReference>()
                    .Build();
                m_Query.SetChangedVersionFilter(ComponentType.ReadOnly<LocalTransform>());
                state.RequireForUpdate(m_Query);
            }

            public void OnUpdate(ref SystemState state)
            {
                foreach (var (transform, context) in SystemAPI
                             .Query<RefRW<LocalTransform>, ContainerReference>())
                {
                    var view = context.Value.Resolve<IView>();
                    transform.ValueRW.Position = view.Transform.localPosition;
                    transform.ValueRW.Rotation = view.Transform.localRotation;
                }
            }
        }
    }
}
