using System.Collections.Generic;
using Common.Core.Loading;
using UnityEngine;

namespace Game.Core.Loading
{
    public class LoadingConfig : ScriptableObject
    {
        [SerializeField] private List<LoadingManager.CommandItem> commands;
        public IEnumerable<LoadingManager.CommandItem> GetCommands() => commands;
    }
}
