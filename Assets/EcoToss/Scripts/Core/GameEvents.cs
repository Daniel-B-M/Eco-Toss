using System;
using UnityEngine;

namespace EcoToss.Core
{
    /// <summary>
    /// Canal central de eventos del juego (patrón Observer).
    /// Quien produce algo llama a un método Raise...; quien quiere reaccionar
    /// se suscribe al evento en OnEnable y se desuscribe en OnDisable.
    /// </summary>
    public static class GameEvents
    {
        // ---------- Turno ----------

        /// <summary>Un residuo nuevo está listo para lanzar.</summary>
        public static event Action<WasteData> WasteSpawned;

        /// <summary>El jugador lanzó el residuo.</summary>
        public static event Action<WasteData> WasteThrown;

        // ---------- Resultado del lanzamiento ----------

        /// <summary>El residuo cayó en la caneca correcta.</summary>
        public static event Action<WasteData> WasteSortedCorrectly;

        /// <summary>El residuo cayó en una caneca equivocada. Incluye la caneca en la que cayó.</summary>
        public static event Action<WasteData, BinType> WasteSortedIncorrectly;

        /// <summary>El residuo no entró en ninguna caneca (cayó al piso o se acabó el tiempo).</summary>
        public static event Action<WasteData> WasteMissed;

        // ---------- Progreso ----------

        /// <summary>Se alcanzó una racha de aciertos seguidos. Incluye el número de aciertos.</summary>
        public static event Action<int> CorrectStreakReached;

        /// <summary>El karma cambió. Incluye el valor nuevo, de 0 a 100.</summary>
        public static event Action<int> KarmaChanged;

        // ---------- Métodos para disparar los eventos ----------

        public static void RaiseWasteSpawned(WasteData wasteData) => WasteSpawned?.Invoke(wasteData);
        public static void RaiseWasteThrown(WasteData wasteData) => WasteThrown?.Invoke(wasteData);
        public static void RaiseWasteSortedCorrectly(WasteData wasteData) => WasteSortedCorrectly?.Invoke(wasteData);
        public static void RaiseWasteSortedIncorrectly(WasteData wasteData, BinType binHit) => WasteSortedIncorrectly?.Invoke(wasteData, binHit);
        public static void RaiseWasteMissed(WasteData wasteData) => WasteMissed?.Invoke(wasteData);
        public static void RaiseCorrectStreakReached(int streakCount) => CorrectStreakReached?.Invoke(streakCount);
        public static void RaiseKarmaChanged(int karmaPercent) => KarmaChanged?.Invoke(karmaPercent);

        /// <summary>
        /// Limpia todas las suscripciones al iniciar el juego. Evita que queden suscriptores
        /// de una partida anterior cuando el "Domain Reload" del editor está desactivado.
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
        }
    }
}
