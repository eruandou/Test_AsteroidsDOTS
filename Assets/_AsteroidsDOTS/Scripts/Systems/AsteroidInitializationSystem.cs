using _AsteroidsDOTS.Scripts.DataComponents;
using _AsteroidsDOTS.Scripts.DataComponents.Asteroids;
using _AsteroidsDOTS.Scripts.DataComponents.Tags;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

namespace _AsteroidsDOTS.Scripts.Systems
{
    public class AsteroidInitializationSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem m_endSimulationBuffer;

        protected override void OnCreate()
        {
            m_endSimulationBuffer = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var l_ecb = m_endSimulationBuffer.CreateCommandBuffer().AsParallelWriter();
            Entities.ForEach((Entity p_entity, int entityInQueryIndex,
                ref IndividualRandomData p_randomData, in AsteroidData p_asteroidData,
                in UninitializedAsteroid p_uninitializedAsteroid) =>
            {
                p_randomData.Random = Random.CreateFromIndex((uint)entityInQueryIndex);
                var l_speed = p_uninitializedAsteroid.IntendedDirection *
                              p_randomData.Random.NextFloat(p_asteroidData.RandomMinMaxVelocity.x,
                                  p_asteroidData.RandomMinMaxVelocity.y);
                var l_physicsVelocity = new PhysicsVelocity() { Linear = l_speed };
                l_ecb.SetComponent(entityInQueryIndex, p_entity, l_physicsVelocity);
                l_ecb.RemoveComponent<UninitializedAsteroid>(entityInQueryIndex, p_entity);
            }).ScheduleParallel();

            m_endSimulationBuffer.AddJobHandleForProducer(Dependency);
        }
    }
}