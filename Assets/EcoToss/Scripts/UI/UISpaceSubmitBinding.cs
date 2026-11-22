using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace EcoToss.UI
{
    /// <summary>
    /// Adds Space as an extra "Submit" key for UI buttons, so menus can be confirmed
    /// with the same hand that plays (WASD / arrows + Space) — one-handed accessibility.
    /// Works with whatever UI input module is active in the scene, even one created at runtime.
    /// </summary>
    public class UISpaceSubmitBinding : MonoBehaviour
    {
        #region Constants

        private const string SpaceBindingPath = "<Keyboard>/space";

        #endregion

        #region Unity Lifecycle

        // Start (not Awake) so any EventSystem created in another script's Awake already exists.
        private void Start()
        {
            AddSpaceToSubmitAction();
        }

        #endregion

        #region Private Methods

        private void AddSpaceToSubmitAction()
        {
            InputAction submitAction = FindSubmitAction();
            if (submitAction == null)
            {
                Debug.LogWarning($"[{nameof(UISpaceSubmitBinding)}] No InputSystemUIInputModule with a Submit action was found. Space will not confirm UI buttons.", this);
                return;
            }

            // The default UI actions can be shared between scenes, so never add the binding twice.
            if (HasBinding(submitAction, SpaceBindingPath)) return;

            bool wasActionEnabled = submitAction.enabled;
            if (wasActionEnabled) submitAction.Disable();

            submitAction.AddBinding(SpaceBindingPath);

            if (wasActionEnabled) submitAction.Enable();
        }

        private static InputAction FindSubmitAction()
        {
            EventSystem eventSystem = EventSystem.current;
            if (eventSystem == null) return null;

            var uiInputModule = eventSystem.GetComponent<InputSystemUIInputModule>();
            if (uiInputModule == null || uiInputModule.submit == null) return null;

            return uiInputModule.submit.action;
        }

        private static bool HasBinding(InputAction action, string bindingPath)
        {
            foreach (InputBinding binding in action.bindings)
            {
                if (string.Equals(binding.path, bindingPath, System.StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        #endregion
    }
}
