using _AsteroidsDOTS.Scripts.DataComponents;
using _AsteroidsDOTS.Scripts.DataComponents.Enemies;
using _AsteroidsDOTS.Scripts.DataComponents.Tags;
using Unity.Entities;

namespace _AsteroidsDOTS.Scripts.Systems.Enemy
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class EnemyBehaviourSystem : SystemBase
    {
        private EndInitializationEntityCommandBufferSystem m_endSimulationBuffer;

        protected override void OnCreate()
        {
            m_endSimulationBuffer = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var l_ecb = m_endSimulationBuffer.CreateCommandBuffer().AsParallelWriter();

            var l_currentTime = (float)Time.ElapsedTime;
            Entities.WithAny<DumbUfoTag, CleverUfoTag>().ForEach((Entity p_entity, int entityInQueryIndex,
                ref ShootingData p_enemyShootingData,
                ref EntityHealthData p_enemyHealth) =>
            {
                if (p_enemyHealth.ShouldDie)
                {
                    l_ecb.AddComponent<DeadPointsEntityTag>(entityInQueryIndex, p_entity);

                    //Add points
                    // p_pointsData.PointsToGiveOut
                    return;
                }

                if (p_enemyShootingData.NextShootingTime <= l_currentTime)
                {
                    p_enemyShootingData.LastShootingTime = l_currentTime;
                    p_enemyShootingData.ShouldShootProjectile = true;
                }
            }).Schedule();

            m_endSimulationBuffer.AddJobHandleForProducer(Dependency);
        }
    }
}