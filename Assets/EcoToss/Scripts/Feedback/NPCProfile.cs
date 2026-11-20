using System;
using UnityEngine;

namespace EcoToss.Feedback
{
    /// <summary>
    /// Data of one neighborhood NPC: display name, portraits and phrases for positive and
    /// negative feedback. Pure data (no logic about when to react): the NPCReactionController
    /// decides that. The streak uses a combined sprite stored in NPCReactionConfig.
    /// A reaction without portrait is treated as "not available" for this NPC.
    /// </summary>
    [CreateAssetMenu(fileName = "NPCProfile_New", menuName = "EcoToss/Feedback/NPC Profile")]
    public class NPCProfile : ScriptableObject
    {
        #region Nested Types

        /// <summary>Portrait plus the pool of phrases the NPC can say for one reaction.</summary>
        [Serializable]
        public class ReactionContent
        {
            [SerializeField] private Sprite portrait;
            [SerializeField, TextArea(1, 3)] private string[] messages = Array.Empty<string>();

            public Sprite Portrait => portrait;
            public string[] Messages => messages;
        }

        #endregion

        #region Serialized Fields

        [Header("Identity")]
        [SerializeField] private string displayName;

        [Header("Feedback Reactions")]
        [SerializeField] private ReactionContent positiveReaction = new ReactionContent();
        [SerializeField] private ReactionContent negativeReaction = new ReactionContent();

        #endregion

        #region Public API

        public string DisplayName => displayName;

        /// <summary>True when this NPC has a portrait for the given reaction.</summary>
        public bool HasReaction(NPCReactionType reactionType) => GetPortrait(reactionType) != null;

        /// <summary>Portrait for Positive or Negative. Returns null for any other reaction.</summary>
        public Sprite GetPortrait(NPCReactionType reactionType)
        {
            ReactionContent reactionContent = GetReactionContent(reactionType);
            return reactionContent != null ? reactionContent.Portrait : null;
        }

        /// <summary>Random phrase for Positive or Negative, or an empty string when none are set.</summary>
        public string GetRandomMessage(NPCReactionType reactionType)
        {
            ReactionContent reactionContent = GetReactionContent(reactionType);
            string[] messagePool = reactionContent != null ? reactionContent.Messages : null;
            if (messagePool == null || messagePool.Length == 0) return string.Empty;

            return messagePool[UnityEngine.Random.Range(0, messagePool.Length)];
        }

        #endregion

        #region Helpers

        private ReactionContent GetReactionContent(NPCReactionType reactionType)
        {
            switch (reactionType)
            {
                case NPCReactionType.Positive: return positiveReaction;
                case NPCReactionType.Negative: return negativeReaction;
                default:                       return null;
            }
        }

        #endregion
    }
}
