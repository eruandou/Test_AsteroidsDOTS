using _AsteroidsDOTS.Scripts.DataComponents;
using _AsteroidsDOTS.Scripts.DataComponents.Powerups;
using Unity.Entities;

namespace _AsteroidsDOTS.Scripts.Systems.Powerups
{
    public class HealthPowerUpSystem : SystemBase
    {
        private EndInitializationEntityCommandBufferSystem m_endInitializationBuffer;

        protected override void OnCreate()
        {
            m_endInitializationBuffer = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
            RequireSingletonForUpdate<FullRecoveryPowerUpData>();
        }

        protected override void OnUpdate()
        {
            var l_ecb = m_endInitializationBuffer.CreateCommandBuffer();
            Entities.WithAll<FullRecoveryPowerUpData>().ForEach(
                    (Entity p_playerEntity, ref EntityHealthData p_playerHealth) =>
                    {
                        p_playerHealth.Health = p_playerHealth.MaxHealth;
                        l_ecb.RemoveComponent<FullRecoveryPowerUpData>(p_playerEntity);
                    })
                .Schedule();
            m_endInitializationBuffer.AddJobHandleForProducer(Dependency);
        }
    }
}