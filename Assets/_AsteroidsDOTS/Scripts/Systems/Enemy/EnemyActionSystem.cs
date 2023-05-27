using _AsteroidsDOTS.Scripts.DataComponents;
using _AsteroidsDOTS.Scripts.DataComponents.Tags;
using _AsteroidsDOTS.Scripts.DevelopmentUtilities;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace _AsteroidsDOTS.Scripts.Systems.Enemy
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class EnemyActionSystem : SystemBase
    {
        private Entity m_playerEntity;
        private EndSimulationEntityCommandBufferSystem m_endSimulationBuffer;


        protected override void OnCreate()
        {
            m_endSimulationBuffer = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnStartRunning()
        {
            m_playerEntity = GetSingletonEntity<PlayerTag>();
        }

        protected override void OnUpdate()
        {
            var l_ecb = m_endSimulationBuffer.CreateCommandBuffer();
            Entities.WithAll<DumbUfoTag>().ForEach((Entity p_entity, ref IndividualRandomData p_randomData,
                ref ShootingData p_enemyShootingData, in LocalToWorld p_enemyLocalToWorld) =>
            {
                if (!p_enemyShootingData.ShouldShootProjectile)
                    return;

                var l_randomDir = p_randomData.Random.NextFloat2Direction();
                var l_randomDirection = new float3(l_randomDir.x, 0, l_randomDir.y);

                var l_projectileEntity = l_ecb.Instantiate(p_enemyShootingData.ProjectilePrefab);

                var l_spawnPosition = p_enemyLocalToWorld.Position +
                                      l_randomDirection * p_enemyShootingData.ProjectileSpawnForwardOffset;
                var l_translation = new Translation() { Value = l_spawnPosition };
                l_ecb.SetComponent(l_projectileEntity, l_translation);


                var l_rotation = new Rotation()
                    { Value = quaternion.LookRotation(l_randomDirection, Float3Constants.Up) };
                l_ecb.SetComponent(l_projectileEntity, l_rotation);

                UninitializedProjectileTag l_uninitializedProjectile = new UninitializedProjectileTag()
                    { IntendedForwards = l_randomDirection };
                l_ecb.AddComponent(l_projectileEntity, l_uninitializedProjectile);


                p_enemyShootingData.ShouldShootProjectile = false;
            }).Schedule();

            var l_playerLocalToWorld = EntityManager.GetComponentData<Translation>(m_playerEntity);

            Entities.WithAll<CleverUfoTag>().ForEach((Entity p_entity, ref IndividualRandomData p_randomData,
                ref ShootingData p_enemyShootingData, in LocalToWorld p_enemyLocalToWorld) =>
            {
                if (!p_enemyShootingData.ShouldShootProjectile)
                    return;


                var l_shootDirection = l_playerLocalToWorld.Value - p_enemyLocalToWorld.Position;
                l_shootDirection.y = 0;
                l_shootDirection = math.normalize(l_shootDirection);

                var l_projectileEntity = l_ecb.Instantiate(p_enemyShootingData.ProjectilePrefab);

                var l_spawnPosition = p_enemyLocalToWorld.Position +
                                      l_shootDirection * p_enemyShootingData.ProjectileSpawnForwardOffset;
                var l_translation = new Translation() { Value = l_spawnPosition };
                l_ecb.SetComponent(l_projectileEntity, l_translation);


                var l_rotation = new Rotation()
                    { Value = quaternion.LookRotation(l_shootDirection, Float3Constants.Up) };
                l_ecb.SetComponent(l_projectileEntity, l_rotation);

                UninitializedProjectileTag l_uninitializedProjectile = new UninitializedProjectileTag()
                    { IntendedForwards = l_shootDirection };
                l_ecb.AddComponent(l_projectileEntity, l_uninitializedProjectile);

                p_enemyShootingData.ShouldShootProjectile = false;
            }).Schedule();

            m_endSimulationBuffer.AddJobHandleForProducer(Dependency);
        }
    }
}