using EcoToss.Core;
using TMPro;
using UnityEngine;

namespace EcoToss.UI
{
    /// <summary>
    /// HUD view for the Civic Karma bar. Listens only to GameEvents:
    /// KarmaChanged moves the fill and the "50/100" label, CorrectStreakChanged updates the streak label.
    /// The fill width is driven by the anchors of its RectTransform (0 = empty, 1 = full),
    /// so it works without a sprite. It never changes karma: KarmaSystem owns the rules.
    /// </summary>
    public class KarmaBarView : MonoBehaviour
    {
        #region Constants

        private const int MaxKarma = 100;

        #endregion

        #region Serialized Fields

        [Header("References")]
        [Tooltip("RectTransform of KarmaBarFill. Its right anchor is moved to show the karma percentage.")]
        [SerializeField] private RectTransform karmaBarFillRect;

        [Tooltip("Label that shows the karma as 'value/100'.")]
        [SerializeField] private TMP_Text karmaValueText;

        [Tooltip("Label that shows the current consecutive correct sorts.")]
        [SerializeField] private TMP_Text streakText;

        [Header("Animation")]
        [Tooltip("How fast the fill moves toward the new value, in bar widths per second (0.5 = a 10% change takes 0.2 s).")]
        [SerializeField, Min(0.01f)] private float fillAnimationSpeed = 0.5f;

        [Header("Text Formats")]
        [SerializeField] private string karmaValueFormat = "{0}/100";
        [SerializeField] private string streakFormat = "Racha: {0}";

        #endregion

        #region Private Fields

        private float displayedFillRatio;
        private float targetFillRatio;
        private bool hasReceivedFirstKarmaValue;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            if (karmaBarFillRect == null || karmaValueText == null || streakText == null)
            {
                Debug.LogError($"[{nameof(KarmaBarView)}] Missing references on '{name}'. Disabling.", this);
                enabled = false;
                return;
            }

            // The fill grows from the left edge and always covers the full height of the background.
            karmaBarFillRect.anchorMin = Vector2.zero;
            karmaBarFillRect.pivot = new Vector2(0f, 0.5f);
            karmaBarFillRect.offsetMin = Vector2.zero;
            karmaBarFillRect.offsetMax = Vector2.zero;
        }

        private void OnEnable()
        {
            GameEvents.KarmaChanged += HandleKarmaChanged;
            GameEvents.CorrectStreakChanged += HandleCorrectStreakChanged;
        }

        private void OnDisable()
        {
            GameEvents.KarmaChanged -= HandleKarmaChanged;
            GameEvents.CorrectStreakChanged -= HandleCorrectStreakChanged;
        }

        private void Update()
        {
            if (Mathf.Approximately(displayedFillRatio, targetFillRatio)) return;

            // Unscaled time keeps the animation readable even if the game is paused right after a throw.
            displayedFillRatio = Mathf.MoveTowards(displayedFillRatio, targetFillRatio,
                fillAnimationSpeed * Time.unscaledDeltaTime);
            ApplyFillRatio(displayedFillRatio);
        }

        #endregion

        #region Event Handlers

        private void HandleKarmaChanged(int karmaPercent)
        {
            targetFillRatio = Mathf.Clamp01((float)karmaPercent / MaxKarma);
            karmaValueText.text = string.Format(karmaValueFormat, karmaPercent);

            // The first value (sent by KarmaSystem.Start) snaps, so the bar does not animate from empty.
            if (!hasReceivedFirstKarmaValue)
            {
                hasReceivedFirstKarmaValue = true;
                displayedFillRatio = targetFillRatio;
                ApplyFillRatio(displayedFillRatio);
            }
        }

        private void HandleCorrectStreakChanged(int currentStreak)
        {
            streakText.text = string.Format(streakFormat, currentStreak);
        }

        #endregion

        #region Fill Helpers

        private void ApplyFillRatio(float fillRatio)
        {
            karmaBarFillRect.anchorMax = new Vector2(fillRatio, 1f);
        }

        #endregion
    }
}
