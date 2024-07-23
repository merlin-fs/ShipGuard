using System;

namespace Game.Model
{
    using Stats;

    public class DamageHealth : Damage
    {
        public override void Apply(ref StatAspect stat, float value)
        {
            //stat.GetRW(Global.Stats.Health).Damage(value);
        }
    }
}   