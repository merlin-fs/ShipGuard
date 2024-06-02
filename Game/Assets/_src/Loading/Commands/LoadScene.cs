using System;
using System.Threading.Tasks;

using Common.Core.Loading;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Core.Loading
{
    public class LoadScene : ILoadingCommand
    {
        [SerializeField] private string scene;
        private float m_Progress;
        
        private AsyncOperation m_AsyncOperationHandle; 
        public float GetProgress()
        {
            return m_Progress;
        }

        public Task Execute(ILoadingManager manager)
        {
            return UniTask.Run( async () =>
            {
                await UniTask.SwitchToMainThread();
                var scene = SceneManager.GetActiveScene();
                if (scene.IsValid() && scene.name == this.scene)
                {
                    m_Progress = 1;
                    await UniTask.WaitUntil(() => scene.isLoaded);
                    return;
                }

                m_AsyncOperationHandle = SceneManager.LoadSceneAsync(this.scene, LoadSceneMode.Single);

                await UniTask.WaitUntil(() =>
                {
                    m_Progress = m_AsyncOperationHandle.progress;
                    return m_AsyncOperationHandle.isDone;
                });
            }).AsTask();
        }
    }
}