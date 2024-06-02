using System;
using System.Linq;
using System.Collections.Generic;

using Common.Core.Loading;

using UnityEngine;

namespace Common.Core.Progress
{
    public class MultiProgress : IProgressWritable
    {
        private bool m_Done;
        private readonly Dictionary<ILoadingCommand, float> m_Progress = new();
        private event IProgress.OnProgressChange OnProgressChange;
        private IProgressWritable Self => this;

        public MultiProgress(params ILoadingCommand[] objects)
        {
            foreach(var iter in objects)
                m_Progress.Add(iter, 0);
        }


        public void SetProgress(ILoadingCommand obj, float value)
        {
            if (!m_Progress.TryGetValue(obj, out float objValue)) return;
            value = Mathf.Clamp(value, 0, 1);
            if (objValue.CompareTo(value) == 0) return;
            
            m_Progress[obj] = value;
            OnProgressChange?.Invoke(Self.Value);
            Debug.Log($"Progress: {Self.Value}");
        }

        public void SetDone()
        {
            Self.SetDone();
        }

        #region IProgress
        float IProgressWritable.SetDone()
        {
            m_Done = true;
            return Self.Value;
        }

        float IProgress.Value
        {
            get
            {
                if (m_Done) return 1;
                if (m_Progress.Count == 0) return 0;

                return m_Progress.Values.Sum() / m_Progress.Count;
            }
        }

        float IProgressWritable.SetProgress(float value)
        {
            return Self.Value;
        }

        event IProgress.OnProgressChange IProgress.OnChange
        {
            add => OnProgressChange += value;
            remove => OnProgressChange -= value;
        }
        #endregion
    }
}
