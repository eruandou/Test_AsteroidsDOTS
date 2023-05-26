using _AsteroidsDOTS.Scripts.DataComponents;
using _AsteroidsDOTS.Scripts.DataComponents.Tags;
using Unity.Entities;

namespace _AsteroidsDOTS.Scripts.Systems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public class PointsSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem m_endSimulationBuffer;

        protected override void OnCreate()
        {
            m_endSimulationBuffer = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var l_ecb = m_endSimulationBuffer.CreateCommandBuffer().AsParallelWriter();
            Entities.WithAll<DeadPointsEntityTag>()
                .ForEach((Entity l_entity, int entityInQueryIndex, in PointAwardData p_pointAwardData) =>
                {
                    //Add points here.

                    l_ecb.DestroyEntity(entityInQueryIndex, l_entity);
                })
                .ScheduleParallel();

            m_endSimulationBuffer.AddJobHandleForProducer(Dependency);
        }
    }
}