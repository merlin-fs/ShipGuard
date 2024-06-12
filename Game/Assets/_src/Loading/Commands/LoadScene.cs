using System;
using System.Threading.Tasks;

using Common.Core.Loading;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace Game.Core.Loading
{
    public class LoadScene : ILoadingCommand, IProgress<float>
    {
        [SerializeField] private AssetReference sceneRef;
        [SerializeField] private LoadSceneMode loadMode = LoadSceneMode.Single;
        
        private float m_Progress;

        public float GetProgress()
        {
            return m_Progress;
        }

        public LoadScene(){}

        public LoadScene(Guid sceneGuid, LoadSceneMode mode = LoadSceneMode.Single)
        {
            sceneRef = new AssetReference(sceneGuid.ToString());
            loadMode = mode;
        }
        
        public Task Execute(ILoadingManager manager)
        {
            return UniTask.RunOnThreadPool(async () =>
            {
                await UniTask.SwitchToMainThread();
                /*
                if (loadMode == LoadSceneMode.Single && 
                    SceneManager.GetActiveScene().IsValid() && 
                    SceneManager.GetActiveScene().name == this.sceneName)
                    return;
                */
                await Addressables.LoadSceneAsync(sceneRef, loadMode).Task;
                //await SceneManager.LoadSceneAsync(this.sceneName, loadMode).ToUniTask(this);
            }).AsTask();
        }

        public void Report(float value)
        {
            m_Progress = value;
        }
    }
}