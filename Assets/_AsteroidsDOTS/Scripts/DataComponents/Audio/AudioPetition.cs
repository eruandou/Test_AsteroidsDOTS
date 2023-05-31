using Unity.Collections;
using Unity.Entities;

namespace _AsteroidsDOTS.Scripts.DataComponents.Audio
{
    public struct AudioPetition: IComponentData
    {
        public FixedString32 AudioID;
        public float Volume;
        public bool ShouldLoop;
    }

    public struct AudioStopPetition : IComponentData
    {
        public FixedString32 AudioID;
    }
}