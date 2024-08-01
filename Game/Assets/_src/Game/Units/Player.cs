using System;

using Common.Core;

using Unity.Entities;

using Common.Defs;

using Game.Core.Defs;
using Game.Core.Spawns;
using Game.Views;

using Unity.Transforms;

namespace Game.Model.Units
{
    [Serializable]
    public struct Player : IUnit, IDefinable<Unit.Def>, IComponentData, IDefinableCallback
    {
        public RefLink<Unit.Def> RefLink { get; private set; }

        public void SetDef(ref RefLink<Unit.Def> link)
        {
            RefLink = link;
        }
        
        #region IDefineableCallback
        public void AddComponentData(Entity entity, IDefinableContext context)
        {
            context.AddComponentData(entity, this);
            context.AddComponentData(entity, new Move());
            context.AddComponentData(entity, new Spawn.ViewTag());
            context.AddComponentData(entity, new LocalTransform());
            context.AddComponentData(entity, new LocalToWorld());
        }

        public void InitializationView(IView view, Entity entity)
        {
            view.Transform.gameObject.AddComponent<PlayerCameraComponent>();
        }
        #endregion
    }
}