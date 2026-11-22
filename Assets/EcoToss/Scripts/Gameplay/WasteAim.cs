using EcoToss.Core;
using UnityEngine;

namespace EcoToss.Gameplay
{
    /// <summary>
    /// Rotates the aim pivot with the mouse or the keyboard (WASD / arrows), clamping horizontal and vertical angles.
    /// Reads input from ThrowInputReader (so it only moves while Playing)
    /// and resets to the initial angle every time a new waste is spawned.
    /// </summary>
    public class WasteAim : MonoBehaviour
    {
        #region Serialized Fields

        [Header("References")]
        [SerializeField] private ThrowInputReader throwInputReader;

        [Header("Mouse Control")]
        [SerializeField] private float mouseSensitivity = 0.1f;

        [Header("Keyboard Control")]
        [Tooltip("How fast the aim turns while a WASD / arrow key is held, in degrees per second.")]
        [SerializeField] private float keyboardAimSpeed = 60f;

        [Header("Aim Limits (degrees)")]
        [SerializeField] private float maxHorizontalAngle = 45f;
        [SerializeField] private float minVerticalAngle = 10f;
        [SerializeField] private float maxVerticalAngle = 70f;
        [SerializeField] private float initialVerticalAngle = 35f;

        #endregion

        #region Private Fields

        private float horizontalAngle;
        private float verticalAngle;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            ResetAim();
        }

        private void OnEnable()
        {
            GameEvents.WasteSpawned += HandleWasteSpawned;
        }

        private void OnDisable()
        {
            GameEvents.WasteSpawned -= HandleWasteSpawned;
        }

        private void Update()
        {
            Vector2 mouseAngleDelta = throwInputReader.AimDelta * mouseSensitivity;
            Vector2 keyboardAngleDelta = throwInputReader.KeyboardAimAxis * (keyboardAimSpeed * Time.deltaTime);
            Vector2 totalAngleDelta = mouseAngleDelta + keyboardAngleDelta;

            if (totalAngleDelta == Vector2.zero) return;

            horizontalAngle += totalAngleDelta.x;
            verticalAngle += totalAngleDelta.y;

            horizontalAngle = Mathf.Clamp(horizontalAngle, -maxHorizontalAngle, maxHorizontalAngle);
            verticalAngle = Mathf.Clamp(verticalAngle, minVerticalAngle, maxVerticalAngle);

            ApplyRotation();
        }


        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, transform.forward * 3f);
        }

        #endregion

        #region Public API

        /// <summary>Points the aim straight ahead at the initial vertical angle.</summary>
        public void ResetAim()
        {
            horizontalAngle = 0f;
            verticalAngle = initialVerticalAngle;
            ApplyRotation();
        }

        #endregion

        #region Private Methods

        private void HandleWasteSpawned(WasteData spawnedWaste)
        {
            ResetAim();
        }

        private void ApplyRotation()
        {
            transform.localRotation = Quaternion.Euler(-verticalAngle, horizontalAngle, 0f);
        }

        #endregion
    }
}
