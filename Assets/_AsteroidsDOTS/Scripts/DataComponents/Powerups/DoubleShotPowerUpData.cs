namespace _AsteroidsDOTS.Scripts.DataComponents.Powerups
{
    public struct DoubleShotPowerUpData : IPowerUp
    {
        public float Duration;
        public bool IsInUse;
        public bool ShouldExpire => Duration <= 0;
    }
}