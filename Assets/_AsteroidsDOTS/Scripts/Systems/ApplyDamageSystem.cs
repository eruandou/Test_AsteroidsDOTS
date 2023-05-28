using _AsteroidsDOTS.Scripts.DataComponents;
using Unity.Entities;

namespace _AsteroidsDOTS.Scripts.Systems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public class ApplyDamageSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((ref EntityHealthData p_healthData) =>
            {
                if (p_healthData.PendingHealthModification == 0) return;

                p_healthData.Health += p_healthData.PendingHealthModification;
                p_healthData.PendingHealthModification = 0;
            }).ScheduleParallel();
        }
    }
}