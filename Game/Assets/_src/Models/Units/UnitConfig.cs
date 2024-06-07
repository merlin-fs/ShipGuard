using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

using Game.Core.Defs;
using Game.Model.Stats;

namespace Game.Model.Units
{
    [CreateAssetMenu(fileName = "Unit", menuName = "Configs/Unit")]
    public class UnitConfig: GameObjectConfig, IConfigContainer, IConfigStats
    {
        public Unit.UnitDef Value = new Unit.UnitDef();

        protected override void Configure(Entity prefab, IDefinableContext context)
        {
            base.Configure(prefab, context);
            Value.AddComponentData(prefab, context);
        }

        IEnumerable<ChildConfig> IConfigContainer.Childs => Value.Parts;

        void IConfigStats.Configure(DynamicBuffer<Stat> stats)
        {
            Stat.AddStat<Stat.Health>(stats, Value.Health.Value);
            Stat.AddStat<Unit.Speed>(stats, Value.Speed.Value);
        }
    }
}
