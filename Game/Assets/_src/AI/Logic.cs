using Common.Defs;

using Game.Core.Defs;

using Unity.Entities;

namespace Game.AI.GOAP
{
    public partial struct Logic : IComponentData, IEnableableComponent, IDefinable<Logic.LogicDef>, IDefinableCallback, IStorageData
    {
        private RefLink<LogicDef> m_RefLink;
        private LogicDef Def => m_RefLink.Value;

        public void SetDef(ref RefLink<LogicDef> link)
        {
            m_RefLink = link;
        }

        public void AddComponentData(Entity entity, IDefinableContext context)
        {
            context.AddComponentData(entity, this);
            context.AddComponentData(entity, new ChangeTag());
            //context.SetComponentEnabled<Logic>(entity, false);
            //context.AddBuffer<Plan>(entity);
            context.AddBuffer<WorldStateComponent>(entity);
            //var goals = context.AddBuffer<Goal>(entity);
            //foreach (var iter in Def.Goals)
            //    goals.Add(iter);

            var buff = context.AddBuffer<WorldStateComponent>(entity);
            Def.SetupWorldStates(ref buff);
        }
        
        public struct ChangeTag : IComponentData{}
    }
}
