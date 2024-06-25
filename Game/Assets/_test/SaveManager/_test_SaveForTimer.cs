using System.Diagnostics;

using Reflex.Attributes;

using Unity.Entities;
using Unity.Entities.Serialization;

using UnityEngine;

using Debug = UnityEngine.Debug;

namespace Game
{
    public class _test_SaveForTimer : MonoBehaviour
    {
        [Inject] private SerializeManager m_SerializeManager;

        private float m_Time = 0;

        void Update()
        {
            return;
            m_Time += Time.deltaTime;
            if (!(m_Time > 10)) return;
            
            var stopwatch = Stopwatch.StartNew();
            using (var writer = new StreamBinaryWriter($"{Application.persistentDataPath}/saveData.test"))
            {
                m_SerializeManager.SerializeWorld(World.DefaultGameObjectInjectionWorld, writer);
            }
            Debug.Log($"SerializeWorld : {stopwatch.Elapsed}");
            m_Time = 0;
        }
    }
}
