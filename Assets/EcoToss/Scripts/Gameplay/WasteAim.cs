using UnityEngine;
using UnityEngine.InputSystem;

namespace EcoToss.Gameplay
{
    public class WasteAim : MonoBehaviour
    {
        [Header("Control del mouse")]
        [SerializeField] private float mouseSensitivity = 0.1f;

        [Header("Límites de apuntado (grados)")]
        [SerializeField] private float maxHorizontalAngle = 45f;
        [SerializeField] private float minVerticalAngle = 10f;
        [SerializeField] private float maxVerticalAngle = 70f;
        [SerializeField] private float initialVerticalAngle = 35f;

        private float horizontalAngle;
        private float verticalAngle;

        private void Awake()
        {
            verticalAngle = initialVerticalAngle;
        }

        private void Update()
        {
            if (Mouse.current == null)
                return;

            Vector2 mouseDelta = Mouse.current.delta.ReadValue();

            horizontalAngle += mouseDelta.x * mouseSensitivity;
            verticalAngle += mouseDelta.y * mouseSensitivity;

            horizontalAngle = Mathf.Clamp(horizontalAngle, -maxHorizontalAngle, maxHorizontalAngle);
            verticalAngle = Mathf.Clamp(verticalAngle, minVerticalAngle, maxVerticalAngle);

            transform.localRotation = Quaternion.Euler(-verticalAngle, horizontalAngle, 0f);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, transform.forward * 3f);
        }
    }
}
