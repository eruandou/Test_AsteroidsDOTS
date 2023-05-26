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
            Entities.ForEach((ref ShootingData p_playerShootingData, in LocalToWorld p_playerLocalToWorld,
                    in Rotation p_playerRotation) =>
                {
                    if (!p_playerShootingData.ShouldShootProjectile) return;


                    //Make sure the player's input consequence gets reset ONLY if they actually shot this frame
                    p_playerShootingData.ShouldShootProjectile = false;

                    //Instantiate the projectile entity and set its initial position and 
                    Entity l_projectileEntity = l_ecb.Instantiate(p_playerShootingData.ProjectilePrefab);
                    var l_projectileTag = new UninitializedProjectileTag()
                        { IntendedForwards = p_playerLocalToWorld.Forward };
                    l_ecb.AddComponent<UninitializedProjectileTag>(l_projectileEntity);
                    l_ecb.SetComponent(l_projectileEntity, l_projectileTag);
                    var l_spawnPosition = p_playerLocalToWorld.Position + p_playerLocalToWorld.Forward *
                        p_playerShootingData.ProjectileSpawnForwardOffset;
                    var l_projectileTranslation = new Translation() { Value = l_spawnPosition };
                    l_ecb.SetComponent(l_projectileEntity, l_projectileTranslation);
                    l_ecb.SetComponent(l_projectileEntity, p_playerRotation);
                }
            ).Schedule();

            m_endSimulationBuffer.AddJobHandleForProducer(Dependency);
        }
    }
}