using System;

using Common.Core;

using Unity.Entities;

namespace Game.Core.Defs
{
    [Serializable]
    public partial struct PrefabInfo: IComponentData
    {
        private ObjectID m_ConfigID;
        public ObjectID ConfigID { get => m_ConfigID; set => m_ConfigID = value; }
    }
}
