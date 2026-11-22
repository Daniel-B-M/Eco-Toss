using EcoToss.Audio;
using UnityEngine;
using UnityEngine.UI;

namespace EcoToss.UI
{
    /// <summary>
    /// Connects the three volume sliders of the Settings panel to the AudioManager.
    /// Lives on the root of the SettingsPanel prefab, so the Gameplay variant inherits it.
    /// The AudioManager comes from another scene (DontDestroyOnLoad), so it is reached through
    /// AudioManager.Instance instead of a [SerializeField] reference.
    /// </summary>
    public class VolumeSettingsView : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Sliders (range 0 to 1)")]
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            if (masterVolumeSlider == null || musicVolumeSlider == null || sfxVolumeSlider == null)
            {
                Debug.LogError($"[{nameof(VolumeSettingsView)}] Missing slider references on '{name}'. Disabling.", this);
                enabled = false;
                return;
            }

            masterVolumeSlider.onValueChanged.AddListener(HandleMasterVolumeChanged);
            musicVolumeSlider.onValueChanged.AddListener(HandleMusicVolumeChanged);
            sfxVolumeSlider.onValueChanged.AddListener(HandleSfxVolumeChanged);
        }

        private void OnEnable()
        {
            // Runs every time the panel opens, so the sliders always show the saved values.
            RefreshSlidersFromSavedVolume();
        }

        private void Start()
        {
            // Second refresh in case this panel was enabled before the AudioManager finished its Awake.
            RefreshSlidersFromSavedVolume();
        }

        private void OnDestroy()
        {
            if (masterVolumeSlider != null) masterVolumeSlider.onValueChanged.RemoveListener(HandleMasterVolumeChanged);
            if (musicVolumeSlider != null) musicVolumeSlider.onValueChanged.RemoveListener(HandleMusicVolumeChanged);
            if (sfxVolumeSlider != null) sfxVolumeSlider.onValueChanged.RemoveListener(HandleSfxVolumeChanged);
        }

        #endregion

        #region Slider Sync

        private void RefreshSlidersFromSavedVolume()
        {
            AudioManager audioManager = AudioManager.Instance;
            if (audioManager == null)
                return;

            // WithoutNotify: placing the handles must not save the same value again.
            masterVolumeSlider.SetValueWithoutNotify(audioManager.MasterVolume);
            musicVolumeSlider.SetValueWithoutNotify(audioManager.MusicVolume);
            sfxVolumeSlider.SetValueWithoutNotify(audioManager.SfxVolume);
        }

        #endregion

        #region Slider Handlers

        private void HandleMasterVolumeChanged(float sliderValue)
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.SetMasterVolume(sliderValue);
        }

        private void HandleMusicVolumeChanged(float sliderValue)
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.SetMusicVolume(sliderValue);
        }

        private void HandleSfxVolumeChanged(float sliderValue)
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.SetSfxVolume(sliderValue);
        }

        #endregion
    }
}
