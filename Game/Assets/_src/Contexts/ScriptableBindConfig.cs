using Reflex.Core;

using UnityEngine;

namespace Game.Core.Contexts
{
    public class ScriptableBindConfig : ScriptableObject
    {
        public void Bind(ContainerBuilder containerBuilder) => containerBuilder.AddSingleton(this);
    }
}
