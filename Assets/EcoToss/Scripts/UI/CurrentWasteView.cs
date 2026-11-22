using EcoToss.Core;
using TMPro;
using UnityEngine;

namespace EcoToss.UI
{
    /// <summary>
    /// HUD view for the current waste indicator (WastePanel).
    /// Listens to WasteSpawned and shows the DisplayName of the waste that is ready to throw.
    /// The hint is Post-MVP, so only the name is shown for now.
    /// </summary>
    public class CurrentWasteView : MonoBehaviour
    {
        #region Serialized Fields

        [Header("References")]
        [Tooltip("Label that shows the name of the current waste.")]
        [SerializeField] private TMP_Text wasteNameText;

        [Header("Text")]
        [Tooltip("Shown before the first waste appears, so the placeholder name from the scene never shows.")]
        [SerializeField] private string noWasteText = "—";

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            if (wasteNameText == null)
            {
                Debug.LogError($"[{nameof(CurrentWasteView)}] Waste name text is not assigned on '{name}'. Disabling.", this);
                enabled = false;
                return;
            }

            wasteNameText.text = noWasteText;
        }

        private void OnEnable()
        {
            GameEvents.WasteSpawned += HandleWasteSpawned;
        }

        private void OnDisable()
        {
            GameEvents.WasteSpawned -= HandleWasteSpawned;
        }

        #endregion

        #region Event Handlers

        private void HandleWasteSpawned(WasteData spawnedWaste)
        {
            wasteNameText.text = spawnedWaste != null ? spawnedWaste.DisplayName : noWasteText;
        }

        #endregion
    }
}
