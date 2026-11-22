using UnityEngine;

namespace EcoToss.Gameplay
{
    /// <summary>
    /// Tuning values for the Civic Karma rules. Data only: KarmaSystem reads them.
    /// Penalties are stored as positive numbers and subtracted by KarmaSystem.
    /// Karma limits (0 and 100) are fixed in KarmaSystem because KarmaChanged is a percentage.
    /// </summary>
    [CreateAssetMenu(fileName = "KarmaConfig", menuName = "EcoToss/Karma Config")]
    public class KarmaConfig : ScriptableObject
    {
        #region Starting Value

        [Header("Start")]
        [Tooltip("Karma percentage at the beginning of the match.")]
        [SerializeField, Range(0, 100)] private int startingKarma = 50;

        #endregion

        #region Rewards

        [Header("Rewards")]
        [Tooltip("Karma gained when the waste lands in the correct bin.")]
        [SerializeField, Min(0)] private int correctSortReward = 10;

        [Tooltip("Consecutive correct sorts needed to trigger the streak bonus (Neighborhood Pride).")]
        [SerializeField, Min(1)] private int streakLengthForBonus = 3;

        [Tooltip("Extra karma added on top of the correct sort reward when the streak is reached. Granted once per streak.")]
        [SerializeField, Min(0)] private int streakBonusReward = 15;

        #endregion

        #region Penalties

        [Header("Penalties (positive values, subtracted)")]
        [Tooltip("Karma lost when the waste lands in the wrong bin (Gossip).")]
        [SerializeField, Min(0)] private int wrongBinPenalty = 15;

        [Tooltip("Karma lost when the waste hits the floor or times out.")]
        [SerializeField, Min(0)] private int missPenalty = 20;

        #endregion

        #region Public Properties

        public int StartingKarma => startingKarma;
        public int CorrectSortReward => correctSortReward;
        public int StreakLengthForBonus => streakLengthForBonus;
        public int StreakBonusReward => streakBonusReward;
        public int WrongBinPenalty => wrongBinPenalty;
        public int MissPenalty => missPenalty;

        #endregion
    }
}
