using UnityEngine;
using UnityEngine.InputSystem;

namespace EcoToss.Gameplay
{
    /// <summary>
    /// Handles mouse-driven aiming rotation for targeting the waste throw,
    /// clamping horizontal and vertical angles within configurable limits.
    /// </summary>
    public class WasteAim : MonoBehaviour
    {
        [Header("Control del mouse")]
        [SerializeField] private float mouseSensitivity = 0.1f;

        [Header("Límites de apuntado (grados)")]
        [SerializeField] private float maxHorizontalAngle = 45f;
        [SerializeField] private float minVerticalAngle = 10f;
        [SerializeField] private float maxVerticalAngle = 70f;
        [SerializeField] private float initialVerticalAngle = 35f;

        // Current rotation angles in degrees
        private float horizontalAngle;
        private float verticalAngle;

        private void Awake()
        {
            // Set the initial vertical pitch angle
            verticalAngle = initialVerticalAngle;
        }

        private void Update()
        {
            if (Mouse.current == null)
                return;

            // Read delta input and accumulate aiming angles
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();

            horizontalAngle += mouseDelta.x * mouseSensitivity;
            verticalAngle += mouseDelta.y * mouseSensitivity;

            // Restrict angles within allowed boundary constraints
            horizontalAngle = Mathf.Clamp(horizontalAngle, -maxHorizontalAngle, maxHorizontalAngle);
            verticalAngle = Mathf.Clamp(verticalAngle, minVerticalAngle, maxVerticalAngle);

            // Apply clamped pitch and yaw rotation to the aim transform
            transform.localRotation = Quaternion.Euler(-verticalAngle, horizontalAngle, 0f);
        }

        private void OnDrawGizmos()
        {
            // Visualize the aiming direction ray in the Scene view
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, transform.forward * 3f);
        }
    }
}
