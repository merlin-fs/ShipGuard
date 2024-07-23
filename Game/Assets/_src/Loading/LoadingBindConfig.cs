using System.Collections.Generic;

using Common.Core.Loading;

using Game.UI;

using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.Core.Loading
{
    public class LoadingBindConfig : ScriptableObject
    {
        [SerializeField] private AssetReference mainSceneRef;
        
        public IEnumerable<ICommandItem> GetCommands()
        {
            return new ICommandItem[] {
                //0
                new CommandLoadConfigRepositories("defs").AsItem(),
                //1
                new CommandLoadPrefabRepositories("defs").AsItem(),
                //2
                new CommandLoadScene(mainSceneRef).AsItem(),
                //3
                new CommandUiShow<UI.Loading>(UILayer.Loading).AsItem(2),
                //4
                new CommandCreateEntitiesWorld().AsItem(0, 1, 2),
                //5
                new CommandLoadEntitiesPrefab().AsItem(4),
                //6
                new CommandStartGameInitialization().AsItem(5),
                //7
                new CommandUiShow<MainMenu>(UILayer.Main).AsItem(6),
                //8
                new CommandUiHide<UI.Loading>().AsItem(6),
            };
        }
    }
}
