using System;
using System.Collections.Generic;

using Common.Core;

using Unity.Entities;

using UnityEngine;

namespace Game.Model
{
    using Core.Defs;

    /// <summary>
    /// Тип урона
    /// </summary>
    [CreateAssetMenu(fileName = "DamageType", menuName = "Configs/DamageType")]
    public class DamageConfig : GameObjectConfig
    {
        public DamageTargets Targets;
        [SerializeReference, ReferenceSelect(typeof(IDamage))]
        public List<IDamage> Damages = new List<IDamage>();
        public override ComponentTypeSet GetComponentTypeSet() => new ComponentTypeSet();
    }
}