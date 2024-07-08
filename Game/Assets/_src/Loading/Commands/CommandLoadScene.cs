using System;
using System.Threading.Tasks;

using Common.Core;
using Common.Core.Loading;

using Cysharp.Threading.Tasks;

using Game.Model.Locations;

using Reflex.Attributes;
using Reflex.Extensions;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace Game.Core.Loading
{
    public class CommandLoadScene : ICommandProgress, ICommandNewContainer, IProgress<float>
    {
        [SerializeField] private AssetReference sceneRef;
        [SerializeField] private LoadSceneMode loadMode = LoadSceneMode.Single;

        private float m_Progress;
        private IContainer m_Container;

        public CommandLoadScene(){}

        public CommandLoadScene(Guid sceneGuid, LoadSceneMode mode = LoadSceneMode.Single)
        {
            sceneRef = new AssetReference(sceneGuid.ToString());
            loadMode = mode;
        }

        public CommandLoadScene(AssetReference sceneRef, LoadSceneMode mode = LoadSceneMode.Single)
        {
            this.sceneRef = sceneRef;
            loadMode = mode;
        }
        
        public float GetProgress()
        {
            return m_Progress;
        }

        public IContainer GetContainer()
        {
            return m_Container;
        }

        public Task Execute()
        {
            return UniTask.RunOnThreadPool(async () =>
            {
                await UniTask.SwitchToMainThread();

                var task = Addressables.LoadSceneAsync(sceneRef, loadMode);
                task.Completed += handle =>
                {
                    m_Container = handle.Result.Scene.GetSceneContainer();
                };
                
                await task.Task;
            }).AsTask();
        }

        public void Report(float value)
        {
            m_Progress = value;
        }
    }
}