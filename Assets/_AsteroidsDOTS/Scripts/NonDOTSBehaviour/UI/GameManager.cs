using System;
using _AsteroidsDOTS.Scripts.Globals;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _AsteroidsDOTS.Scripts.NonDOTSBehaviour.UI
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private static void SetGameState(bool p_hasStarted)
        {
            AsteroidsDOTS.GameIsStarted = p_hasStarted;
        }

        public void FinishGame()
        {
            SetGameState(false);
            SceneManager.LoadScene("MainMenu");
        }

        public void StartGame()
        {
            SceneManager.LoadScene("Game");
            SetGameState(true);
        }
    }
}