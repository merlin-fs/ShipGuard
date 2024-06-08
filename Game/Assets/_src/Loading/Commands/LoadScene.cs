using System;
using System.Threading.Tasks;

using Common.Core.Loading;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Core.Loading
{
    public class LoadScene : ILoadingCommand, IProgress<float>
    {
        [SerializeField] private string sceneName;
        [SerializeField] private LoadSceneMode loadMode = LoadSceneMode.Single;
        
        private float m_Progress;

        public float GetProgress()
        {
            return m_Progress;
        }

        public LoadScene(){}

        public LoadScene(string name, LoadSceneMode mode = LoadSceneMode.Single)
        {
            sceneName = name;
            loadMode = mode;
        }
        
        public Task Execute(ILoadingManager manager)
        {
            return UniTask.RunOnThreadPool( async () =>
            {
                await UniTask.SwitchToMainThread();
                await SceneManager.LoadSceneAsync(this.sceneName, loadMode).ToUniTask(this);
            }).AsTask();
        }

        public void Report(float value)
        {
            m_Progress = value;
        }
    }
}