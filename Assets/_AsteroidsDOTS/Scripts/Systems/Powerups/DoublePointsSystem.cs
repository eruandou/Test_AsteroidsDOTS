using _AsteroidsDOTS.Scripts.DataComponents;
using _AsteroidsDOTS.Scripts.DataComponents.Powerups;
using Unity.Entities;

namespace _AsteroidsDOTS.Scripts.Systems.Powerups
{
    public class DoublePointsSystem : SystemBase
    {
        private EndInitializationEntityCommandBufferSystem m_endInitializationBuffer;

        protected override void OnCreate()
        {
            RequireSingletonForUpdate<DoublePointsPowerUpData>();
            m_endInitializationBuffer = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var l_ecb = m_endInitializationBuffer.CreateCommandBuffer();
            var l_deltaTime = Time.DeltaTime;
            Entities.ForEach((Entity p_entity, ref GameStateData p_gameStateData,
                ref DoublePointsPowerUpData p_doublePointsData) =>
            {
                if (!p_doublePointsData.IsInUse)
                {
                    p_gameStateData.PointsMultiplier = 2;
                    p_doublePointsData.IsInUse = true;
                    return;
                }

                p_doublePointsData.Duration -= l_deltaTime;

                if (p_doublePointsData.ShouldExpire)
                {
                    p_gameStateData.PointsMultiplier = 1;
                    l_ecb.RemoveComponent<DoublePointsPowerUpData>(p_entity);
                }
            }).Schedule();

            m_endInitializationBuffer.AddJobHandleForProducer(Dependency);
        }
    }
}