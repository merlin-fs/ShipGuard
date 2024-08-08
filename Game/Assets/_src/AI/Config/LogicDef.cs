using System;
using System.Collections.Generic;

using Common.Defs;

using Unity.Entities;

using UnityEngine;

namespace Game.AI.GOAP
{
    public partial struct Logic
    {
        [Serializable]
        public partial class LogicDef: IDef
        {
            //TODO: временное поле, будет заменено на конфиг
            [SerializeReference, ReferenceSelect(typeof(ILogicConfig))]
            private ILogicConfig logicInst;

            private GOAPGraph m_Graph;

            public int GetTypeIndexDefinable() => ((ComponentType)typeof(Logic)).TypeIndex;
            
            public void Initialize()
            {
                InitInst();
            }

            public float ActionCosts(GOAPAction action) => m_Graph.ActionCosts(action);

            public IEnumerable<GOAPAction> GetNeighbors(GOAPAction action) => m_Graph.GetNeighbors(action);

            private void InitInst()
            {
                logicInst?.Initialize(this);
                m_Graph = GOAPGraph.BuildGraph(m_Actions);
                WorldStatesInitialization();
            }
        }
    }
}
