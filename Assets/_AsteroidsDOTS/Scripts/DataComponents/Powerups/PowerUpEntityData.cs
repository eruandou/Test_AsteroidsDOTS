using Unity.Entities;

namespace _AsteroidsDOTS.Scripts.DataComponents.Powerups
{
    public enum PowerUpType
    {
        DoublePoints,
        DoubleShot,
        RecoverHealth,
        InvulnerabilityShield,
        SuperBomb
    }
    
    [GenerateAuthoringComponent]
    public struct PowerUpEntityData : IComponentData
    {
        public bool AlreadyPickedUp;
        public PowerUpType PowerUpType;
    }
}