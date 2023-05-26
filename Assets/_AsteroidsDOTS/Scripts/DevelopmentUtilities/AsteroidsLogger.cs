using System;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _AsteroidsDOTS.Scripts.DevelopmentUtilities
{
    public enum LogLevel
    {
        Message,
        Warning,
        Error
    };

    public class AsteroidsLogger
    {
        public static void Log(LogLevel p_logLevel, object p_message)
        {
#if UNITY_EDITOR
            switch (p_logLevel)
            {
                case LogLevel.Message:
                    Debug.Log(p_message);
                    break;
                case LogLevel.Warning:
                    Debug.LogWarning(p_message);
                    break;
                case LogLevel.Error:
                    Debug.LogError(p_message);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(p_logLevel), p_logLevel, null);
            }
#endif
        }
    }
}