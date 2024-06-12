using System;

using Game.Model.Locations;

using Reflex.Attributes;
using Reflex.Core;

using UnityEngine;

namespace Game.Core.Contexts
{
    public class LocationSceneContext : MonoBehaviour, IInstaller
    {
        [SerializeField] private LocationRoot locationRoot;
        
        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            containerBuilder.AddSingleton(locationRoot);
        }
    }
}