using UnityEngine;

namespace EcoToss.Gameplay
{
    /// <summary>
    /// Computes and renders a parabolic trajectory preview using a LineRenderer
    /// before the waste item is launched.
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    public class ThrowTrajectory : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private WasteThrower wasteThrower;

        [Header("Trajectory Settings")]
        [SerializeField, Min(2)] private int pointCount = 60;
        [SerializeField, Min(0.1f)] private float previewDuration = 3f;

        private LineRenderer trajectoryLine;

        private void Awake()
        {
            // Configure the line renderer to draw points in world space coordinates
            trajectoryLine = GetComponent<LineRenderer>();
            trajectoryLine.useWorldSpace = true;
        }

        private void LateUpdate()
        {
            // Only display the trajectory preview while aiming (prior to launch)
            trajectoryLine.enabled = !wasteThrower.HasLaunched;

            if (wasteThrower.HasLaunched)
                return;

            trajectoryLine.positionCount = pointCount;

            Vector3 startPosition = wasteThrower.LaunchStartPosition;
            Vector3 initialVelocity = wasteThrower.LaunchVelocity;

            // Sample discrete positions along the ballistic flight path: p(t) = p0 + v0*t + 0.5*g*t^2
            for (int pointIndex = 0; pointIndex < pointCount; pointIndex++)
            {
                float time = pointIndex * previewDuration / (pointCount - 1);

                Vector3 position =
                    startPosition
                    + initialVelocity * time
                    + 0.5f * Physics.gravity * time * time;

                trajectoryLine.SetPosition(pointIndex, position);
            }
        }
    }
}