using System;
using UnityEngine;

namespace EcoToss.Gameplay
{
    /// <summary>
    /// Countdown for each aiming turn. When it reaches zero it raises TimeExpired
    /// so the thrower can auto-launch. Freezes whenever input is not allowed
    /// (briefing, pause, end screens). Accessibility can disable the auto throw.
    /// </summary>
    public class AimTimer : MonoBehaviour
    {
        #region Serialized Fields

        [Header("References")]
        [SerializeField] private ThrowInputReader throwInputReader;

        [Header("Timer Settings")]
        [SerializeField, Min(0.1f)] private float aimDuration = 5f;

        [Tooltip("If false, the timer never expires (Accessibility / Assisted Mode).")]
        [SerializeField] private bool autoThrowEnabled = true;

        [Header("Temporary Debug HUD (removed in Phase 5)")]
        [SerializeField] private Font debugHudFont;

        #endregion

        #region Events

        /// <summary>Raised once when the countdown reaches zero.</summary>
        public event Action TimeExpired;

        #endregion

        #region Public Properties

        public float RemainingTime { get; private set; }
        public bool IsRunning { get; private set; }

        /// <summary>Entry point for the Accessibility teammate. Disabling it stops any running countdown.</summary>
        public bool AutoThrowEnabled
        {
            get => autoThrowEnabled;
            set
            {
                autoThrowEnabled = value;
                if (!autoThrowEnabled) RemainingTime = aimDuration;
            }
        }

        #endregion

        #region Unity Lifecycle

        private void Update()
        {
            if (!IsRunning || !autoThrowEnabled) return;
            if (!throwInputReader.IsInputAllowed) return;

            RemainingTime = Mathf.Max(0f, RemainingTime - Time.deltaTime);

            if (RemainingTime <= 0f)
            {
                IsRunning = false;
                TimeExpired?.Invoke();
            }
        }

        #endregion

        #region Public API

        /// <summary>Restarts the countdown for a new aiming turn.</summary>
        public void Begin()
        {
            RemainingTime = aimDuration;
            IsRunning = true;
        }

        /// <summary>Stops the countdown without raising TimeExpired (e.g. the player threw manually).</summary>
        public void Stop()
        {
            IsRunning = false;
        }

        #endregion

        #region Temporary Debug HUD

        private void OnGUI()
        {
            if (!IsRunning) return;

            GUIStyle style = new GUIStyle(GUI.skin.box)
            {
                font = debugHudFont,
                fontSize = 28,
                alignment = TextAnchor.MiddleCenter
            };
            style.normal.textColor = Color.white;

            string message = autoThrowEnabled ? $"Tiempo: {RemainingTime:0.0} s" : "Sin límite de tiempo";
            GUI.Box(new Rect(20, 20, 320, 60), message, style);
        }

        #endregion
    }
}
