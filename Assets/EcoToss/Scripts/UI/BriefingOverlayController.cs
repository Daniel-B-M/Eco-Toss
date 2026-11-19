using EcoToss.Core;
using UnityEngine;
using UnityEngine.UI;

namespace EcoToss.UI
{
    /// <summary>
    /// Shows the briefing overlay while the match is in the Briefing state.
    /// It never hides itself directly: it asks the GameManager to start the match
    /// and reacts to GameEvents.GameStateChanged.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class BriefingOverlayController : MonoBehaviour
    {
        [SerializeField] private Button startMatchButton;

        private CanvasGroup overlayCanvasGroup;

        private void Awake()
        {
            overlayCanvasGroup = GetComponent<CanvasGroup>();
            startMatchButton.onClick.AddListener(StartMatch);

            // Hidden until the GameManager announces the Briefing state.
            SetVisible(false);
        }

        private void OnEnable() => GameEvents.GameStateChanged += OnGameStateChanged;
        private void OnDisable() => GameEvents.GameStateChanged -= OnGameStateChanged;

        private void OnGameStateChanged(GameState newState)
        {
            SetVisible(newState == GameState.Briefing);
        }

        private void StartMatch()
        {
            GameManager.Instance.ChangeState(GameState.Playing);
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
    }
}
