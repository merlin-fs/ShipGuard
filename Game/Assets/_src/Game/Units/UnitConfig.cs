using System.Collections.Generic;

using Unity.Entities;
using UnityEngine;

using Game.Core.Defs;
using Game.Model.Stats;
using Game.Views;

namespace Game.Model.Units
{
    [CreateAssetMenu(fileName = "Unit", menuName = "Configs/Unit")]
    public class UnitConfig: GameObjectConfig, IConfigContainer, IConfigStats
    {
        public Unit.Def Value = new();

        protected override void Configure(Entity prefab, EntityManager manager, IDefinableContext context)
        {
            base.Configure(prefab, manager, context);
            Value.AddComponentData(prefab, manager, context);
        }

        IEnumerable<ChildConfig> IConfigContainer.Childs => Value.Parts;

        void IConfigStats.Configure(DynamicBuffer<Stat> stats)
        {
            Stat.AddStat<Stat.Health>(stats, Value.Health.Value);
            Stat.AddStat<Unit.Speed>(stats, Value.Speed.Value);
        }

        public override void Configure(IView view, Entity entity, EntityManager manager)
        {
            Value.InitializationView(view, entity, manager);
        }

        public override ComponentTypeSet GetComponentTypeSet() => new (ComponentType.FromTypeIndex(Value.GetTypeIndexDefinable()));
    }
}
