using _AsteroidsDOTS.Scripts.DataComponents.GameState;
using _AsteroidsDOTS.Scripts.DataComponents.Tags;
using _AsteroidsDOTS.Scripts.DataComponents.UI;
using Unity.Entities;

namespace _AsteroidsDOTS.Scripts.Systems
{
    public class CheckEndGameSystem : SystemBase
    {
        private BeginInitializationEntityCommandBufferSystem m_beginInitializationBuffer;

        protected override void OnCreate()
        {
            RequireSingletonForUpdate<GameFinishedTag>();
            m_beginInitializationBuffer = World.GetExistingSystem<BeginInitializationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var l_ecb = m_beginInitializationBuffer.CreateCommandBuffer();
            var l_gameData = GetSingletonEntity<GameStateDataPlayer>();
            Entities.ForEach((GameEnder p_gameEnder) =>
            {
                p_gameEnder.SetPopup();
                l_ecb.RemoveComponent<GameFinishedTag>(l_gameData);
            }).WithoutBurst().Run();
        }
    }
}