using _AsteroidsDOTS.Scripts.DataComponents;
using _AsteroidsDOTS.Scripts.DataComponents.Audio;
using _AsteroidsDOTS.Scripts.NonDOTSBehaviour;
using Unity.Entities;

namespace _AsteroidsDOTS.Scripts.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class AudioPlayerSystem : SystemBase
    {
        private EntityQueryDesc m_soundQueryDesc;
        private BeginPresentationEntityCommandBufferSystem m_beginPresentationBuffer;

        protected override void OnCreate()
        {
            m_beginPresentationBuffer = World.GetExistingSystem<BeginPresentationEntityCommandBufferSystem>();
            m_soundQueryDesc = new EntityQueryDesc()
            {
                Any = new[]
                {
                    ComponentType.ReadOnly<AudioPetition>(),
                    ComponentType.ReadOnly<AudioStopPetition>()
                }
            };

            RequireForUpdate(GetEntityQuery(m_soundQueryDesc));
        }

        protected override void OnUpdate()
        {
            var l_audioPlayer = EntityAudioManager.Instance;
            var l_ecb = m_beginPresentationBuffer.CreateCommandBuffer();

            Entities.ForEach((Entity p_entity, in AudioPetition p_audioPetition) =>
            {
                l_audioPlayer.PlaySoundPetition(p_audioPetition);
                l_ecb.RemoveComponent<AudioPetition>(p_entity);
            }).WithoutBurst().Run();

            Entities.ForEach((Entity p_entity, in AudioStopPetition p_audioStopPetition) =>
            {
                l_audioPlayer.StopSoundPetition(p_audioStopPetition.AudioID);
                l_ecb.RemoveComponent<AudioStopPetition>(p_entity);
            }).WithoutBurst().Run();
        }
    }
}