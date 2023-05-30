using _AsteroidsDOTS.Scripts.DataComponents.Asteroids;
using _AsteroidsDOTS.Scripts.DataComponents.Enemies;
using _AsteroidsDOTS.Scripts.DataComponents.Powerups;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace _AsteroidsDOTS.Scripts.NonDOTSBehaviour.UI
{
    public class InGameUI : MonoBehaviour
    {
        [SerializeField] private GameObject m_gameFinishedPanel;
        [SerializeField] private Button m_retryButton;
        [SerializeField] private Button m_backToMenuButton;
        [SerializeField] private HelpPanel m_helpPanel;

        private void Start()
        {
            m_helpPanel.Init();
            m_gameFinishedPanel.SetActive(false);
        }

        public void SetGameFinishedPopup()
        {
            m_gameFinishedPanel.SetActive(true);
        }

        private void OnEnable()
        {
            m_retryButton.onClick.AddListener(Restart);
            m_backToMenuButton.onClick.AddListener(QuitLevel);
        }

        private void OnDisable()
        {
            m_retryButton.onClick.RemoveListener(Restart);
            m_backToMenuButton.onClick.RemoveListener(QuitLevel);
        }

        private void Restart()
        {
            if (GameManager.Instance != null)
            {
                CleanupScene();
                GameManager.Instance.StartGame();
            }
        }

        private void CleanupScene()
        {
            var l_entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var l_universalQuery = l_entityManager.UniversalQuery;
            l_entityManager.DestroyEntity(l_universalQuery);
            ScriptBehaviourUpdateOrder.RemoveWorldFromCurrentPlayerLoop(World.DefaultGameObjectInjectionWorld);
            DefaultWorldInitialization.Initialize("Game World");
        }

        private void QuitLevel()
        {
            if (GameManager.Instance != null)
            {
                CleanupScene();
                GameManager.Instance.FinishGame();
            }
        }

        private void SetPlayerInitials(string p_initials)
        {
            string l_allCaps = p_initials.ToUpper();

            //Save data to historic table
        }
    }
}