using Unity.Collections;
using Unity.Entities;

namespace _AsteroidsDOTS.Scripts.DataComponents.Audio
{
    public struct DestructionSoundData : IComponentData
    {
        public FixedString32 DestructionSound;
        public float Volume;
    }
}