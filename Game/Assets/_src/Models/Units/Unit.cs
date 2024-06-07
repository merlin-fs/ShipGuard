using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

using Common.Defs;

using Game.Core;
using Game.Core.Defs;
using Game.Model.Stats;

namespace Game.Model.Units
{
    [Serializable]
    public partial struct Unit : IUnit, IDefinable, IComponentData, IDefinableCallback
    {
        public RefLink<UnitDef> RefLink { get; }

        public Unit(RefLink<UnitDef> config)
        {
            RefLink = config;
        }
        
        #region IDefineableCallback
        public void AddComponentData(Entity entity, IDefinableContext context)
        {
            context.AddComponentData(entity, new Move());
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
        public class UnitDef : IDef<Unit>
        {
            public List<ChildConfig> Parts = new List<ChildConfig>();

            [SerializeReference, ReferenceSelect(typeof(IStatValue))]
            public IStatValue Speed;

            [SerializeReference, ReferenceSelect(typeof(IStatValue))]
            public IStatValue Health;
        }
    }
}