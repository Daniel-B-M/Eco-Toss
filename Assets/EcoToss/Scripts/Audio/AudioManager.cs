using EcoToss.Core;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

namespace EcoToss.Audio
{
    /// <summary>
    /// Plays the background music and every sound effect of the game (Singleton pattern).
    /// Survives scene changes (DontDestroyOnLoad) so the music never restarts between MainMenu and Gameplay.
    /// Gameplay sounds react to GameEvents (Observer pattern); no gameplay script references this class.
    /// Volume is controlled through an AudioMixer and saved in PlayerPrefs.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        #region Constants
        // Names of the exposed parameters in the AudioMixer. They must match exactly.
        private const string MasterVolumeParameter = "MasterVolume";
        private const string MusicVolumeParameter = "MusicVolume";
        private const string SfxVolumeParameter = "SfxVolume";

        // PlayerPrefs keys used to remember the volume between sessions.
        private const string MasterVolumeKey = "EcoToss.MasterVolume";
        private const string MusicVolumeKey = "EcoToss.MusicVolume";
        private const string SfxVolumeKey = "EcoToss.SfxVolume";

        private const float DefaultVolume = 0.8f;
        private const float MinimumLinearVolume = 0.0001f; // Avoids Log10(0) = -infinity.
        #endregion

        #region Singleton
        public static AudioManager Instance { get; private set; }

        private void Awake()
        {
            // The same prefab lives in MainMenu and Gameplay so any scene can be tested alone.
            // The first one created stays; any later copy destroys itself.
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }
        #endregion

        #region Inspector References
        [Header("Mixer")]
        [SerializeField] private AudioMixer audioMixer;

        [Header("Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;

        [Header("Music")]
        [SerializeField] private AudioClip backgroundMusicClip;
        [Tooltip("Music level (0-1) in the main menu.")]
        [SerializeField, Range(0f, 1f)] private float menuMusicLevel = 1f;
        [Tooltip("Music level (0-1) during the match. Low so it stays in the background.")]
        [SerializeField, Range(0f, 1f)] private float gameplayMusicLevel = 0.25f;
        [Tooltip("Music level (0-1) while the game is paused or the match has ended.")]
        [SerializeField, Range(0f, 1f)] private float duckedMusicLevel = 0.12f;

        // Level the music source is using right now. Starts at the menu level.
        private float currentMusicLevel = -1f;

        [Header("Gameplay SFX")]
        [SerializeField] private AudioClip throwWhooshClip;
        [SerializeField] private AudioClip correctSortClip;
        [SerializeField] private AudioClip wrongSortClip;
        [SerializeField] private AudioClip correctStreakClip;
        [SerializeField] private AudioClip victoryClip;
        [SerializeField] private AudioClip defeatClip;

        [Header("UI SFX")]
        [SerializeField] private AudioClip buttonHoverClip;
        [SerializeField] private AudioClip buttonSelectClip;

        // The throw result, the streak and the match end can all be raised in the same frame.
        // Each one queues its sound with a priority and LateUpdate plays only the most important one.
        private enum OutcomeSoundPriority { None, Sort, Streak, MatchEnd }

        private AudioClip pendingOutcomeClip;
        private OutcomeSoundPriority pendingOutcomePriority = OutcomeSoundPriority.None;
        #endregion

        #region Unity Lifecycle
        private void Start()
        {
            // AudioMixer.SetFloat does not work reliably in Awake, so saved volumes are applied here.
            ApplySavedVolumes();
            StartBackgroundMusic();
        }

private void LateUpdate()
        {
            // Resolved once per frame, after every gameplay event of this frame was raised.
            if (pendingOutcomePriority == OutcomeSoundPriority.None)
                return;

            PlaySfx(pendingOutcomeClip);
            pendingOutcomeClip = null;
            pendingOutcomePriority = OutcomeSoundPriority.None;
        }


        private void OnEnable()
        {
            GameEvents.WasteThrown += HandleWasteThrown;
            GameEvents.WasteSortedCorrectly += HandleWasteSortedCorrectly;
            GameEvents.WasteSortedIncorrectly += HandleWasteSortedIncorrectly;
            GameEvents.WasteMissed += HandleWasteMissed;
            GameEvents.CorrectStreakReached += HandleCorrectStreakReached;
            GameEvents.GameStateChanged += HandleGameStateChanged;
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }

        private void OnDisable()
        {
            GameEvents.WasteThrown -= HandleWasteThrown;
            GameEvents.WasteSortedCorrectly -= HandleWasteSortedCorrectly;
            GameEvents.WasteSortedIncorrectly -= HandleWasteSortedIncorrectly;
            GameEvents.WasteMissed -= HandleWasteMissed;
            GameEvents.CorrectStreakReached -= HandleCorrectStreakReached;
            GameEvents.GameStateChanged -= HandleGameStateChanged;
            SceneManager.sceneLoaded -= HandleSceneLoaded;
        }
        #endregion

        #region Music
private void StartBackgroundMusic()
        {
            if (musicSource == null || backgroundMusicClip == null)
                return;

            // If no scene or state has set a level yet, start at the menu level.
            if (currentMusicLevel < 0f)
                currentMusicLevel = menuMusicLevel;

            musicSource.clip = backgroundMusicClip;
            musicSource.loop = true;
            musicSource.volume = currentMusicLevel;
            musicSource.Play();
        }

        /// <summary>Lowers the music (pause, victory, defeat) or brings it back to full volume.</summary>
/// <summary>Sets the music source level (menu, gameplay or ducked). The mixer slider still scales it.</summary>
        private void SetMusicLevel(float level)
        {
            currentMusicLevel = level;

            if (musicSource != null)
                musicSource.volume = level;
        }
        #endregion

        #region Sound Effects
        private void PlaySfx(AudioClip clip)
        {
            // PlayOneShot lets several effects overlap on the same source (e.g. success + streak).
            if (sfxSource != null && clip != null)
                sfxSource.PlayOneShot(clip);
        }

/// <summary>Keeps the outcome sound only if it is at least as important as the one already queued this frame.</summary>
        private void QueueOutcomeSound(AudioClip clip, OutcomeSoundPriority priority)
        {
            if (priority < pendingOutcomePriority)
                return;

            pendingOutcomeClip = clip;
            pendingOutcomePriority = priority;
        }


        /// <summary>Called by UI buttons when the pointer enters them.</summary>
        public void PlayButtonHover() => PlaySfx(buttonHoverClip);

        /// <summary>Called by UI buttons when they are clicked.</summary>
        public void PlayButtonSelect() => PlaySfx(buttonSelectClip);
        #endregion

        #region Game Event Handlers
        private void HandleWasteThrown(WasteData wasteData) => PlaySfx(throwWhooshClip);

        private void HandleWasteSortedCorrectly(WasteData wasteData) => QueueOutcomeSound(correctSortClip, OutcomeSoundPriority.Sort);

        private void HandleWasteSortedIncorrectly(WasteData wasteData, BinType binHit) => QueueOutcomeSound(wrongSortClip, OutcomeSoundPriority.Sort);

        private void HandleWasteMissed(WasteData wasteData) => QueueOutcomeSound(wrongSortClip, OutcomeSoundPriority.Sort);

        private void HandleCorrectStreakReached(int streakCount) => QueueOutcomeSound(correctStreakClip, OutcomeSoundPriority.Streak);

private void HandleGameStateChanged(GameState newState)
        {
            switch (newState)
            {
                case GameState.Paused:
                    SetMusicLevel(duckedMusicLevel);
                    break;

                case GameState.Victory:
                    SetMusicLevel(duckedMusicLevel);
                    QueueOutcomeSound(victoryClip, OutcomeSoundPriority.MatchEnd);
                    break;

                case GameState.Defeat:
                    SetMusicLevel(duckedMusicLevel);
                    QueueOutcomeSound(defeatClip, OutcomeSoundPriority.MatchEnd);
                    break;

                default: // Briefing and Playing
                    SetMusicLevel(gameplayMusicLevel);
                    break;
            }
        }

private void HandleSceneLoaded(Scene loadedScene, LoadSceneMode loadMode)
        {
            // Every scene starts at the menu level. In Gameplay, the GameManager raises
            // Briefing or Playing in its Start right after, which lowers it to the gameplay level.
            SetMusicLevel(menuMusicLevel);
        }
        #endregion

        #region Volume (used by the Settings sliders)
        public float MasterVolume => PlayerPrefs.GetFloat(MasterVolumeKey, DefaultVolume);
        public float MusicVolume => PlayerPrefs.GetFloat(MusicVolumeKey, DefaultVolume);
        public float SfxVolume => PlayerPrefs.GetFloat(SfxVolumeKey, DefaultVolume);

        /// <summary>Sets the master volume. Expects a slider value from 0 to 1.</summary>
        public void SetMasterVolume(float linearVolume) => SetVolume(MasterVolumeParameter, MasterVolumeKey, linearVolume);

        /// <summary>Sets the music volume. Expects a slider value from 0 to 1.</summary>
        public void SetMusicVolume(float linearVolume) => SetVolume(MusicVolumeParameter, MusicVolumeKey, linearVolume);

        /// <summary>Sets the sound effects volume. Expects a slider value from 0 to 1.</summary>
        public void SetSfxVolume(float linearVolume) => SetVolume(SfxVolumeParameter, SfxVolumeKey, linearVolume);

        private void SetVolume(string mixerParameter, string prefsKey, float linearVolume)
        {
            linearVolume = Mathf.Clamp01(linearVolume);
            PlayerPrefs.SetFloat(prefsKey, linearVolume);
            ApplyVolumeToMixer(mixerParameter, linearVolume);
        }

        private void ApplySavedVolumes()
        {
            ApplyVolumeToMixer(MasterVolumeParameter, MasterVolume);
            ApplyVolumeToMixer(MusicVolumeParameter, MusicVolume);
            ApplyVolumeToMixer(SfxVolumeParameter, SfxVolume);
        }

        private void ApplyVolumeToMixer(string mixerParameter, float linearVolume)
        {
            if (audioMixer == null)
                return;

            // The mixer works in decibels: 1 -> 0 dB, 0.5 -> about -6 dB, 0 -> -80 dB (silence).
            float volumeInDecibels = Mathf.Log10(Mathf.Max(linearVolume, MinimumLinearVolume)) * 20f;
            audioMixer.SetFloat(mixerParameter, volumeInDecibels);
        }
        #endregion
    }
}
