using _AsteroidsDOTS.Scripts.DataComponents.Tags;
using Unity.Entities;
using Unity.Transforms;

namespace _AsteroidsDOTS.Scripts.Systems
{
    public class CopyPlayerPositionSystem : SystemBase
    {
        private Entity m_playerEntity;
        private EntityQueryDesc m_queryDesc;

        protected override void OnCreate()
        {
            m_queryDesc = new EntityQueryDesc()
            {
                All = new[]
                {
                    ComponentType.ReadOnly<CopyPlayerPositionTag>(),
                    ComponentType.ReadWrite<Translation>(),
                }
            };

            RequireForUpdate(GetEntityQuery(m_queryDesc));
            RequireSingletonForUpdate<PlayerTag>();
        }

        protected override void OnStartRunning()
        {
            m_playerEntity = GetSingletonEntity<PlayerTag>();
        }

        protected override void OnUpdate()
        {
            if (m_playerEntity == Entity.Null)
                return;
            var l_playerLocalToWorld = EntityManager.GetComponentData<Translation>(m_playerEntity);
            var l_playerPosition = l_playerLocalToWorld.Value;

            Entities.WithAll<CopyPlayerPositionTag>()
                .ForEach((ref Translation p_translation) => { p_translation.Value = l_playerPosition; })
                .ScheduleParallel();
        }
    }
}