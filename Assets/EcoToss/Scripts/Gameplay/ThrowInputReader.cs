using EcoToss.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EcoToss.Gameplay
{
    /// <summary>
    /// Single source of throw input: mouse or keyboard (WASD / arrows) to aim,
    /// hold left click or Space to charge, release to throw.
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

        /// <summary>Keyboard aim direction (WASD / arrows), each axis between -1 and 1. Zero when input is not allowed.</summary>
        public Vector2 KeyboardAimAxis =>
            CanReadKeyboard ? ReadKeyboardAimAxis(Keyboard.current) : Vector2.zero;

        /// <summary>True while the charge button (left click or Space) is held down.</summary>
        public bool IsChargeHeld => IsMouseChargeHeld || IsKeyboardChargeHeld;

        /// <summary>
        /// True only on the frame the charge is released.
        /// If both buttons are held, the throw happens when the last one is released.
        /// </summary>
        public bool WasChargeReleasedThisFrame =>
            (WasMouseChargeReleasedThisFrame || WasKeyboardChargeReleasedThisFrame) && !IsChargeHeld;

        /// <summary>True when the match is in the Playing state.</summary>
        public bool IsInputAllowed => isInputAllowed;

        /// <summary>True if the cursor should be locked while Playing. The pause menu uses it to detect a lost cursor lock (WebGL Escape, Alt+Tab).</summary>
        public bool IsCursorLockEnabled => lockCursorWhilePlaying;

        #endregion

        #region Device Checks

        private bool CanReadMouse => isInputAllowed && Mouse.current != null;
        private bool CanReadKeyboard => isInputAllowed && Keyboard.current != null;

        private bool IsMouseChargeHeld => CanReadMouse && Mouse.current.leftButton.isPressed;
        private bool IsKeyboardChargeHeld => CanReadKeyboard && Keyboard.current.spaceKey.isPressed;

        private bool WasMouseChargeReleasedThisFrame =>
            CanReadMouse && Mouse.current.leftButton.wasReleasedThisFrame;
        private bool WasKeyboardChargeReleasedThisFrame =>
            CanReadKeyboard && Keyboard.current.spaceKey.wasReleasedThisFrame;

        /// <summary>Builds the aim direction from WASD and the arrow keys (both work at the same time).</summary>
        private static Vector2 ReadKeyboardAimAxis(Keyboard keyboard)
        {
            float horizontal = 0f;
            float vertical = 0f;

            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) horizontal += 1f;
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) horizontal -= 1f;
            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) vertical += 1f;
            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) vertical -= 1f;

            return new Vector2(horizontal, vertical);
        }


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
