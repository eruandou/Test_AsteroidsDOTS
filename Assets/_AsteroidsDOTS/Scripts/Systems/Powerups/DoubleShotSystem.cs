using _AsteroidsDOTS.Scripts.DataComponents;
using _AsteroidsDOTS.Scripts.DataComponents.Powerups;
using _AsteroidsDOTS.Scripts.DataComponents.Tags;
using Unity.Entities;

namespace _AsteroidsDOTS.Scripts.Systems.Powerups
{
    public class DoubleShotSystem : SystemBase
    {
        private EndInitializationEntityCommandBufferSystem m_endInitializationBuffer;

        protected override void OnCreate()
        {
            m_endInitializationBuffer = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
            RequireSingletonForUpdate<DoubleShotPowerUpData>();
        }

        protected override void OnUpdate()
        {
            var l_ecb = m_endInitializationBuffer.CreateCommandBuffer();
            var l_deltaTime = Time.DeltaTime;
            Entities.ForEach((Entity p_entity, ref ShootingStatsPowerUpData p_shootingData,
                ref DoubleShotPowerUpData p_doubleShotPowerUpData) =>
            {
                if (!p_doubleShotPowerUpData.IsInUse)
                {
                    p_shootingData.BulletsToShoot = 2;
                    p_doubleShotPowerUpData.IsInUse = true;
                    return;
                }

                p_doubleShotPowerUpData.Duration -= l_deltaTime;

                if (p_doubleShotPowerUpData.ShouldExpire)
                {
                    p_shootingData.BulletsToShoot = 1;
                    l_ecb.RemoveComponent<DoublePointsPowerUpData>(p_entity);
                }
            }).Schedule();

            m_endInitializationBuffer.AddJobHandleForProducer(Dependency);
        }
    }
}