using System.Collections.Generic;

using Game.AI.GOAP;

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
        public Unit.Def value = new();
        
        public Logic.LogicDef logic = new ();

        public override void Configure(Entity prefab, EntityManager manager, IDefinableContext context)
        {
            base.Configure(prefab, manager, context);
            value.AddComponentData(prefab, manager, context);
            logic.AddComponentData(prefab, manager, context);
        }

        IEnumerable<ChildConfig> IConfigContainer.Childs => value.Parts;

        void IConfigStats.Configure(DynamicBuffer<Stat> stats)
        {
            Stat.AddStat<Stat.Health>(stats, value.Health.Value);
            Stat.AddStat<Unit.Speed>(stats, value.Speed.Value);
        }

        public override void OnAfterDeserialize()
        {
            base.OnAfterDeserialize();
            logic.Initialize();
        }
        
        public override void Configure(IView view, Entity entity, EntityManager manager)
        {
            value.InitializationView(view, entity, manager);
        }

        public override ComponentTypeSet GetComponentTypeSet() => new (ComponentType.FromTypeIndex(value.GetTypeIndexDefinable()), ComponentType.FromTypeIndex(logic.GetTypeIndexDefinable()));
    }
}
