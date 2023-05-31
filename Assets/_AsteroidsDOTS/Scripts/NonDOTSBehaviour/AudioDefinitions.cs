using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace _AsteroidsDOTS.Scripts.NonDOTSBehaviour
{
    [Serializable]
    public struct AudioDefinition
    {
        public string AudioID;
        public AudioClip AudioClip;
    }

    [CreateAssetMenu(menuName = "DotSteroids/Audio/AudioData", order = 0)]
    public class AudioDefinitions : ScriptableObject
    {
        [SerializeField] private AudioDefinition[] m_audioDefinitions;

        private Dictionary<FixedString32, AudioClip> m_idToAudio;


        private void InitializeDictionary()
        {
            if (m_idToAudio == null)
            {
                m_idToAudio = new Dictionary<FixedString32, AudioClip>();

                foreach (var l_audioDefinition in m_audioDefinitions)
                {
                    m_idToAudio.Add(new FixedString32(l_audioDefinition.AudioID), l_audioDefinition.AudioClip);
                }
            }
        }

        public bool TryGetAudioClip(FixedString32 p_audioID, out AudioClip p_clip)
        {
            p_clip = default;
            if (m_idToAudio == null)
                InitializeDictionary();
            if (!m_idToAudio.ContainsKey(p_audioID))
                return false;
            p_clip = m_idToAudio[p_audioID];
            return true;
        }
    }
}