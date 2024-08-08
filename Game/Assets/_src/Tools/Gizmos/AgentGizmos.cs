using UnityEngine;
using Unity.Entities;

namespace Game.Debugs.Gizmos.Editor
{
    //[ExecuteInEditMode]
    //[CustomEditor(typeof(ViewUnit))]
    public class AgentGizmos : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        void OnEnable()
        {
            /*
            if (Application.isPlaying)
            {
                var world = World.DefaultGameObjectInjectionWorld;
                if (world == null)
                    return;
                var manager = world.EntityManager;
                if (!manager.HasComponent<DrawGizmos>(Agent.GetOrCreateEntity()))
                {
                    manager.AddComponent<DrawGizmos>(Agent.GetOrCreateEntity());
                }
            }
            */
        }

        void OnDisable()
        {
            /*
            if (Application.isPlaying)
            {
                var world = World.DefaultGameObjectInjectionWorld;
                if (world == null)
                    return;
                var manager = world.EntityManager;
                if (manager.HasComponent<DrawGizmos>(Agent.GetOrCreateEntity()))
                {
                    manager.RemoveComponent<DrawGizmos>(Agent.GetOrCreateEntity());
                }
            }
            */
        }

        private void OnDrawGizmos()
        {
            // Call OnSceneGUI only once
            //if (target == Selection.activeObject)
            //    return;

            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null)
                return;
            var gizmosSystem = world.Unmanaged.GetExistingUnmanagedSystem<GizmosSystem>();
            if (gizmosSystem == SystemHandle.Null)
                return;
            var gizmos = world.EntityManager.GetComponentData<GizmosSystem.Singleton>(gizmosSystem);
            gizmos.ExecuteCommandBuffers();
        }
    }
}
