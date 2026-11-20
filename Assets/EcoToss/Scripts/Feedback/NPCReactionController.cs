using System.Collections.Generic;
using EcoToss.Core;
using EcoToss.UI;
using UnityEngine;

namespace EcoToss.Feedback
{
    /// <summary>
    /// Decides which NPC reacts to each throw result and asks the ReactionPanelView to show it.
    /// Driven only by GameEvents (Observer): it never references gameplay components.
    /// Rules: correct = Positive, wrong bin or miss = Negative (chance + cooldown from config,
    /// random NPC without repeating in a row). A streak always shows and wins over Positive.
    /// The panel hides when the next waste spawns or the match ends.
    /// </summary>
    public class NPCReactionController : MonoBehaviour
    {
        #region Constants

        private const int MaxPercent = 100;

        #endregion

        #region Serialized Fields

        [Header("Config")]
        [SerializeField] private NPCReactionConfig reactionConfig;

        [Header("View")]
        [SerializeField] private ReactionPanelView reactionPanelView;

        #endregion

        #region Private Fields

        // One throw can raise WasteSortedCorrectly and CorrectStreakReached in either order,
        // so the outcome is stored and resolved once per frame in LateUpdate.
        private bool hasPendingReaction;
        private NPCReactionType pendingReactionType;

        private int throwsSinceLastReaction;
        private NPCProfile lastFeedbackSpeaker;

        // Reused list to avoid allocations on every throw.
        private readonly List<NPCProfile> eligibleSpeakers = new List<NPCProfile>();

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            if (reactionConfig == null || reactionPanelView == null)
            {
                Debug.LogError($"[{nameof(NPCReactionController)}] Config or ReactionPanelView is not assigned on '{name}'. Disabling.", this);
                enabled = false;
                return;
            }

            // Lets the very first result show a reaction even when a cooldown is set.
            throwsSinceLastReaction = reactionConfig.MinThrowsBetweenFeedback;
        }

        private void OnEnable()
        {
            GameEvents.WasteSortedCorrectly += HandleWasteSortedCorrectly;
            GameEvents.WasteSortedIncorrectly += HandleWasteSortedIncorrectly;
            GameEvents.WasteMissed += HandleWasteMissed;
            GameEvents.CorrectStreakReached += HandleCorrectStreakReached;
            GameEvents.WasteSpawned += HandleWasteSpawned;
            GameEvents.GameStateChanged += HandleGameStateChanged;
        }

        private void OnDisable()
        {
            GameEvents.WasteSortedCorrectly -= HandleWasteSortedCorrectly;
            GameEvents.WasteSortedIncorrectly -= HandleWasteSortedIncorrectly;
            GameEvents.WasteMissed -= HandleWasteMissed;
            GameEvents.CorrectStreakReached -= HandleCorrectStreakReached;
            GameEvents.WasteSpawned -= HandleWasteSpawned;
            GameEvents.GameStateChanged -= HandleGameStateChanged;
        }

        private void LateUpdate()
        {
            if (!hasPendingReaction) return;

            hasPendingReaction = false;

            // The last throw may have ended the match: the end-game UI takes over.
            if (!IsMatchPlaying) return;

            if (pendingReactionType == NPCReactionType.Streak)
                ShowStreakReaction();
            else
                TryShowFeedbackReaction(pendingReactionType);
        }

        #endregion

        #region Event Handlers

        private void HandleWasteSortedCorrectly(WasteData sortedWaste) => QueueReaction(NPCReactionType.Positive);

        private void HandleWasteSortedIncorrectly(WasteData sortedWaste, BinType binHit) => QueueReaction(NPCReactionType.Negative);

        private void HandleWasteMissed(WasteData missedWaste) => QueueReaction(NPCReactionType.Negative);

        private void HandleCorrectStreakReached(int streakCount) => QueueReaction(NPCReactionType.Streak);

        private void HandleWasteSpawned(WasteData spawnedWaste) => reactionPanelView.Hide();

        private void HandleGameStateChanged(GameState newState)
        {
            if (newState != GameState.Victory && newState != GameState.Defeat) return;

            hasPendingReaction = false;
            reactionPanelView.Hide();
        }

        #endregion

        #region Reaction Rules

        /// <summary>Stores this throw's reaction. A streak is never overwritten by Positive.</summary>
        private void QueueReaction(NPCReactionType reactionType)
        {
            if (!IsMatchPlaying) return;
            if (hasPendingReaction && pendingReactionType == NPCReactionType.Streak) return;

            pendingReactionType = reactionType;
            hasPendingReaction = true;
        }

        /// <summary>Streak: always shown, combined sprite with one shared phrase.</summary>
        private void ShowStreakReaction()
        {
            if (reactionConfig.StreakPortrait == null) return;

            reactionPanelView.ShowStreak(
                reactionConfig.StreakPortrait,
                reactionConfig.StreakDisplayName,
                reactionConfig.GetRandomStreakMessage());

            throwsSinceLastReaction = 0;
        }

        /// <summary>Positive / Negative: applies cooldown and chance, then draws one NPC.</summary>
        private void TryShowFeedbackReaction(NPCReactionType reactionType)
        {
            bool isCooldownOver = throwsSinceLastReaction >= reactionConfig.MinThrowsBetweenFeedback;
            bool passesChance = Random.Range(0, MaxPercent) < reactionConfig.FeedbackChancePercent;

            NPCProfile speaker = isCooldownOver && passesChance ? PickFeedbackSpeaker(reactionType) : null;

            if (speaker == null)
            {
                throwsSinceLastReaction++;
                return;
            }

            reactionPanelView.ShowFeedback(speaker, reactionType, speaker.GetRandomMessage(reactionType));

            lastFeedbackSpeaker = speaker;
            throwsSinceLastReaction = 0;
        }

        /// <summary>Random NPC that has this reaction, avoiding the previous speaker when possible.</summary>
        private NPCProfile PickFeedbackSpeaker(NPCReactionType reactionType)
        {
            eligibleSpeakers.Clear();
            foreach (NPCProfile candidate in reactionConfig.FeedbackCandidates)
            {
                if (candidate != null && candidate.HasReaction(reactionType))
                    eligibleSpeakers.Add(candidate);
            }

            if (eligibleSpeakers.Count > 1) eligibleSpeakers.Remove(lastFeedbackSpeaker);
            if (eligibleSpeakers.Count == 0) return null;

            return eligibleSpeakers[Random.Range(0, eligibleSpeakers.Count)];
        }

        // Same rule as KarmaSystem and TurnController: without a GameManager (test scenes) it still works.
        private static bool IsMatchPlaying =>
            GameManager.Instance == null || GameManager.Instance.CurrentState == GameState.Playing;

        #endregion
    }
}
