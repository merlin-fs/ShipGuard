using System;
using System.Threading.Tasks;

using Common.Core;
using Unity.Entities;

using UnityEngine;

namespace Common.Defs
{
    public interface IConfig: IIdentifiable<ObjectID>
    {
        Entity EntityPrefab { get; }
        void Configure(Entity root, IDefinableContext context);
    }
}
