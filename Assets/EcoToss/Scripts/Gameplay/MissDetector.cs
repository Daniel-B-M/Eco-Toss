using System.Collections;
using EcoToss.Core;
using UnityEngine;

namespace EcoToss.Gameplay
{
    /// <summary>
    /// Detects failed throws in two ways: the waste touches the play area floor,
    /// or it is still unresolved a few seconds after being launched
    /// (stuck on a rim, flew out of the area...). Raises WasteMissed once per throw.
    /// Lives on the invisible floor collider of the play area.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class MissDetector : MonoBehaviour
    {
        #region Serialized Fields

        [Header("References")]
        [SerializeField] private WasteThrower wasteThrower;

        [Header("Timeout")]
        [Tooltip("Seconds after launch before an unresolved waste counts as a miss.")]
        [SerializeField, Min(0.5f)] private float missTimeoutSeconds = 4f;

        [Header("Debug")]
        [SerializeField] private bool logMissesInConsole = true;

        #endregion

        #region Private Fields

        private Coroutine missTimeoutRoutine;

        #endregion

        #region Unity Lifecycle

        private void OnEnable()
        {
            wasteThrower.WasteLaunched += HandleWasteLaunched;
        }

        private void OnDisable()
        {
            wasteThrower.WasteLaunched -= HandleWasteLaunched;
            StopMissTimeout();
        }

        private void OnCollisionEnter(Collision collision)
        {
            Rigidbody hitBody = collision.rigidbody;
            if (hitBody == null) return;
            if (!hitBody.TryGetComponent(out WasteItem landedWaste)) return;

            RegisterMiss(landedWaste, "hit the floor");
        }

        #endregion

        #region Timeout

        private void HandleWasteLaunched(WasteItem launchedWaste)
        {
            // Only one waste is in play at a time: a new launch replaces the old timeout.
            StopMissTimeout();
            missTimeoutRoutine = StartCoroutine(MissTimeoutRoutine(launchedWaste));
        }

        private IEnumerator MissTimeoutRoutine(WasteItem launchedWaste)
        {
            // Scaled time: the countdown also freezes while the game is paused.
            yield return new WaitForSeconds(missTimeoutSeconds);
            missTimeoutRoutine = null;

            // Skip if the item already went back to the pool (inactive or frozen again).
            if (launchedWaste == null || !launchedWaste.gameObject.activeInHierarchy || !launchedWaste.IsInFlight)
                yield break;

            RegisterMiss(launchedWaste, "timed out");
        }

        private void StopMissTimeout()
        {
            if (missTimeoutRoutine == null) return;

            StopCoroutine(missTimeoutRoutine);
            missTimeoutRoutine = null;
        }

        #endregion

        #region Evaluation

        /// <summary>Raises WasteMissed unless a bin (or a previous miss) already resolved this throw.</summary>
        private void RegisterMiss(WasteItem missedWaste, string reason)
        {
            if (!missedWaste.TryMarkAsResolved()) return;

            if (logMissesInConsole)
                Debug.Log($"[{nameof(MissDetector)}] {missedWaste.Data.DisplayName} MISSED ({reason})", this);

            GameEvents.RaiseWasteMissed(missedWaste.Data);
        }

        #endregion
    }
}
