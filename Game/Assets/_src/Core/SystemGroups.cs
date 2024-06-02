using System;

using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

namespace Game
{
    [UpdateInGroup(typeof(InitializationSystemGroup), OrderFirst = true)]
    public partial class GameSpawnSystemCommandBufferSystem : BeginInitializationEntityCommandBufferSystem
    {
        protected override void OnUpdate()
        {
            try
            {
                base.OnUpdate();
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
        }
    }


    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class GameSpawnSystemGroup : ComponentSystemGroup { }
    

    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class GameSystemGroup : ComponentSystemGroup { }

        [UpdateInGroup(typeof(GameSystemGroup), OrderFirst = true)]
        public partial class GameLogicInitSystemGroup : ComponentSystemGroup { }

        [UpdateInGroup(typeof(GameSystemGroup))]
        [UpdateAfter(typeof(GameLogicInitSystemGroup))]
        public partial class GameLogicSystemGroup : ComponentSystemGroup { }

        [UpdateInGroup(typeof(GameLogicSystemGroup), OrderFirst = true)]
        public partial class GameLogicObjectSystemGroup : ComponentSystemGroup { }

        [UpdateInGroup(typeof(GameLogicSystemGroup), OrderLast = true)]
        public partial class GameLogicCommandBufferSystem : EntityCommandBufferSystem
        {
            #region Singleton
            public unsafe struct Singleton : IComponentData, IECBSingleton
            {
                internal UnsafeList<EntityCommandBuffer>* pendingBuffers;
                internal AllocatorManager.AllocatorHandle allocator;

                /// <summary>
                /// Create a command buffer for the parent system to play back.
                /// </summary>
                /// <remarks>The command buffers created by this method are automatically added to the system's list of
                /// pending buffers.</remarks>
                /// <param name="world">The world in which to play it back.</param>
                /// <returns>The command buffer to record to.</returns>
                public EntityCommandBuffer CreateCommandBuffer(WorldUnmanaged world)
                {
                    return EntityCommandBufferSystem.CreateCommandBuffer(ref *pendingBuffers, allocator, world);
                }

                /// <summary>
                /// Sets the list of command buffers to play back when this system updates.
                /// </summary>
                /// <remarks>This method is only intended for internal use, but must be in the public API due to language
                /// restrictions. Command buffers created with <see cref="CreateCommandBuffer"/> are automatically added to
                /// the system's list of pending buffers to play back.</remarks>
                /// <param name="buffers">The list of buffers to play back. This list replaces any existing pending command buffers on this system.</param>
                public void SetPendingBufferList(ref UnsafeList<EntityCommandBuffer> buffers)
                {
                    pendingBuffers = (UnsafeList<EntityCommandBuffer>*)UnsafeUtility.AddressOf(ref buffers);
                }

                /// <summary>
                /// Set the allocator that command buffers created with this singleton should be allocated with.
                /// </summary>
                /// <param name="allocatorIn">The allocator to use</param>
                public void SetAllocator(Allocator allocatorIn)
                {
                    allocator = allocatorIn;
                }

                /// <summary>
                /// Set the allocator that command buffers created with this singleton should be allocated with.
                /// </summary>
                /// <param name="allocatorIn">The allocator to use</param>
                public void SetAllocator(AllocatorManager.AllocatorHandle allocatorIn)
                {
                    allocator = allocatorIn;
                }
            }
            /// <inheritdoc cref="EntityCommandBufferSystem.OnCreate"/>
            #endregion
            protected override void OnCreate()
            {
                base.OnCreate();

                this.RegisterSingleton<Singleton>(ref PendingBuffers, World.Unmanaged);
            }
            protected override void OnUpdate()
            {
                try
                {
                    base.OnUpdate();
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogException(e);
                }
            }
        }

        [UpdateInGroup(typeof(GameSystemGroup), OrderLast = true)]
        [UpdateAfter(typeof(GameLogicSystemGroup))]
        public partial class GameLogicEndSystemGroup : ComponentSystemGroup { }

        [UpdateInGroup(typeof(GameLogicEndSystemGroup), OrderLast = true)]
        public partial class GameLogicEndCommandBufferSystem : EntityCommandBufferSystem
        {
            protected override void OnUpdate()
            {
                try
                {
                    base.OnUpdate();
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogException(e);
                }
            }
        }


        [UpdateInGroup(typeof(LateSimulationSystemGroup))]
        public partial class GameEndSystemGroup : ComponentSystemGroup { }


        ///[UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
        //public class GameEndSystemCommandBufferSystem : EndSimulationEntityCommandBufferSystem { }


        [UpdateInGroup(typeof(PresentationSystemGroup))]
        public partial class GamePresentationSystemGroup : ComponentSystemGroup { }
}
