using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;

using Common.Core;
using Common.Repositories;

using Game.Core.Defs;

using Unity.Entities;

namespace Game.Core.Repositories
{
    public class ConfigRepository: Repository<ObjectID, IConfig, ConfigRepository.Attribute>
    {
        public class Attribute : IEntityAttributes<IConfig>
        {
            public IConfig Entity { get; }
            public HashSet<string> Labels { get; }

            public Attribute(IConfig entity, string[] labels)
            {
                Entity = entity;
                Labels = labels != null 
                    ? new HashSet<string>(labels) 
                    : new HashSet<string>();
            }
        }

        private readonly ConcurrentDictionary<string, byte> m_Labels = new ConcurrentDictionary<string, byte>();
        
        public IEnumerable<string> Labels => m_Labels.Keys;
        
        public void Insert(ObjectID id, IConfig config, params string[] labels)
        {
            foreach (var iter in labels)
                m_Labels[iter] = 0;
            m_Repo.Insert(id, new Attribute(config, labels));
        }

        public void Insert(IEnumerable<IConfig> configs, params string[] labels)
        {
            foreach (var iter in labels)
                m_Labels[iter] = 0;
            m_Repo.Insert(configs.Select(config => ( config.ID, new Attribute(config, labels) )).ToArray());
        }
    }
}
