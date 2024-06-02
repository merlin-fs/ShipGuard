using System;
using System.Collections;
using UnityEngine;


namespace UnityEngine
{
    public static class CoroutineUtils
    {
        public static void DelayedAction(this MonoBehaviour self, float delay, Action Action, Func<bool> isCancel = null)
        {
            self.StartCoroutine(DelayedActionCoroutine(delay, Action, isCancel));
        }

        private static IEnumerator DelayedActionCoroutine(float delay, Action Action, Func<bool> isCancel)
        {
            for (float timer = delay; timer >= 0; timer -= Time.deltaTime)
            {
                if (isCancel?.Invoke() ?? false)
                {
                    yield break;
                }
                yield return null;
            }
            Action.Invoke();
        }
    }
}