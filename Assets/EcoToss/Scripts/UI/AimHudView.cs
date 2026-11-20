using EcoToss.Gameplay;
using TMPro;
using UnityEngine;

namespace EcoToss.UI
{
    /// <summary>
    /// HUD view for the aiming turn: the countdown (TimerPanel) and the launch charge (PowerPanel).
    /// Both values change every frame, so they are read in Update instead of using events.
    /// It only reads AimTimer and WasteThrower: it never starts, stops or launches anything.
    /// The values freeze while the waste is flying and reset when the next waste appears,
    /// because AimTimer and WasteThrower already behave that way.
    /// </summary>
    public class AimHudView : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Gameplay Sources")]
        [SerializeField] private AimTimer aimTimer;
        [SerializeField] private WasteThrower wasteThrower;

        [Header("Timer Panel")]
        [Tooltip("Label that shows the remaining aiming time in whole seconds.")]
        [SerializeField] private TMP_Text timerValueText;

        [Header("Power Panel")]
        [Tooltip("RectTransform of PowerBarFill. Its right anchor is moved to show the charge (0 to 1).")]
        [SerializeField] private RectTransform powerBarFillRect;

        [Header("Text Formats")]
        [SerializeField] private string timerSecondsFormat = "{0} s";
        [SerializeField] private string timerDisabledText = "Sin límite";

        #endregion

        #region Private Fields

        // Cached so the label is only rebuilt when the shown second changes (no string garbage per frame).
        private int lastShownSeconds = -1;
        private bool wasShowingDisabledText;

        // Before the first turn AimTimer reports 0, so the scene text is kept until a countdown starts.
        private bool hasAnyTurnStarted;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            if (aimTimer == null || wasteThrower == null || timerValueText == null || powerBarFillRect == null)
            {
                Debug.LogError($"[{nameof(AimHudView)}] Missing references on '{name}'. Disabling.", this);
                enabled = false;
                return;
            }

            // The fill grows from the left edge and always covers the full height of the background.
            powerBarFillRect.anchorMin = Vector2.zero;
            powerBarFillRect.pivot = new Vector2(0f, 0.5f);
            powerBarFillRect.offsetMin = Vector2.zero;
            powerBarFillRect.offsetMax = Vector2.zero;
            ApplyPowerFill(0f);
        }

        private void Update()
        {
            RefreshTimerLabel();
            ApplyPowerFill(wasteThrower.ChargePercent);
        }

        #endregion

        #region Timer Panel

        private void RefreshTimerLabel()
        {
            if (!aimTimer.AutoThrowEnabled)
            {
                ShowTimerDisabledText();
                return;
            }

            if (aimTimer.IsRunning) hasAnyTurnStarted = true;
            if (!hasAnyTurnStarted) return;

            // Ceil so the countdown reads 5, 4, 3, 2, 1 and only shows 0 when the time is really over.
            int secondsToShow = Mathf.CeilToInt(aimTimer.RemainingTime);
            if (secondsToShow == lastShownSeconds && !wasShowingDisabledText) return;

            lastShownSeconds = secondsToShow;
            wasShowingDisabledText = false;
            timerValueText.text = string.Format(timerSecondsFormat, secondsToShow);
        }

        private void ShowTimerDisabledText()
        {
            if (wasShowingDisabledText) return;

            wasShowingDisabledText = true;
            timerValueText.text = timerDisabledText;
        }

        #endregion

        #region Power Panel

        private void ApplyPowerFill(float chargeRatio)
        {
            powerBarFillRect.anchorMax = new Vector2(Mathf.Clamp01(chargeRatio), 1f);
        }

        #endregion
    }
}
