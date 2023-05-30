#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

namespace _AsteroidsDOTS.Scripts.NonDOTSBehaviour.UI
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private Button m_playButton;
        [SerializeField] private Button m_exitButton;

        private void Awake()
        {
            m_playButton.onClick.AddListener(PlayGame);
            m_exitButton.onClick.AddListener(ExitGame);
        }

        private void PlayGame()
        {
            GameManager.Instance.StartGame();
        }

        private void ExitGame()
        {
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();

#else
            Application.Quit();
#endif
        }
    }
}