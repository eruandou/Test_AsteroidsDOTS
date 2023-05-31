using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace _AsteroidsDOTS.Scripts.DataComponents.Audio
{
    public struct ShootSoundData : IComponentData
    {
        public FixedString32 ShotSound;
        public float ShotVolume;
    }
}