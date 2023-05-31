using _AsteroidsDOTS.Scripts.DataComponents;
using _AsteroidsDOTS.Scripts.DataComponents.Audio;
using _AsteroidsDOTS.Scripts.DataComponents.GameState;
using _AsteroidsDOTS.Scripts.DataComponents.Tags;
using _AsteroidsDOTS.Scripts.DataComponents.UI;
using Unity.Entities;

namespace _AsteroidsDOTS.Scripts.Systems
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class PlayerHealthSystem : SystemBase
    {
        private EndInitializationEntityCommandBufferSystem m_endInitializationBuffer;

        protected override void OnCreate()
        {
            m_endInitializationBuffer = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
            RequireSingletonForUpdate<PlayerTag>();
        }

        protected override void OnUpdate()
        {
            var l_ecb = m_endInitializationBuffer.CreateCommandBuffer();
            var l_gameStateData = GetSingleton<GameStateDataPlayer>();
            var l_gameDataEntity = GetSingletonEntity<GameData>();
            var l_gameData = GetSingleton<GameData>();
            Entities.WithAny<PlayerTag>().ForEach(
                (Entity p_entity, in EntityHealthData p_healthData, in DestructionSoundData p_destructionSoundData) =>
                {
                    if (p_healthData.ShouldDie)
                    {
                        l_ecb.DestroyEntity(p_entity);
                        l_gameStateData.CurrentPlayerLives--;
                        l_ecb.SetComponent(l_gameDataEntity, l_gameStateData);
                        if (!l_gameStateData.PlayerLost)
                        {
                            l_ecb.AddComponent(l_gameDataEntity, new RespawnPlayerData()
                            {
                                RespawnTimeLeft = l_gameData.PlayerRespawnTime
                            });
                            var l_deathSoundComponent = new AudioPetition()
                            {
                                AudioID = p_destructionSoundData.DestructionSound,
                                ShouldLoop = false,
                                Volume = p_destructionSoundData.Volume
                            };
                            l_ecb.AddComponent(p_entity, l_deathSoundComponent);
                        }
                        else
                        {
                            //Set Game Finish UI via Tag
                            l_ecb.AddComponent<GameFinishedTag>(l_gameDataEntity);
                        }
                    }
                }).Schedule();

            m_endInitializationBuffer.AddJobHandleForProducer(Dependency);
        }
    }
}