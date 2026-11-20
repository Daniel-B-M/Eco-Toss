using EcoToss.Core;
using UnityEngine;

namespace EcoToss.Gameplay
{
    /// <summary>
    /// Applies the Civic Karma rules. Listens to the throw outcome events, keeps the
    /// correct-sort streak, clamps karma between 0 and 100 and raises KarmaChanged.
    /// Ends the match through the GameManager: 100 = Victory, 0 = Defeat.
    /// It never touches UI, audio or NPCs: they react to the events.
    /// </summary>
    public class KarmaSystem : MonoBehaviour
    {
        #region Constants

        private const int MinKarma = 0;
        private const int MaxKarma = 100;

        #endregion

        #region Serialized Fields

        [Header("Config")]
        [SerializeField] private KarmaConfig karmaConfig;
        #endregion

        #region Private Fields

        private int currentKarma;
        private int consecutiveCorrectSorts;
        private bool hasStreakBonusBeenGranted;

        #endregion

        #region Public Properties

        public int CurrentKarma => currentKarma;
        public int ConsecutiveCorrectSorts => consecutiveCorrectSorts;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            if (karmaConfig == null)
            {
                Debug.LogError($"[{nameof(KarmaSystem)}] KarmaConfig is not assigned on '{name}'. Disabling.", this);
                enabled = false;
                return;
            }

            currentKarma = Mathf.Clamp(karmaConfig.StartingKarma, MinKarma, MaxKarma);
        }

        private void OnEnable()
        {
            GameEvents.WasteSortedCorrectly += HandleWasteSortedCorrectly;
            GameEvents.WasteSortedIncorrectly += HandleWasteSortedIncorrectly;
            GameEvents.WasteMissed += HandleWasteMissed;
        }

        private void OnDisable()
        {
            GameEvents.WasteSortedCorrectly -= HandleWasteSortedCorrectly;
            GameEvents.WasteSortedIncorrectly -= HandleWasteSortedIncorrectly;
            GameEvents.WasteMissed -= HandleWasteMissed;
        }

        private void Start()
        {
            // Start (not Awake) so the HUD and other listeners are already subscribed.
            GameEvents.RaiseKarmaChanged(currentKarma);
            GameEvents.RaiseCorrectStreakChanged(consecutiveCorrectSorts);
        }

        #endregion

        #region Outcome Handlers

        private void HandleWasteSortedCorrectly(WasteData sortedWaste)
        {
            if (!IsMatchPlaying) return;

            consecutiveCorrectSorts++;
            GameEvents.RaiseCorrectStreakChanged(consecutiveCorrectSorts);
            int karmaDelta = karmaConfig.CorrectSortReward;

            // The bonus is granted once per streak; any failure re-arms it.
            bool reachedStreak = !hasStreakBonusBeenGranted
                && consecutiveCorrectSorts >= karmaConfig.StreakLengthForBonus;

            if (reachedStreak)
            {
                hasStreakBonusBeenGranted = true;
                karmaDelta += karmaConfig.StreakBonusReward;
                GameEvents.RaiseCorrectStreakReached(consecutiveCorrectSorts);
            }

            ApplyKarmaDelta(karmaDelta);
        }

        private void HandleWasteSortedIncorrectly(WasteData sortedWaste, BinType binHit)
        {
            if (!IsMatchPlaying) return;

            ResetStreak();
            ApplyKarmaDelta(-karmaConfig.WrongBinPenalty);
        }

        private void HandleWasteMissed(WasteData missedWaste)
        {
            if (!IsMatchPlaying) return;

            ResetStreak();
            ApplyKarmaDelta(-karmaConfig.MissPenalty);
        }

        #endregion

        #region Karma Rules

        private void ResetStreak()
        {
            consecutiveCorrectSorts = 0;
            hasStreakBonusBeenGranted = false;
            GameEvents.RaiseCorrectStreakChanged(consecutiveCorrectSorts);
        }

        /// <summary>Adds (or subtracts) karma once, clamps it, announces it and checks the match end.</summary>
        private void ApplyKarmaDelta(int karmaDelta)
        {
            currentKarma = Mathf.Clamp(currentKarma + karmaDelta, MinKarma, MaxKarma);
            GameEvents.RaiseKarmaChanged(currentKarma);
            CheckMatchEnd();
        }

        private void CheckMatchEnd()
        {
            if (GameManager.Instance == null) return;

            if (currentKarma >= MaxKarma)
                GameManager.Instance.ChangeState(GameState.Victory);
            else if (currentKarma <= MinKarma)
                GameManager.Instance.ChangeState(GameState.Defeat);
        }

        // Same rule as TurnController: without a GameManager (test scenes) karma still works.
        private static bool IsMatchPlaying =>
            GameManager.Instance == null || GameManager.Instance.CurrentState == GameState.Playing;

        #endregion
    }
}
