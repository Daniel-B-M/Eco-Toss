using System;
using UnityEngine;

namespace EcoToss.Core
{
    /// <summary>
    /// Central game event bus implementing the Observer pattern.
    /// Producers invoke Raise... methods, while listeners subscribe in OnEnable and unsubscribe in OnDisable.
    /// </summary>
    public static class GameEvents
    {
        #region Turn Events
        /// <summary>Invoked when a new waste item is spawned and ready to throw.</summary>
        public static event Action<WasteData> WasteSpawned;

        /// <summary>Invoked when the player initiates a throw.</summary>
        public static event Action<WasteData> WasteThrown;
        #endregion

        #region Throw Outcome Events
        /// <summary>Invoked when the waste lands in the matching recycling bin.</summary>
        public static event Action<WasteData> WasteSortedCorrectly;

        /// <summary>Invoked when the waste lands in the wrong bin (includes target bin hit).</summary>
        public static event Action<WasteData, BinType> WasteSortedIncorrectly;

        /// <summary>Invoked when the waste misses all bins (ground hit or timer expiration).</summary>
        public static event Action<WasteData> WasteMissed;
        #endregion

        #region Progression Events
        /// <summary>Invoked when a consecutive correct sort streak is achieved.</summary>
        public static event Action<int> CorrectStreakReached;

        /// <summary>Invoked when player karma changes (normalized percentage: 0 to 100).</summary>
        public static event Action<int> KarmaChanged;
        #endregion

        #region Match Events
        /// <summary>Invoked when the match state changes (playing, paused, victory or defeat).</summary>
        public static event Action<GameState> GameStateChanged;
        #endregion

        #region Event Invocators
        public static void RaiseWasteSpawned(WasteData wasteData) => WasteSpawned?.Invoke(wasteData);
        public static void RaiseWasteThrown(WasteData wasteData) => WasteThrown?.Invoke(wasteData);
        public static void RaiseWasteSortedCorrectly(WasteData wasteData) => WasteSortedCorrectly?.Invoke(wasteData);
        public static void RaiseWasteSortedIncorrectly(WasteData wasteData, BinType binHit) => WasteSortedIncorrectly?.Invoke(wasteData, binHit);
        public static void RaiseWasteMissed(WasteData wasteData) => WasteMissed?.Invoke(wasteData);
        public static void RaiseCorrectStreakReached(int streakCount) => CorrectStreakReached?.Invoke(streakCount);
        public static void RaiseKarmaChanged(int karmaPercent) => KarmaChanged?.Invoke(karmaPercent);
        public static void RaiseGameStateChanged(GameState newState) => GameStateChanged?.Invoke(newState);
        #endregion

        #region Lifecycle Reset
        /// <summary>
        /// Resets all event delegate subscriptions on game initialization to prevent stale listeners
        /// when Editor Domain Reload is disabled.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ClearAllSubscribers()
        {
            WasteSpawned = null;
            WasteThrown = null;
            WasteSortedCorrectly = null;
            WasteSortedIncorrectly = null;
            WasteMissed = null;
            CorrectStreakReached = null;
            KarmaChanged = null;
            GameStateChanged = null;
        }
        #endregion
    }
}
