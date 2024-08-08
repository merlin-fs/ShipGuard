using System.Collections.Generic;

namespace Game.AI.GOAP
{
    public partial struct Logic
    {
        public partial class LogicDef
        {
            private readonly List<GOAPAction> m_Actions = new ();

            public ConfigTransition AddTransition<T>()
                where T: IAction
            {
                return ConfigTransition.Create(typeof(T), action => m_Actions.Add(action));
            }
        }
    }
}
