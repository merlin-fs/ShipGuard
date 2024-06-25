using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

using Common.Defs;

using Game.Core;
using Game.Core.Defs;
using Game.Core.Spawns;
using Game.Model.Stats;

namespace Game.Model.Units
{
    [Serializable]
    public partial struct Unit : IUnit, IDefinable<Unit.Def>, IComponentData, IDefinableCallback
    {
        public RefLink<Def> RefLink { get; private set; }

        public void SetDef(RefLink<Def> link)
        {
            RefLink = link;
        }
        
        #region IDefineableCallback
        public void AddComponentData(Entity entity, IDefinableContext context)
        {
            context.AddComponentData(entity, new Move());
            context.AddComponentData(entity, new Spawn.ViewTag());
            //TODO: доробити StorageData
            context.AddComponentData(entity, new TUserStorageDataTag());
        }

        public void RemoveComponentData(Entity entity, IDefinableContext context) { }
        #endregion
        
        [EnumHandle]
        public enum State
        {
            Stop,
            WeaponInRange,
        }

        #region Stats
        public struct Speed: IStat { }
        #endregion

        [Serializable]
        public class Def : IDef<Unit>
        {
            public List<ChildConfig> Parts = new List<ChildConfig>();

            [SerializeReference, ReferenceSelect(typeof(IStatValue))]
            public IStatValue Speed;

            [SerializeReference, ReferenceSelect(typeof(IStatValue))]
            public IStatValue Health;
        }
    }
}