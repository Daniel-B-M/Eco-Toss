using System;
using EcoToss.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EcoToss.UI
{
    /// <summary>
    /// Shows the briefing overlay in two modes:
    /// - Match start: visible while the match is in the Briefing state. The button asks the
    ///   GameManager to start playing; the overlay reacts to GameEvents.GameStateChanged.
    /// - Reference ("Cómo jugar" from the pause menu): opened on demand without changing the state.
    ///   The button reads "Volver" and closing it raises ReferenceClosed so the pause menu comes back.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class BriefingOverlayController : MonoBehaviour
    {
        #region Serialized Fields

        [Header("References")]
        [SerializeField] private Button startMatchButton;
        [SerializeField] private TMP_Text startMatchButtonLabel;

        [Tooltip("Reminder about the first throw. Hidden in reference mode because the match already started.")]
        [SerializeField] private GameObject firstThrowReminder;

        [Header("Button Texts")]
        [SerializeField] private string startMatchButtonText = "¡Entendido, comenzar!";
        [SerializeField] private string referenceBackButtonText = "Volver";

        #endregion

        #region Private Fields

        private CanvasGroup overlayCanvasGroup;
        private bool isReferenceMode;

        #endregion

        #region Events

        /// <summary>Raised when the overlay opened in reference mode is closed. The pause menu listens to this.</summary>
        public event Action ReferenceClosed;

        #endregion

        #region Public Properties

        public bool IsOpenAsReference => isReferenceMode;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            overlayCanvasGroup = GetComponent<CanvasGroup>();
            startMatchButton.onClick.AddListener(HandleMainButtonClicked);

            // Hidden until the GameManager announces the Briefing state.
            SetVisible(false);
        }

        private void OnEnable() => GameEvents.GameStateChanged += OnGameStateChanged;
        private void OnDisable() => GameEvents.GameStateChanged -= OnGameStateChanged;

        #endregion

        #region Public API

        /// <summary>Opens the briefing for consultation ("Cómo jugar") without changing the match state.</summary>
        public void OpenAsReference()
        {
            isReferenceMode = true;
            ApplyModeTexts();
            SetVisible(true);
        }

        /// <summary>Closes the reference view and notifies the pause menu. Does nothing in match-start mode.</summary>
        public void CloseReference()
        {
            if (!isReferenceMode) return;

            isReferenceMode = false;
            ApplyModeTexts();
            SetVisible(false);
            ReferenceClosed?.Invoke();
        }

        #endregion

        #region Event Handlers

        private void OnGameStateChanged(GameState newState)
        {
            // Any state change (e.g. resuming with P while consulting) leaves reference mode silently.
            isReferenceMode = false;
            ApplyModeTexts();
            SetVisible(newState == GameState.Briefing);
        }

        private void HandleMainButtonClicked()
        {
            if (isReferenceMode)
                CloseReference();
            else
                GameManager.Instance.ChangeState(GameState.Playing);
        }

        #endregion

        #region View Helpers

        private void ApplyModeTexts()
        {
            if (startMatchButtonLabel != null)
                startMatchButtonLabel.text = isReferenceMode ? referenceBackButtonText : startMatchButtonText;

            if (firstThrowReminder != null)
                firstThrowReminder.SetActive(!isReferenceMode);
        }

        /// <summary>
        /// Uses a CanvasGroup instead of SetActive so this object stays active
        /// and keeps listening to events while hidden.
        /// </summary>
        private void SetVisible(bool isVisible)
        {
            overlayCanvasGroup.alpha = isVisible ? 1f : 0f;
            overlayCanvasGroup.interactable = isVisible;
            overlayCanvasGroup.blocksRaycasts = isVisible;
        }

        #endregion
    }
}
