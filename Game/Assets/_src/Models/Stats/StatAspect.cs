using Unity.Entities;

using Unity.Collections;

namespace Game.Model.Stats
{
    public readonly partial struct StatAspect : IAspect
    {
        private readonly Entity m_Self;

        private readonly DynamicBuffer<Stat> m_Items;
        [ReadOnly]
        private readonly DynamicBuffer<Modifier> m_Modifiers;

        public Entity Self => m_Self;
        public DynamicBuffer<Stat> Values => m_Items;

        public void Estimation(float delta)
        {
            for (int i = 0; i < m_Items.Length; i++)
            {
                Modifier.Estimation(Self, ref m_Items.ElementAt(i), m_Modifiers, delta);
            }
        }
        
    /*
        #region DesignTime

#if UNITY_EDITOR
        [CreateProperty]
        public readonly List<Stat> StatsNames => m_Items.AsNativeArray().ToArray().ToList();//Select(i => i.StatName)

#endif
        #endregion
        */
    }
}