using System.Collections.Generic;

namespace Game.AI.GOAP
{
    public class GOAPAction
    {
        public LogicActionHandle Action;
        public List<LogicActionHandle> QueryActions = new();
        public List<LogicActionHandle> ResultActions = new();
        
        public List<WorldStateHandle> Preconditions = new();
        public List<WorldStateHandle> Effects = new();
        public float Cost;
    }
}
