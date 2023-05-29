using _AsteroidsDOTS.Scripts.DataComponents;
using _AsteroidsDOTS.Scripts.DataComponents.UI;
using Unity.Entities;

namespace _AsteroidsDOTS.Scripts.Systems
{
    public class InitializationSystem : SystemBase
    {
        private BeginInitializationEntityCommandBufferSystem m_beginInitializationBuffer;

        protected override void OnCreate()
        {
            m_beginInitializationBuffer = World.GetExistingSystem<BeginInitializationEntityCommandBufferSystem>();
        }

        protected override void OnStartRunning()
        {
            //  var l_ecb = m_beginInitializationBuffer.CreateCommandBuffer();
            // var l_uiUpdateEntity = GetSingletonEntity<UIUpdater>();
            //Entities.ForEach((in GameStateData p_gameStateData) =>
            // {
            //     l_ecb.AddComponent<DirtyUITag>(l_uiUpdateEntity);
            // }).Schedule();
            //
            // m_beginInitializationBuffer.AddJobHandleForProducer(Dependency);
        }

        protected override void OnUpdate()
        {
        }
    }
}