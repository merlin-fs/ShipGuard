using UnityEngine;

namespace Game.AI.GOAP
{
    public partial struct Logic
    {
        public interface ILogicConfig
        {
            void Initialize(LogicDef def);
        }
    }
}
