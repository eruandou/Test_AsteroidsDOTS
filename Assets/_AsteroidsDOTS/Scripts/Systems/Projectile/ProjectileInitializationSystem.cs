using _AsteroidsDOTS.Scripts.DataComponents;
using _AsteroidsDOTS.Scripts.DataComponents.Tags;
using _AsteroidsDOTS.Scripts.Globals;
using Unity.Entities;
using Unity.Physics;

namespace _AsteroidsDOTS.Scripts.Systems.Projectile
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class ProjectileInitializationSystem : SystemBase
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

            var l_ecb = m_endSimulationBuffer.CreateCommandBuffer().AsParallelWriter();
            Entities.ForEach((Entity p_entity, int entityInQueryIndex, ref ProjectileData p_projectileData,
                    in UninitializedProjectileTag p_uninitializedProjectile) =>
                {
                    var l_projectileVelocity = new PhysicsVelocity()
                        { Linear = p_uninitializedProjectile.IntendedForwards * p_projectileData.Speed };
                    l_ecb.SetComponent(entityInQueryIndex, p_entity, l_projectileVelocity);
                    l_ecb.RemoveComponent<UninitializedProjectileTag>(entityInQueryIndex, p_entity);
                }
            ).ScheduleParallel();

            m_endSimulationBuffer.AddJobHandleForProducer(Dependency);
        }
    }
}