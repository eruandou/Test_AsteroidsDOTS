using _AsteroidsDOTS.Scripts.DataComponents;
using _AsteroidsDOTS.Scripts.DataComponents.Tags;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

namespace _AsteroidsDOTS.Scripts.Systems.Enemy
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class EnemyActionSystem : SystemBase
    {
        private LocalToWorld m_playerLocalToWorld;
        private EndSimulationEntityCommandBufferSystem m_endSimulationBuffer;


        protected override void OnCreate()
        {
            m_endSimulationBuffer = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnStartRunning()
        {
            var l_playerSingleton = GetSingletonEntity<InputConfigurationData>();
            m_playerLocalToWorld = EntityManager.GetComponentData<LocalToWorld>(l_playerSingleton);
        }

        protected override void OnUpdate()
        {
            var l_ecb = m_endSimulationBuffer.CreateCommandBuffer();
            Entities.WithAll<DumbUfoTag>().ForEach((Entity p_entity, ref IndividualRandomData p_randomData,
                ref ShootingData p_enemyShootingData, in LocalToWorld p_enemyLocalToWorld) =>
            {
                if (!p_enemyShootingData.ShouldShootProjectile)
                    return;

                var l_randomDirection = p_randomData.Random.NextFloat3Direction();
                l_randomDirection.y = 0;

                var l_projectileEntity = l_ecb.Instantiate(p_enemyShootingData.ProjectilePrefab);

                var l_spawnPosition = p_enemyLocalToWorld.Position +
                                      p_enemyLocalToWorld.Forward * p_enemyShootingData.ProjectileSpawnForwardOffset;
                var l_translation = new Translation() { Value = l_spawnPosition };
                l_ecb.SetComponent(l_projectileEntity, l_translation);

                var l_uninitializedProjectile = new UninitializedProjectileTag()
                    { IntendedForwards = l_randomDirection };
                l_ecb.AddComponent<UninitializedProjectileTag>(l_projectileEntity);
                l_ecb.SetComponent(l_projectileEntity, l_uninitializedProjectile);

                p_enemyShootingData.ShouldShootProjectile = false;
            }).Schedule();

            Entities.WithAll<CleverUfoTag>().ForEach((Entity p_entity, ref IndividualRandomData p_randomData,
                    in ShootingData p_enemyShootingData) =>
                {
                }).WithoutBurst()
                .Run();

            m_endSimulationBuffer.AddJobHandleForProducer(Dependency);
        }
    }
}