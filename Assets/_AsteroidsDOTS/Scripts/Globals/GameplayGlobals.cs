using Unity.Mathematics;
using UnityEngine;

namespace _AsteroidsDOTS.Scripts.Globals
{
    public static class GameplayGlobals
    {
        public static float2 HorizontalLimits = new float2 { x = -115, y = 115 };
        public static float2 VerticalLimits = new float2 { x = -67.5f, y = 67.5f };
        public static float ScreenLimitOffset = 0.5f;
    }
}