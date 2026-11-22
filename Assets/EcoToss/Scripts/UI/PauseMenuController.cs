using EcoToss.Core;
using EcoToss.Gameplay;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace EcoToss.UI
{
    /// <summary>
    /// Pause menu of the Gameplay scene (PauseOverlay).
    /// - P or Escape pauses while Playing, and goes back / resumes while Paused.
    /// - Auto-pauses when the window loses focus or the cursor lock is lost
    ///   (in WebGL the browser takes the first Escape to free the cursor).
    /// - Buttons: Resume, How to play (briefing in reference mode), Settings, Restart and Main menu
    ///   (the last two ask for confirmation).
    /// It only asks the GameManager to change state and reacts to GameStateChanged to show or hide itself.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class PauseMenuController : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Gameplay Sources")]
        [Tooltip("Used to know if the cursor should be locked while Playing (auto-pause on lost lock).")]
        [SerializeField] private ThrowInputReader throwInputReader;
        [SerializeField] private BriefingOverlayController briefingOverlay;

        [Header("Panels")]
        [SerializeField] private GameObject pauseMenuPanel;
        [SerializeField] private GameObject restartConfirmationPanel;
        [SerializeField] private GameObject exitConfirmationPanel;
        [SerializeField] private GameObject settingsPanel;

        [Header("Pause Menu Buttons")]
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button showBriefingButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button mainMenuButton;

        [Header("Confirmation Buttons")]
        [SerializeField] private Button restartConfirmButton;
        [SerializeField] private Button restartCancelButton;
        [SerializeField] private Button exitConfirmButton;
        [SerializeField] private Button exitCancelButton;

        [Header("Settings Buttons")]
        [SerializeField] private Button settingsBackButton;

        [Header("Scenes")]
        [SerializeField] private string mainMenuSceneName = "MainMenu";

        #endregion

        #region Private Fields

        private CanvasGroup overlayCanvasGroup;

        // Edge detection: only a change from locked to unlocked pauses, so a browser that
        // refuses to re-lock the cursor right after resuming can't cause a pause loop.
        private bool wasCursorLockedLastFrame;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            overlayCanvasGroup = GetComponent<CanvasGroup>();
            RegisterButtonListeners();

            // Hidden until the GameManager announces the Paused state.
            SetVisible(false);
        }

        private void OnEnable()
        {
            GameEvents.GameStateChanged += HandleGameStateChanged;
            briefingOverlay.ReferenceClosed += HandleBriefingReferenceClosed;
        }

        private void OnDisable()
        {
            GameEvents.GameStateChanged -= HandleGameStateChanged;
            briefingOverlay.ReferenceClosed -= HandleBriefingReferenceClosed;
        }

        private void Update()
        {
            if (GameManager.Instance == null) return;

            bool wasPauseKeyPressed = WasPauseKeyPressedThisFrame();

            switch (GameManager.Instance.CurrentState)
            {
                case GameState.Playing:
                    if (wasPauseKeyPressed || WasCursorLockLostThisFrame())
                        RequestPause();
                    break;

                case GameState.Paused:
                    if (wasPauseKeyPressed)
                        HandleBackAction();
                    break;
            }

            wasCursorLockedLastFrame = Cursor.lockState == CursorLockMode.Locked;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            // Alt+Tab or switching browser tabs pauses the match.
            if (!hasFocus && IsState(GameState.Playing))
                RequestPause();
        }

        #endregion

        #region Input

        private static bool WasPauseKeyPressedThisFrame()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null) return false;

            return keyboard.pKey.wasPressedThisFrame || keyboard.escapeKey.wasPressedThisFrame;
        }

        private bool WasCursorLockLostThisFrame()
        {
            if (throwInputReader == null || !throwInputReader.IsCursorLockEnabled) return false;

            return wasCursorLockedLastFrame && Cursor.lockState != CursorLockMode.Locked;
        }

        /// <summary>P/Escape while paused: close the open layer first, resume only from the main pause panel.</summary>
        private void HandleBackAction()
        {
            if (briefingOverlay.IsOpenAsReference)
                briefingOverlay.CloseReference();
            else if (!pauseMenuPanel.activeSelf)
                ShowPausePanel();
            else
                RequestResume();
        }

        #endregion

        #region Event Handlers

        private void HandleGameStateChanged(GameState newState)
        {
            bool isPaused = newState == GameState.Paused;
            SetVisible(isPaused);

            if (isPaused)
                ShowPausePanel();
        }

        private void HandleBriefingReferenceClosed()
        {
            SetVisible(true);
            ShowPausePanel();
        }

        #endregion

        #region Button Actions

        private void RegisterButtonListeners()
        {
            resumeButton.onClick.AddListener(RequestResume);
            showBriefingButton.onClick.AddListener(OpenBriefingReference);
            settingsButton.onClick.AddListener(() => ShowPanel(settingsPanel, settingsBackButton));
            restartButton.onClick.AddListener(() => ShowPanel(restartConfirmationPanel, restartCancelButton));
            mainMenuButton.onClick.AddListener(() => ShowPanel(exitConfirmationPanel, exitCancelButton));

            restartConfirmButton.onClick.AddListener(RestartMatch);
            restartCancelButton.onClick.AddListener(ShowPausePanel);
            exitConfirmButton.onClick.AddListener(GoToMainMenu);
            exitCancelButton.onClick.AddListener(ShowPausePanel);

            settingsBackButton.onClick.AddListener(ShowPausePanel);
        }

        private void RequestPause() => GameManager.Instance.ChangeState(GameState.Paused);
        private void RequestResume() => GameManager.Instance.ChangeState(GameState.Playing);

        private void OpenBriefingReference()
        {
            // The pause overlay hides while the briefing is on screen and comes back on ReferenceClosed.
            SetVisible(false);
            briefingOverlay.OpenAsReference();
        }

        private static void RestartMatch()
        {
            // GameManager.OnDestroy restores timeScale, and the briefing is skipped because it was already seen.
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void GoToMainMenu()
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }

        #endregion

        #region Panel Navigation

        private void ShowPausePanel() => ShowPanel(pauseMenuPanel, resumeButton);

        /// <summary>Shows one panel of the overlay and selects a button for keyboard navigation.</summary>
        private void ShowPanel(GameObject panelToShow, Button buttonToSelect)
        {
            pauseMenuPanel.SetActive(panelToShow == pauseMenuPanel);
            restartConfirmationPanel.SetActive(panelToShow == restartConfirmationPanel);
            exitConfirmationPanel.SetActive(panelToShow == exitConfirmationPanel);
            settingsPanel.SetActive(panelToShow == settingsPanel);

            if (EventSystem.current == null) return;
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(buttonToSelect.gameObject);
        }

        /// <summary>
        /// Uses a CanvasGroup instead of SetActive so this object keeps running Update
        /// (pause keys, auto-pause) and listening to events while hidden.
        /// </summary>
        private void SetVisible(bool isVisible)
        {
            overlayCanvasGroup.alpha = isVisible ? 1f : 0f;
            overlayCanvasGroup.interactable = isVisible;
            overlayCanvasGroup.blocksRaycasts = isVisible;
        }

        private static bool IsState(GameState state) =>
            GameManager.Instance != null && GameManager.Instance.CurrentState == state;

        #endregion
    }
}
