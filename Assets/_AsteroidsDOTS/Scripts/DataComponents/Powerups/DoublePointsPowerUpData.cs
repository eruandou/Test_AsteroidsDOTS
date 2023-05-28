namespace _AsteroidsDOTS.Scripts.DataComponents.Powerups
{
    public struct DoublePointsPowerUpData : IPowerUp
    {
        public float Duration;
        public bool IsInUse;
        public bool ShouldExpire => Duration <= 0;
    }
}