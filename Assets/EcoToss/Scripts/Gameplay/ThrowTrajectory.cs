using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ThrowTrajectory : MonoBehaviour
{
    [SerializeField] private WasteThrower wasteThrower;
    [SerializeField] private Transform throwOrigin;

    [SerializeField, Min(2)] private int pointCount = 60;
    [SerializeField, Min(0.1f)] private float previewDuration = 3f;

    private LineRenderer trajectoryLine;

    private void Awake()
    {
        trajectoryLine = GetComponent<LineRenderer>();
        trajectoryLine.useWorldSpace = true;
    }

    private void LateUpdate()
    {
        trajectoryLine.enabled = !wasteThrower.HasLaunched;

        if (wasteThrower.HasLaunched)
            return;

        trajectoryLine.positionCount = pointCount;

        Vector3 startPosition = throwOrigin.position;
        Vector3 initialVelocity = wasteThrower.LaunchVelocity;

        for (int i = 0; i < pointCount; i++)
        {
            float time = i * previewDuration / (pointCount - 1);

            Vector3 position =
                startPosition
                + initialVelocity * time
                + 0.5f * Physics.gravity * time * time;

            trajectoryLine.SetPosition(i, position);
        }
    }
}