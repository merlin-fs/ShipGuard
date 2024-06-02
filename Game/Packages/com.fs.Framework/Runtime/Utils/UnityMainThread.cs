using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace System.Threading
{
    public static class UnityMainThread
    {
        private static SynchronizationContext m_Context;

        public static SynchronizationContext Context => m_Context;

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
#endif
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Capture()
        {
            m_Context = SynchronizationContext.Current;
        }
    }
}