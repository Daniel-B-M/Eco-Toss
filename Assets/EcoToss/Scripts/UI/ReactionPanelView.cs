using EcoToss.Feedback;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EcoToss.UI
{
    /// <summary>
    /// Passive view of the HUD reaction panel. Feedback uses the main portrait slot and the
    /// streak uses its own slot (combined sprite); only one slot is visible at a time.
    /// It never decides what to show; the NPCReactionController does.
    /// Visibility uses a CanvasGroup so the GameObject stays active.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class ReactionPanelView : MonoBehaviour
    {
        #region Serialized Fields

        [Header("References")]
        [SerializeField] private CanvasGroup panelCanvasGroup;
        [SerializeField] private Image feedbackPortraitImage;
        [SerializeField] private Image streakPortraitImage;
        [SerializeField] private TMP_Text speakerNameLabel;
        [SerializeField] private TMP_Text reactionMessageLabel;

        #endregion

        #region Public Properties

        public bool IsVisible => panelCanvasGroup != null && panelCanvasGroup.alpha > 0f;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            if (panelCanvasGroup == null) panelCanvasGroup = GetComponent<CanvasGroup>();
            panelCanvasGroup.interactable = false;
            panelCanvasGroup.blocksRaycasts = false;
            Hide();
        }

        #endregion

        #region Public API

        /// <summary>Shows one NPC's positive or negative reaction in the main portrait slot.</summary>
        public void ShowFeedback(NPCProfile speaker, NPCReactionType reactionType, string message)
        {
            if (speaker == null)
            {
                Hide();
                return;
            }

            SetPortrait(streakPortraitImage, null);
            SetPortrait(feedbackPortraitImage, speaker.GetPortrait(reactionType));
            ShowTexts(speaker.DisplayName, message);
        }

        /// <summary>Shows the streak reaction (combined sprite) in the streak portrait slot.</summary>
        public void ShowStreak(Sprite streakPortrait, string speakerName, string message)
        {
            SetPortrait(feedbackPortraitImage, null);
            SetPortrait(streakPortraitImage, streakPortrait);
            ShowTexts(speakerName, message);
        }

        public void Hide()
        {
            panelCanvasGroup.alpha = 0f;
        }

        #endregion

        #region Helpers

        private void ShowTexts(string speakerName, string message)
        {
            speakerNameLabel.text = speakerName.ToUpperInvariant();
            reactionMessageLabel.text = message;
            panelCanvasGroup.alpha = 1f;
        }

        /// <summary>Shows the slot with the sprite, or hides the slot when there is no sprite.</summary>
        private static void SetPortrait(Image portraitImage, Sprite portrait)
        {
            if (portraitImage == null) return;

            portraitImage.sprite = portrait;
            portraitImage.enabled = true; // Safety: the Image must stay on; visibility is the GameObject.
            portraitImage.gameObject.SetActive(portrait != null);
        }

        #endregion
    }
}
