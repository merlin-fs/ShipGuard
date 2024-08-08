using Game.Core.Defs;

using Unity.Entities;

namespace Game.AI.GOAP
{
    public class LogicConfig : GameObjectConfig
    {
        public Logic.LogicDef Logic = new ();
        
        public override void Configure(Entity entity, EntityManager manager, IDefinableContext context)
        {
            base.Configure(entity, manager, context);
            Logic.AddComponentData(entity, manager, context);
        }

        public override void OnAfterDeserialize()
        {
            base.OnAfterDeserialize();
            Logic.Initialize();
        }
        public override ComponentTypeSet GetComponentTypeSet() => new (ComponentType.FromTypeIndex(Logic.GetTypeIndexDefinable()));
    }
}
