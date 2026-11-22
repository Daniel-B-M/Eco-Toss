using System.Collections;
using EcoToss.Core;
using EcoToss.Gameplay;
using TMPro;
using UnityEngine;

namespace EcoToss.UI
{
    /// <summary>
    /// HUD banner that announces the streak bonus (e.g. "¡Racha x3! +8 karma").
    /// Listens only to GameEvents (Observer pattern): it appears with a scale pop and stays
    /// until the next waste spawns, the same moment the NPC reaction hides, then fades out. The bonus value is read from KarmaConfig,
    /// so the text stays correct if the balance changes.
    /// </summary>
    public class StreakBannerView : MonoBehaviour
    {
        #region Serialized Fields

        [Header("References")]
        [Tooltip("CanvasGroup on the banner root. Controls the fade and blocks no clicks.")]
        [SerializeField] private CanvasGroup bannerCanvasGroup;

        [Tooltip("RectTransform that gets the scale pop (usually the banner root).")]
        [SerializeField] private RectTransform bannerRect;

        [Tooltip("Label that shows the streak message.")]
        [SerializeField] private TMP_Text bannerText;

        [Tooltip("Same KarmaConfig asset used by KarmaSystem. Provides the bonus value shown in the text.")]
        [SerializeField] private KarmaConfig karmaConfig;

        [Header("Text Format")]
        [Tooltip("{0} = streak length, {1} = bonus karma.")]
        [SerializeField] private string bannerTextFormat = "¡Racha x{0}! +{1} karma";

        [Header("Animation (scaled time: freezes while paused)")]
        [SerializeField, Min(0.01f)] private float popDuration = 0.25f;
        [SerializeField, Min(0.01f)] private float fadeOutDuration = 0.4f;

        [Tooltip("Scale over the pop (time 0 to 1). Going above 1 and back gives the 'pop' feel.")]
        [SerializeField] private AnimationCurve popScaleCurve = new AnimationCurve(
            new Keyframe(0f, 0.5f),
            new Keyframe(0.6f, 1.15f),
            new Keyframe(1f, 1f));

        #endregion

        #region Private Fields

        private Coroutine bannerRoutine;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            if (bannerCanvasGroup == null || bannerRect == null || bannerText == null || karmaConfig == null)
            {
                Debug.LogError($"[{nameof(StreakBannerView)}] Missing references on '{name}'. Disabling.", this);
                enabled = false;
                return;
            }

            bannerCanvasGroup.blocksRaycasts = false;
            bannerCanvasGroup.interactable = false;
            HideBannerImmediately();
        }

        private void OnEnable()
        {
            GameEvents.CorrectStreakReached += HandleCorrectStreakReached;
            GameEvents.WasteSpawned += HandleWasteSpawned;
            GameEvents.GameStateChanged += HandleGameStateChanged;
        }

        private void OnDisable()
        {
            GameEvents.CorrectStreakReached -= HandleCorrectStreakReached;
            GameEvents.WasteSpawned -= HandleWasteSpawned;
            GameEvents.GameStateChanged -= HandleGameStateChanged;
        }

        #endregion

        #region Event Handlers

private void HandleCorrectStreakReached(int streakCount)
        {
            bannerText.text = string.Format(bannerTextFormat, streakCount, karmaConfig.StreakBonusReward);
            StartBannerRoutine(PopInRoutine());
        }

        private void HandleWasteSpawned(WasteData spawnedWaste)
        {
            // Synced with NPCReactionController, which hides the reaction on this same event.
            if (bannerCanvasGroup.alpha > 0f)
                StartBannerRoutine(FadeOutRoutine());
        }

        private void HandleGameStateChanged(GameState newState)
        {
            // The end overlay takes the screen: the banner must not stay on top of it.
            if (newState == GameState.Victory || newState == GameState.Defeat)
                HideBannerImmediately();
        }

        #endregion

        #region Banner Animation

private void StartBannerRoutine(IEnumerator routine)
        {
            if (bannerRoutine != null)
                StopCoroutine(bannerRoutine);

            bannerRoutine = StartCoroutine(routine);
        }

        /// <summary>Scale follows the curve while the alpha goes from 0 to 1. Then the banner stays visible.</summary>
        private IEnumerator PopInRoutine()
        {
            float elapsed = 0f;
            while (elapsed < popDuration)
            {
                elapsed += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsed / popDuration);
                bannerRect.localScale = Vector3.one * popScaleCurve.Evaluate(progress);
                bannerCanvasGroup.alpha = progress;
                yield return null;
            }

            bannerRect.localScale = Vector3.one;
            bannerCanvasGroup.alpha = 1f;
            bannerRoutine = null;
        }

        /// <summary>Fades from the current alpha to 0.</summary>
        private IEnumerator FadeOutRoutine()
        {
            float startAlpha = bannerCanvasGroup.alpha;
            float elapsed = 0f;
            while (elapsed < fadeOutDuration)
            {
                elapsed += Time.deltaTime;
                bannerCanvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, Mathf.Clamp01(elapsed / fadeOutDuration));
                yield return null;
            }

            bannerCanvasGroup.alpha = 0f;
            bannerRect.localScale = Vector3.one;
            bannerRoutine = null;
        }

        private void HideBannerImmediately()
        {
            if (bannerRoutine != null)
            {
                StopCoroutine(bannerRoutine);
                bannerRoutine = null;
            }

            bannerCanvasGroup.alpha = 0f;
            bannerRect.localScale = Vector3.one;
        }

        #endregion
    }
}
