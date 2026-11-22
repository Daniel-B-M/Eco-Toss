using System;
using UnityEngine;

namespace EcoToss.Feedback
{
    /// <summary>
    /// Tuning data for in-match NPC reactions: how often feedback appears, which NPCs
    /// can be drawn for positive/negative feedback and what the streak reaction shows.
    /// Data-only: the NPCReactionController applies these rules.
    /// </summary>
    [CreateAssetMenu(fileName = "NPCReactionConfig", menuName = "EcoToss/Feedback/NPC Reaction Config")]
    public class NPCReactionConfig : ScriptableObject
    {
        #region Serialized Fields

        [Header("Feedback Frequency (Positive / Negative)")]
        [Tooltip("Chance that a throw result shows an NPC reaction. Streaks ignore it.")]
        [SerializeField, Range(0, 100)] private int feedbackChancePercent = 100;

        [Tooltip("Throws that must pass after a reaction before another can appear. 0 = no cooldown.")]
        [SerializeField, Min(0)] private int minThrowsBetweenFeedback = 0;

        [Header("Feedback Candidates")]
        [Tooltip("NPCs drawn at random (no repeats in a row) for positive and negative feedback.")]
        [SerializeField] private NPCProfile[] feedbackCandidates = Array.Empty<NPCProfile>();

        [Header("Streak")]
        [Tooltip("Combined portrait of the NPCs that celebrate the streak.")]
        [SerializeField] private Sprite streakPortrait;

        [Tooltip("Name shown above the streak phrase.")]
        [SerializeField] private string streakDisplayName = "Don Jacinto y Doña Gloria";

        [Tooltip("Shared phrases for the streak reaction (one is picked at random).")]
        [SerializeField, TextArea(1, 3)] private string[] streakMessages = Array.Empty<string>();

        #endregion

        #region Public API

        public int FeedbackChancePercent => feedbackChancePercent;
        public int MinThrowsBetweenFeedback => minThrowsBetweenFeedback;
        public NPCProfile[] FeedbackCandidates => feedbackCandidates;
        public Sprite StreakPortrait => streakPortrait;
        public string StreakDisplayName => streakDisplayName;

        /// <summary>Random shared streak phrase, or an empty string when none are set.</summary>
        public string GetRandomStreakMessage()
        {
            if (streakMessages == null || streakMessages.Length == 0) return string.Empty;

            return streakMessages[UnityEngine.Random.Range(0, streakMessages.Length)];
        }

        #endregion
    }
}
