using System.Collections.Generic;
using System.Linq;

namespace Game.AI.GOAP
{
    public partial struct Logic
    {
        public partial class LogicDef
        {
            private class GOAPGraph
            {
                private readonly Dictionary<GOAPAction, List<GOAPAction>> m_Neighbors = new();
                private readonly Dictionary<GOAPAction, float> m_ActionCosts = new();

                private void AddAction(GOAPAction action)
                {
                    if (m_Neighbors.ContainsKey(action)) return;

                    m_Neighbors[action] = new List<GOAPAction>();
                    m_ActionCosts[action] = action.Cost;
                }

                private void AddEdge(GOAPAction from, GOAPAction to)
                {
                    if (m_Neighbors.ContainsKey(from))
                    {
                        m_Neighbors[from].Add(to);
                    }
                }

                public float ActionCosts(GOAPAction action) => m_ActionCosts[action];

                public List<GOAPAction> GetNeighbors(GOAPAction action)
                {
                    return m_Neighbors.ContainsKey(action) ? m_Neighbors[action] : new List<GOAPAction>();
                }

                public static GOAPGraph BuildGraph(List<GOAPAction> actions)
                {
                    var graph = new GOAPGraph();

                    foreach (var action in actions)
                    {
                        graph.AddAction(action);
                    }

                    foreach (var action in actions)
                    {
                        foreach (var otherAction in actions)
                        {
                            if (action != otherAction && CanTransition(action, otherAction))
                            {
                                graph.AddEdge(action, otherAction);
                            }
                        }
                    }

                    bool CanTransition(GOAPAction from, GOAPAction to)
                    {
                        // Проверка, можно ли перейти от одного действия к другому на основе префиксов и эффектов
                        return to.Preconditions.Any(precondition => @from.Effects.Contains(precondition));
                    }

                    return graph;
                }
            }
        }
    }
}
