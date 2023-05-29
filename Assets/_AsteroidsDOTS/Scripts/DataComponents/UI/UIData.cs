using Unity.Entities;

namespace _AsteroidsDOTS.Scripts.DataComponents.UI
{
    public struct UIData : IComponentData
    {
        public int CurrentPoints;
        public int PlayerLives;
    }
}