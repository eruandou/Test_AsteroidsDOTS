using Unity.Collections;
using Unity.Entities;

namespace _AsteroidsDOTS.Scripts.DataComponents.Audio
{
    public struct EnemyMoveSoundData : IComponentData
    {
        public FixedString32 EnemyMoveSound;
        public float MoveVolume;
        public bool ShouldLoop;
    }
}