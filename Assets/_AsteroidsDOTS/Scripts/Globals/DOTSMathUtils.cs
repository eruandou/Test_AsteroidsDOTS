using Unity.Mathematics;

namespace _AsteroidsDOTS.Scripts.Globals
{
    public static class DOTSMathUtils
    {
        public static float3 ClampFloat3(float3 p_float3, float p_maxMagnitude)
        {
            var l_magnitude = math.length(p_float3);

            if (l_magnitude <= p_maxMagnitude)
            {
                return p_float3;
            }

            float3 l_normalizedVector = p_float3 / l_magnitude;
            return l_normalizedVector * p_maxMagnitude;
        }
    }
}