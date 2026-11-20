using EcoToss.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace EcoToss.UI
{
    /// <summary>
    /// Victory and defeat screens (EndGameOverlay).
    /// Shows the matching panel when GameStateChanged reports Victory or Defeat,
    /// with a static background image (clean or dirty square).
    /// Replay reloads the Gameplay scene; Main menu loads the menu scene.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class EndGameOverlayController : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Background")]
        [Tooltip("Full-screen image behind the panels. Hidden while its sprite is missing.")]
        [SerializeField] private Image endGameBackgroundImage;
        [SerializeField] private Sprite victoryBackgroundSprite;
        [SerializeField] private Sprite defeatBackgroundSprite;

        [Header("Victory Panel")]
        [SerializeField] private GameObject victoryPanel;
        [SerializeField] private Button victoryReplayButton;
        [SerializeField] private Button victoryMainMenuButton;

        [Header("Defeat Panel")]
        [SerializeField] private GameObject defeatPanel;
        [SerializeField] private Button defeatReplayButton;
        [SerializeField] private Button defeatMainMenuButton;
        [Header("Scenes")]
        [SerializeField] private string mainMenuSceneName = "MainMenu";

        #endregion

        #region Private Fields

        private CanvasGroup overlayCanvasGroup;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            overlayCanvasGroup = GetComponent<CanvasGroup>();

            victoryReplayButton.onClick.AddListener(ReplayMatch);
            defeatReplayButton.onClick.AddListener(ReplayMatch);
            victoryMainMenuButton.onClick.AddListener(GoToMainMenu);
            defeatMainMenuButton.onClick.AddListener(GoToMainMenu);

            // Hidden until the GameManager announces Victory or Defeat.
            SetVisible(false);
        }

        private void OnEnable()
        {
            GameEvents.GameStateChanged += HandleGameStateChanged;
        }

        private void OnDisable()
        {
            GameEvents.GameStateChanged -= HandleGameStateChanged;
        }

        #endregion

        #region Event Handlers
        private void HandleGameStateChanged(GameState newState)
        {
            switch (newState)
            {
                case GameState.Victory:
                    ShowResult(isVictory: true);
                    break;

                case GameState.Defeat:
                    ShowResult(isVictory: false);
                    break;

                default:
                    SetVisible(false);
                    break;
            }
        }

        #endregion

        #region Result Screen

        private void ShowResult(bool isVictory)
        {
            victoryPanel.SetActive(isVictory);
            defeatPanel.SetActive(!isVictory);
            ApplyBackground(isVictory ? victoryBackgroundSprite : defeatBackgroundSprite);
            SetVisible(true);

            SelectButton(isVictory ? victoryReplayButton : defeatReplayButton);
        }

        private void ApplyBackground(Sprite backgroundSprite)
        {
            if (endGameBackgroundImage == null) return;

            endGameBackgroundImage.sprite = backgroundSprite;
            endGameBackgroundImage.enabled = backgroundSprite != null;
        }

        #endregion

        #region Button Actions

        private static void ReplayMatch()
        {
            // The briefing is skipped because it was already seen in this session.
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void GoToMainMenu()
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }

        #endregion

        #region View Helpers

        private static void SelectButton(Button buttonToSelect)
        {
            if (EventSystem.current == null) return;
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(buttonToSelect.gameObject);
        }

        /// <summary>CanvasGroup instead of SetActive so the overlay keeps listening to events while hidden.</summary>
        private void SetVisible(bool isVisible)
        {
            overlayCanvasGroup.alpha = isVisible ? 1f : 0f;
            overlayCanvasGroup.interactable = isVisible;
            overlayCanvasGroup.blocksRaycasts = isVisible;
        }

        #endregion
    }
}
