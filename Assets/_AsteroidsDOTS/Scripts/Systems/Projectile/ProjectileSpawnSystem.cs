using _AsteroidsDOTS.Scripts.DataComponents;
using _AsteroidsDOTS.Scripts.DataComponents.Tags;
using _AsteroidsDOTS.Scripts.Globals;
using Unity.Entities;
using Unity.Transforms;

namespace _AsteroidsDOTS.Scripts.Systems.Projectile
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class ProjectileSpawnSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem m_endSimulationBuffer;

        protected override void OnCreate()
        {
            m_endSimulationBuffer = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            if (!AsteroidsDOTS.GameIsStarted)
            {
                return;
            }

            var l_ecb = m_endSimulationBuffer.CreateCommandBuffer();
            Entities.WithAll<PlayerTag>().ForEach((ref ShootingData p_shootingData, in LocalToWorld p_localToWorld,
                    in Rotation p_rotation) =>
                {
                    if (!p_shootingData.ShouldShootProjectile) return;


                    p_shootingData.ShouldShootProjectile = false;

                    //Instantiate the projectile entity and set its initial position and 
                    Entity l_projectileEntity = l_ecb.Instantiate(p_shootingData.ProjectilePrefab);
                    UninitializedProjectileTag l_projectileTag = new UninitializedProjectileTag()
                        { IntendedForwards = p_localToWorld.Forward };
                    l_ecb.AddComponent(l_projectileEntity, l_projectileTag);
                    var l_spawnPosition = p_localToWorld.Position + p_localToWorld.Forward *
                        p_shootingData.ProjectileSpawnForwardOffset;
                    var l_projectileTranslation = new Translation() { Value = l_spawnPosition };
                    l_ecb.SetComponent(l_projectileEntity, l_projectileTranslation);
                    l_ecb.SetComponent(l_projectileEntity, p_rotation);
                }
            ).Schedule();

            m_endSimulationBuffer.AddJobHandleForProducer(Dependency);
        }
    }
}