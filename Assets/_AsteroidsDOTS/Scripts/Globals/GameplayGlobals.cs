using Unity.Mathematics;

namespace _AsteroidsDOTS.Scripts.Globals
{
    public static class GameplayStaticGlobals
    {
        public static readonly float2 HorizontalLimits = new float2 { x = -115, y = 115 };
        public static readonly float2 VerticalLimits = new float2 { x = -67.5f, y = 67.5f };
        public static readonly float ScreenLimitOffset = 0.5f;
    }
}