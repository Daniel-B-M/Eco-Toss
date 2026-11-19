using UnityEngine;

namespace EcoToss.Gameplay
{
    [RequireComponent(typeof(LineRenderer))]
    public class ThrowTrajectory : MonoBehaviour
    {
        [SerializeField] private WasteThrower wasteThrower;

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

            Vector3 startPosition = wasteThrower.LaunchStartPosition;
            Vector3 initialVelocity = wasteThrower.LaunchVelocity;

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