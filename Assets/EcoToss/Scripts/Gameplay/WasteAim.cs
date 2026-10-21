using UnityEngine;
using UnityEngine.InputSystem;

public class WasteAim : MonoBehaviour
{
    [SerializeField] private float sensitivity = 0.1f;
    [SerializeField] private float horizontalLimit = 45f;
    [SerializeField] private float minElevation = 10f;
    [SerializeField] private float maxElevation = 70f;

    private float horizontalAngle;
    private float verticalAngle = 35f;

    private void Update()
    {
        if (Mouse.current == null)
            return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        horizontalAngle += mouseDelta.x * sensitivity;
        verticalAngle += mouseDelta.y * sensitivity;

        horizontalAngle = Mathf.Clamp(horizontalAngle, -horizontalLimit, horizontalLimit);
        verticalAngle = Mathf.Clamp(verticalAngle, minElevation, maxElevation);

        transform.localRotation = Quaternion.Euler(-verticalAngle, horizontalAngle, 0f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, transform.forward * 3f);
    }
}
