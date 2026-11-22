using EcoToss.Audio;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EcoToss.UI
{
    /// <summary>
    /// Plays the UI hover and select sounds for the Button on the same GameObject.
    /// Hover plays when the pointer enters; select plays on click (mouse or Enter, through onClick).
    /// Disabled buttons stay silent. The AudioManager is reached through AudioManager.Instance
    /// because it lives in another scene (DontDestroyOnLoad).
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class ButtonSoundEmitter : MonoBehaviour, IPointerEnterHandler
    {
        #region Private Fields

        private Button targetButton;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            targetButton = GetComponent<Button>();
        }

        private void OnEnable()
        {
            targetButton.onClick.AddListener(HandleButtonClicked);
        }

        private void OnDisable()
        {
            targetButton.onClick.RemoveListener(HandleButtonClicked);
        }

        #endregion

        #region Sound Triggers

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (targetButton.IsInteractable() && AudioManager.Instance != null)
                AudioManager.Instance.PlayButtonHover();
        }

        private void HandleButtonClicked()
        {
            // onClick only fires on interactable buttons, so no extra check is needed here.
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayButtonSelect();
        }

        #endregion
    }
}
