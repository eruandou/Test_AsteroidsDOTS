using _AsteroidsDOTS.Scripts.DataComponents;
using Unity.Entities;

namespace _AsteroidsDOTS.Scripts.Systems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public class ApplyDamageSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate(GetEntityQuery(ComponentType.ReadWrite<EntityHealthData>()));
        }

        protected override void OnUpdate()
        {
            var l_deltaTime = Time.DeltaTime;
            Entities.ForEach((ref EntityHealthData p_healthData) =>
            {
                p_healthData.CurrentInvincibilityTime -= l_deltaTime;

                if (p_healthData.PendingHealthModification == 0) return;

                if (p_healthData.IsInvincible)
                {
                    p_healthData.PendingHealthModification = 0;
                    return;
                }

                p_healthData.Health += p_healthData.PendingHealthModification;
                p_healthData.PendingHealthModification = 0;
                p_healthData.CurrentInvincibilityTime = p_healthData.InvincibilityTime;
            }).ScheduleParallel();
        }
    }
}