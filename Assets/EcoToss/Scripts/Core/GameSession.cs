using UnityEngine;

namespace EcoToss.Core
{
    /// <summary>
    /// Data that must survive scene changes while the game is open
    /// (static fields are not destroyed when a scene is loaded).
    /// Everything resets when the game is closed and opened again.
    /// </summary>
    public static class GameSession
    {
        /// <summary>True once the player has already seen the briefing in this session.</summary>
        public static bool HasSeenBriefing { get; private set; }

        public static void MarkBriefingAsSeen() => HasSeenBriefing = true;

        /// <summary>
        /// Resets the session on game start, so it also works when the Editor's
        /// Domain Reload is disabled.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetSession()
        {
            HasSeenBriefing = false;
        }
    }
}
