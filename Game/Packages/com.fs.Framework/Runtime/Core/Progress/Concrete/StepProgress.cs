using System;
using System.Threading;

namespace Common.Core.Progress
{
    public class StepProgress: IProgressWritable
    {
        private bool m_Done;
        private float[] m_Steps;
        private int m_CurrentStep;
        private float m_Progress;
        private event IProgress.OnProgressChange m_OnProgressChange;
        private IProgressWritable Self => this;

        private StepProgress()
        {
            Clear();
        }

        public static StepProgress FromPercent(float[] percents)
        {
            var res = new StepProgress() 
            { 
                m_Steps = new float[percents.Length] 
            };
            Array.Copy(percents, res.m_Steps, res.m_Steps.Length);
            return res;

        }
        
        public static StepProgress FromWeight(float[] weights)
        {
            var res = new StepProgress() 
            { 
                m_Steps = new float[weights.Length] 
            };
            float summ = 0;
            foreach (var weight in weights)
                summ += weight;
            for (int i = 0; i < weights.Length; i++)
                res.m_Steps[i] = weights[i] / summ;
            return res;
        }

        public static StepProgress FromCount(int count)
        {
            var res = new StepProgress() 
            { 
                m_Steps = new float[count] 
            };
            for (int i = 0; i < count; i++)
                res.m_Steps[i] = 1f / count;
            return res;
        }

        private float Progress
        {
            get => m_Progress;
            set 
            {
                if (m_Progress != value)
                {
                    m_Progress = value;
                    m_OnProgressChange?.Invoke(m_Progress);
                }
            }
        }

        public float NextStep()
        {
            if (m_CurrentStep < m_Steps.Length - 1)
            {
                m_CurrentStep++;
                Progress = 0;
            }
            else
            {
                Self.SetProgress(1);
            }
            return Self.Value;
        }

        float IProgressWritable.SetDone()
        {
            m_CurrentStep = m_Steps.Length - 1;
            return Self.SetProgress(1);
        }

        public float Clear()
        {
            m_Done = false;
            m_CurrentStep = 0;
            Progress = 0;
            return Self.Value;
        }

        #region IProgress
        float IProgress.Value
        {
            get
            {
                if (m_Done) return 1;
                if (m_Steps == null) return 0;

                float result = Progress * m_Steps[m_CurrentStep];
                for (int i = 0; i < m_CurrentStep; i++)
                    result += m_Steps[i];
                return result;
            }
        }

        float IProgressWritable.SetProgress(float value)
        {
            Progress = value;
            return Self.Value;
        }

        event IProgress.OnProgressChange IProgress.OnChange
        {
            add => m_OnProgressChange += value;
            remove => m_OnProgressChange -= value;
        }
        #endregion
    }
}
