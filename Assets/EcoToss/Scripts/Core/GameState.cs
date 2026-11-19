namespace EcoToss.Core
{
    /// <summary>
    /// Possible states of a match. Only the GameManager can change them.
    /// </summary>
    public enum GameState
    {
        /// <summary>The rules are explained before the first match of the session.</summary>
        Briefing,

        /// <summary>The player is aiming and throwing waste.</summary>
        Playing,

        /// <summary>The game is stopped (Time.timeScale = 0).</summary>
        Paused,

        /// <summary>Karma reached 100.</summary>
        Victory,

        /// <summary>Karma reached 0.</summary>
        Defeat
    }
}
