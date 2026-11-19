using UnityEngine;

namespace EcoToss.Gameplay
{
    /// <summary>
    /// Draws a parabolic preview of the throw with a LineRenderer.
    /// Only visible while the WasteThrower is aiming a waste, and the line
    /// stops at the first obstacle it hits (bin rim, floor...).
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    public class ThrowTrajectory : MonoBehaviour
    {
        #region Constants

        private const int MaxHitsPerSegment = 8;

        #endregion

        #region Serialized Fields

        [Header("References")]
        [SerializeField] private WasteThrower wasteThrower;

        [Header("Trajectory Settings")]
        [SerializeField, Min(2)] private int pointCount = 60;
        [SerializeField, Min(0.1f)] private float previewDuration = 3f;

        [Header("Collision")]
        [Tooltip("Layers that cut the line. Triggers are always ignored.")]
        [SerializeField] private LayerMask obstacleLayers = Physics.DefaultRaycastLayers;

        #endregion

        #region Private Fields

        private LineRenderer trajectoryLine;
        private readonly RaycastHit[] segmentHits = new RaycastHit[MaxHitsPerSegment];

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            trajectoryLine = GetComponent<LineRenderer>();
            trajectoryLine.useWorldSpace = true;
            trajectoryLine.enabled = false;
        }

        private void LateUpdate()
        {
            bool shouldShowPreview = wasteThrower.IsAiming;
            trajectoryLine.enabled = shouldShowPreview;

            if (!shouldShowPreview) return;

            DrawTrajectory(wasteThrower.LaunchStartPosition, wasteThrower.LaunchVelocity);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Samples the ballistic path p(t) = p0 + v0*t + 0.5*g*t^2 and stops
        /// the line at the first obstacle found between two consecutive points.
        /// </summary>
        private void DrawTrajectory(Vector3 startPosition, Vector3 initialVelocity)
        {
            trajectoryLine.positionCount = pointCount;
            trajectoryLine.SetPosition(0, startPosition);

            int visiblePointCount = pointCount;
            Vector3 previousPosition = startPosition;

            for (int pointIndex = 1; pointIndex < pointCount; pointIndex++)
            {
                float time = pointIndex * previewDuration / (pointCount - 1);

                Vector3 position =
                    startPosition
                    + initialVelocity * time
                    + 0.5f * Physics.gravity * time * time;

                if (TryFindObstacle(previousPosition, position, out Vector3 impactPoint))
                {
                    trajectoryLine.SetPosition(pointIndex, impactPoint);
                    visiblePointCount = pointIndex + 1;
                    break;
                }

                trajectoryLine.SetPosition(pointIndex, position);
                previousPosition = position;
            }

            // Reducing the count keeps the first points and drops the rest.
            trajectoryLine.positionCount = visiblePointCount;
        }

        /// <summary>
        /// Casts between two points of the curve and returns the closest hit,
        /// ignoring triggers and any waste item (including the one being aimed).
        /// </summary>
        private bool TryFindObstacle(Vector3 segmentStart, Vector3 segmentEnd, out Vector3 impactPoint)
        {
            impactPoint = segmentEnd;

            Vector3 segment = segmentEnd - segmentStart;
            float segmentLength = segment.magnitude;
            if (segmentLength <= Mathf.Epsilon) return false;

            int hitCount = Physics.RaycastNonAlloc(
                segmentStart, segment / segmentLength, segmentHits, segmentLength,
                obstacleLayers, QueryTriggerInteraction.Ignore);

            float closestDistance = float.MaxValue;
            bool foundObstacle = false;

            for (int hitIndex = 0; hitIndex < hitCount; hitIndex++)
            {
                RaycastHit hit = segmentHits[hitIndex];
                if (hit.rigidbody != null && hit.rigidbody.TryGetComponent(out WasteItem _)) continue;
                if (hit.distance >= closestDistance) continue;

                closestDistance = hit.distance;
                impactPoint = hit.point;
                foundObstacle = true;
            }

            return foundObstacle;
        }

        #endregion
    }
}
