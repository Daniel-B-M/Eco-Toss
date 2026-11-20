using EcoToss.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EcoToss.Gameplay
{
    /// <summary>
    /// Single source of throw input (mouse aim, hold to charge, release to throw).
    /// Gameplay scripts read from here instead of the mouse, so the rule
    /// "input only works while Playing" lives in one place.
    /// It also locks the cursor while Playing and frees it in every other state.
    /// </summary>
    public class ThrowInputReader : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Cursor")]
        [Tooltip("Hide and lock the cursor to the game window while Playing.")]
        [SerializeField] private bool lockCursorWhilePlaying = true;

        #endregion

        #region Private Fields

        private bool isInputAllowed;

        #endregion

        #region Public Properties

        /// <summary>Mouse movement this frame. Zero when input is not allowed.</summary>
        public Vector2 AimDelta =>
            CanReadMouse ? Mouse.current.delta.ReadValue() : Vector2.zero;

        /// <summary>True while the charge button (left click) is held down.</summary>
        public bool IsChargeHeld =>
            CanReadMouse && Mouse.current.leftButton.isPressed;

        /// <summary>True only on the frame the charge button is released.</summary>
        public bool WasChargeReleasedThisFrame =>
            CanReadMouse && Mouse.current.leftButton.wasReleasedThisFrame;

        /// <summary>True when the match is in the Playing state.</summary>
        public bool IsInputAllowed => isInputAllowed;

        /// <summary>True if the cursor should be locked while Playing. The pause menu uses it to detect a lost cursor lock (WebGL Escape, Alt+Tab).</summary>
        public bool IsCursorLockEnabled => lockCursorWhilePlaying;

        private bool CanReadMouse => isInputAllowed && Mouse.current != null;

        #endregion

        #region Unity Lifecycle

        private void OnEnable()
        {
            GameEvents.GameStateChanged += HandleGameStateChanged;

            // In case the GameManager already announced its state before we subscribed.
            if (GameManager.Instance != null)
                HandleGameStateChanged(GameManager.Instance.CurrentState);
        }

        private void OnDisable()
        {
            GameEvents.GameStateChanged -= HandleGameStateChanged;
            isInputAllowed = false;
            ApplyCursorState(isLocked: false);
        }

        #endregion

        #region Event Handlers

        private void HandleGameStateChanged(GameState newState)
        {
            isInputAllowed = newState == GameState.Playing;
            ApplyCursorState(isLocked: isInputAllowed && lockCursorWhilePlaying);
        }

        #endregion

        #region Cursor

        /// <summary>Locked and hidden while Playing; free and visible for menus (briefing, pause, end screens).</summary>
        private static void ApplyCursorState(bool isLocked)
        {
            Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !isLocked;
        }

        #endregion
    }
}
