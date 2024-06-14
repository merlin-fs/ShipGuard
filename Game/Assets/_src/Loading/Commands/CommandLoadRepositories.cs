using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using Common.Core;
using Common.Core.Loading;

using Cysharp.Threading.Tasks;

using Game.Core.Defs;
using Game.Core.Repositories;

using JetBrains.Annotations;

using Reflex.Attributes;

using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Game.Core.Loading
{
    public abstract class CommandLoadRepositories<T> : ICommandProgress, IProgress<float>
    {
        [SerializeField] private string label;
        
        [Inject] private ObjectRepository m_ObjectRepository;

        private float m_Progress;

        void IProgress<float>.Report(float value) => m_Progress = value;
        
        public float GetProgress()
        {
            return m_Progress; 
        }

        public CommandLoadRepositories(string label) => this.label = label;
        
        protected abstract AsyncOperationHandle<IList<T>> GetAsyncOperationHandle(IEnumerable keys);
        [NotNull]
        protected abstract IEnumerable<IConfig> CastToConfig(IEnumerable<T> result);
        
        public Task Execute()
        {
            return UniTask.Create(async () =>
            {
                await UniTask.SwitchToMainThread();
                var asyncOperationHandle = GetAsyncOperationHandle(label);
                await asyncOperationHandle
                    .ToUniTask(this)
                    .ContinueWith(async result =>
                    {
                        var configs = CastToConfig(result);
                        if (configs == null) return;
                        
                        m_ObjectRepository.Insert(configs, label);
                        foreach (var config in configs)
                            if (config is IViewPrefab viewPrefab)
                            {
                                await viewPrefab.PreloadPrefab();
                            }
                    });
            }).AsTask();
        }
    }
}