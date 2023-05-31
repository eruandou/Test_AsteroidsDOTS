using _AsteroidsDOTS.Scripts.DataComponents;
using _AsteroidsDOTS.Scripts.DataComponents.Audio;
using _AsteroidsDOTS.Scripts.DataComponents.GameState;
using _AsteroidsDOTS.Scripts.DataComponents.Tags;
using Unity.Entities;

namespace _AsteroidsDOTS.Scripts.Systems.Enemy
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class EnemyBehaviourSystem : SystemBase
    {
        private EndInitializationEntityCommandBufferSystem m_endSimulationBuffer;
        private Entity m_gameStateEntity;

        protected override void OnCreate()
        {
            m_endSimulationBuffer = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
        }

        protected override void OnStartRunning()
        {
            m_gameStateEntity = GetSingletonEntity<GameStateDataUfo>();
        }

        protected override void OnUpdate()
        {
            var l_ecb = m_endSimulationBuffer.CreateCommandBuffer();

            var l_currentTime = (float)Time.ElapsedTime;
            var l_gameStateEntity = m_gameStateEntity;
            var l_gameStateData = GetSingleton<GameStateDataUfo>();
            Entities.WithAny<DumbUfoTag, CleverUfoTag>().ForEach((Entity p_entity, ref ShootingData p_enemyShootingData,
                ref EntityHealthData p_enemyHealth, in EnemyMoveSoundData p_soundData, in DestructionSoundData p_destructionSoundData) =>
            {
                if (p_enemyHealth.ShouldDie)
                {
                    l_ecb.AddComponent<DeadPointsEntityTag>(p_entity);
                    l_gameStateData.SpawnedUfo--;
                    l_ecb.SetComponent(l_gameStateEntity, l_gameStateData);
                    var l_destructionSoundPetition = new AudioPetition()
                    {
                        AudioID = p_destructionSoundData.DestructionSound,
                        Volume = p_destructionSoundData.Volume,
                        ShouldLoop = false
                    };
                    var l_audioStopPetition = new AudioStopPetition()
                    {
                        AudioID = p_soundData.EnemyMoveSound
                    };
                    l_ecb.AddComponent(p_entity, l_audioStopPetition);
                    if (p_enemyHealth.DeadParticlesPrefab == Entity.Null)
                        return;

                    l_ecb.Instantiate(p_enemyHealth.DeadParticlesPrefab);
                }

                if (p_enemyShootingData.NextShootingTime <= l_currentTime)
                {
                    p_enemyShootingData.LastShootingTime = l_currentTime;
                    p_enemyShootingData.ShouldShootProjectile = true;
                }
            }).Schedule();

            m_endSimulationBuffer.AddJobHandleForProducer(Dependency);
        }
    }
}