using System.Collections.Generic;
using System.Linq;
using _AsteroidsDOTS.Scripts.DataComponents;
using _AsteroidsDOTS.Scripts.DataComponents.Audio;
using Unity.Collections;
using UnityEngine;

namespace _AsteroidsDOTS.Scripts.NonDOTSBehaviour
{
    public class EntityAudioManager : MonoBehaviour
    {
        [SerializeField] private AudioDefinitions m_audioDefinitions;
        [SerializeField] private int m_startingPoolSize = 10;
        [SerializeField] private int m_maxSoundsPool = 40;

        private List<AudioSource> m_audioSourcesInUse = new List<AudioSource>();
        private Queue<AudioSource> m_freeAudioSources = new Queue<AudioSource>();

        public static EntityAudioManager Instance;

        public void PlaySoundPetition(AudioPetition p_petition)
        {
            PlaySoundPetition(p_petition.AudioID, p_petition.ShouldLoop, p_petition.Volume);
        }

        public void PlaySoundPetition(FixedString32 p_soundID, bool p_isLoop, float p_desiredVolume)
        {
            if (!m_audioDefinitions.TryGetAudioClip(p_soundID, out AudioClip l_clipToPlay))
            {
                //There's no definition for the audio asked for
                return;
            }

            if (!GetNextAudioSource(out AudioSource l_audioSource))
            {
                //Ignore sound because we reached max amount
                return;
            }

            l_audioSource.clip = l_clipToPlay;
            l_audioSource.loop = p_isLoop;
            l_audioSource.Play();
        }

        public void StopSoundPetition(FixedString32 p_soundID)
        {
            if (!m_audioDefinitions.TryGetAudioClip(p_soundID, out AudioClip l_clip))
            {
                return;
            }

            if (!TryGetPlayerAudioSource(l_clip, out AudioSource l_source))
            {
                return;
            }

            l_source.Stop();
        }

        private void Awake()
        {
            //Create pool of audiosources.

            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            for (int i = 0; i < m_startingPoolSize; i++)
            {
                m_freeAudioSources.Enqueue(new GameObject().AddComponent<AudioSource>());
            }
        }

        private bool GetNextAudioSource(out AudioSource p_audioSource)
        {
            if (m_freeAudioSources.Count > 0)
            {
                p_audioSource = m_freeAudioSources.Dequeue();
                m_audioSourcesInUse.Add(p_audioSource);
                return true;
            }


            if (m_audioSourcesInUse.Count > m_maxSoundsPool)
            {
                p_audioSource = default;
                return false;
            }

            var l_newAudioSourceObject = new GameObject().AddComponent<AudioSource>();
            p_audioSource = l_newAudioSourceObject;
            m_audioSourcesInUse.Add(l_newAudioSourceObject);
            return true;
        }

        private bool TryGetPlayerAudioSource(AudioClip p_audioClip, out AudioSource p_audioSource)
        {
            p_audioSource = m_audioSourcesInUse.FirstOrDefault((p_source) => p_source.clip.Equals(p_audioClip));
            return p_audioClip != default;
        }

        private void LateUpdate()
        {
            for (int i = m_audioSourcesInUse.Count - 1; i >= 0; i--)
            {
                var l_audioSource = m_audioSourcesInUse[i];
                if (l_audioSource.isPlaying)
                    continue;
                m_freeAudioSources.Enqueue(l_audioSource);
                m_audioSourcesInUse.RemoveAt(i);
            }
        }
    }
}