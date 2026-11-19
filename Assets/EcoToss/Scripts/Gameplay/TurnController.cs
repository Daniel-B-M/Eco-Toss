using System.Collections;
using EcoToss.Core;
using UnityEngine;

namespace EcoToss.Gameplay
{
    /// <summary>
    /// Runs the core loop: Spawn -> Aiming -> InFlight -> Resolving -> next turn.
    /// It never throws, evaluates or creates waste itself: it only decides WHEN
    /// the SpawnManager and the WasteThrower act, reacting to GameEvents.
    /// </summary>
    public class TurnController : MonoBehaviour
    {
        #region Types

        private enum TurnPhase
        {
            Idle,
            Aiming,
            InFlight,
            Resolving
        }

        #endregion

        #region Serialized Fields

        [Header("References")]
        [SerializeField] private SpawnManager spawnManager;
        [SerializeField] private WasteThrower wasteThrower;

        [Header("Turn Timing")]
        [Tooltip("Seconds the evaluated waste stays visible before the next one appears.")]
        [SerializeField, Min(0f)] private float delayBeforeNextTurn = 1.5f;

        #endregion

        #region Private Fields

        private TurnPhase currentPhase = TurnPhase.Idle;
        private bool hasMatchStarted;
        private Coroutine nextTurnRoutine;

        #endregion

        #region Public Properties

        /// <summary>Adjustable at runtime (e.g. from an options menu).</summary>
        public float DelayBeforeNextTurn
        {
            get => delayBeforeNextTurn;
            set => delayBeforeNextTurn = Mathf.Max(0f, value);
        }

        #endregion

        #region Unity Lifecycle

        private void OnEnable()
        {
            GameEvents.GameStateChanged += HandleGameStateChanged;
            GameEvents.WasteSortedCorrectly += HandleWasteSortedCorrectly;
            GameEvents.WasteSortedIncorrectly += HandleWasteSortedIncorrectly;
            GameEvents.WasteMissed += HandleWasteMissed;
            wasteThrower.WasteLaunched += HandleWasteLaunched;
        }

        private void OnDisable()
        {
            GameEvents.GameStateChanged -= HandleGameStateChanged;
            GameEvents.WasteSortedCorrectly -= HandleWasteSortedCorrectly;
            GameEvents.WasteSortedIncorrectly -= HandleWasteSortedIncorrectly;
            GameEvents.WasteMissed -= HandleWasteMissed;
            wasteThrower.WasteLaunched -= HandleWasteLaunched;
            StopNextTurnRoutine();
        }

        #endregion

        #region Game State

        private void HandleGameStateChanged(GameState newState)
        {
            switch (newState)
            {
                case GameState.Playing:
                    // Only the first time: coming back from pause keeps the current turn.
                    if (hasMatchStarted) return;
                    hasMatchStarted = true;
                    StartNextTurn();
                    break;

                case GameState.Victory:
                case GameState.Defeat:
                    StopTurns();
                    break;
            }
        }

        private static bool IsMatchPlaying =>
            GameManager.Instance == null || GameManager.Instance.CurrentState == GameState.Playing;

        #endregion

        #region Turn Flow

        private void StartNextTurn()
        {
            // The last throw may have ended the match (Phase 3): don't spawn an extra waste.
            if (!IsMatchPlaying)
            {
                currentPhase = TurnPhase.Idle;
                return;
            }

            WasteItem nextWaste = spawnManager.SpawnNext();
            if (nextWaste == null)
            {
                currentPhase = TurnPhase.Idle;
                return;
            }

            currentPhase = TurnPhase.Aiming;
            wasteThrower.BeginAiming(nextWaste);
        }

        private void HandleWasteLaunched(WasteItem launchedWaste)
        {
            if (currentPhase != TurnPhase.Aiming) return;
            currentPhase = TurnPhase.InFlight;
        }

        private void HandleWasteSortedCorrectly(WasteData sortedWaste) => ResolveTurn();
        private void HandleWasteSortedIncorrectly(WasteData sortedWaste, BinType binHit) => ResolveTurn();
        private void HandleWasteMissed(WasteData missedWaste) => ResolveTurn();

        /// <summary>Any evaluation (correct, wrong or miss) closes the turn once.</summary>
        private void ResolveTurn()
        {
            if (currentPhase != TurnPhase.InFlight) return;

            currentPhase = TurnPhase.Resolving;
            nextTurnRoutine = StartCoroutine(NextTurnAfterDelayRoutine());
        }

        private IEnumerator NextTurnAfterDelayRoutine()
        {
            // Scaled time: freezes while paused.
            yield return new WaitForSeconds(delayBeforeNextTurn);
            nextTurnRoutine = null;

            spawnManager.DespawnCurrent();
            StartNextTurn();
        }

        private void StopTurns()
        {
            StopNextTurnRoutine();
            wasteThrower.CancelAiming();
            currentPhase = TurnPhase.Idle;
        }

        private void StopNextTurnRoutine()
        {
            if (nextTurnRoutine == null) return;

            StopCoroutine(nextTurnRoutine);
            nextTurnRoutine = null;
        }

        #endregion
    }
}
