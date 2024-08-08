using Game.AI.GOAP;

using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

namespace Game.Model.AI
{
    public struct FindScanner : Logic.IAction
    {
        public void Execute(Logic.ExecuteContext context, EntityCommandBuffer.ParallelWriter ecb, int sortKey, Entity entity)
        {
            if (!context.EntityManager.HasBuffer<Target.PossibleTargets>(entity)) return;
            if (!context.EntityManager.HasComponent<Target.Query>(entity)) return;
            if (!context.EntityManager.HasComponent<LocalToWorld>(entity)) return;

            var possibleTargets = context.EntityManager.GetBuffer<Target.PossibleTargets>(entity, true).Reinterpret<Entity>();
            var findQuery = context.EntityManager.GetComponentData<Target.Query>(entity);
            var selfTransform = context.EntityManager.GetComponentData<LocalToWorld>(entity);

            CandidateTarget candidateTarget = new CandidateTarget { Value = Entity.Null, Magnitude = float.MaxValue};

            foreach (var candidate in possibleTargets)
            {
                if (!context.EntityManager.HasComponent<LocalToWorld>(candidate)) return;
                var transform = context.EntityManager.GetComponentData<LocalToWorld>(candidate);

                var magnitude = (selfTransform.Position - transform.Position).magnitude();

                if (magnitude < candidateTarget.Magnitude &&
                    utils.SpheresIntersect(selfTransform.Position, findQuery.Radius, transform.Position, 5f, out _))
                {
                    candidateTarget.Magnitude = magnitude;
                    candidateTarget.Value = candidate;
                }
            }

            if (candidateTarget.Value == Entity.Null) return;
            ecb.SetComponent(sortKey, entity, new Target{Value = candidateTarget.Value});
            context.SetWorldState(entity, Target.State.Found, true);
        }

        private struct CandidateTarget
        {
            public float Magnitude;
            public Entity Value;
        }
    }
}
