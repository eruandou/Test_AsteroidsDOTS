using _AsteroidsDOTS.Scripts.DataComponents;
using Unity.Entities;

namespace _AsteroidsDOTS.Scripts.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true), UpdateAfter(typeof(PointsSystem))]
    public class TimedEntityDestroySystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem m_endSimulationBuffer;

        protected override void OnCreate()
        {
            m_endSimulationBuffer = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var l_deltaTime = Time.DeltaTime;
            var l_ecb = m_endSimulationBuffer.CreateCommandBuffer().AsParallelWriter();
            Entities.ForEach((int entityInQueryIndex, Entity p_entity, ref TimedEntityData p_timedEntity) =>
            {
                p_timedEntity.Lifetime -= l_deltaTime;

                if (p_timedEntity.ShouldBeKilled)
                {
                    l_ecb.DestroyEntity(entityInQueryIndex, p_entity);
                }
            }).ScheduleParallel();

            m_endSimulationBuffer.AddJobHandleForProducer(Dependency);
        }
    }
}